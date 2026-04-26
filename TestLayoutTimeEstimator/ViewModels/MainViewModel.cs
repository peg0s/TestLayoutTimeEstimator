using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TestLayoutTimeEstimator.Models;
using TestLayoutTimeEstimator.Services;
using TestLayoutTimeEstimator.Utilities;
using TestLayoutTimeEstimator.Views;

namespace TestLayoutTimeEstimator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ProjectEstimation _currentProject;
        private LayoutElement _selectedElement;
        private ElementType _selectedElementType;
        private ObservableCollection<ElementType> _elementTypes;
        private ObservableCollection<ComplexityCategory> _complexityCategories;
        private double _totalHours;
        private double _totalScore;
        private string _statusMessage;
        private bool _fitImageToCanvas = true;
        private bool _isSelectionMode = false;   // по умолчанию режим перемещения
        private bool _isDirty;

        private readonly IDatabaseService _databaseService;
        private PropertyChangedEventHandler _projectPropertyChangedHandler;
        private NotifyCollectionChangedEventHandler _elementsCollectionChangedHandler;
        private PropertyChangedEventHandler _elementPropertyChangedHandler;

        public ICommand AddElementCommand { get; }
        public ICommand RemoveElementCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand ClearImageCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand LoadProjectCommand { get; }
        public ICommand ClearAllCommand { get; }
        public ICommand NewProjectCommand { get; }
        public ICommand ToggleModeCommand { get; }
        public ICommand EditScoresCommand { get; }

        public MainViewModel()
        {
            _databaseService = new DatabaseService();
            LoadElementTypes();
            LoadComplexityCategories();

            _currentProject = new ProjectEstimation { Name = "Новый проект" };
            SubscribeToProjectEvents();

            AddElementCommand = new RelayCommand(AddElement, CanAddElement);
            RemoveElementCommand = new RelayCommand(RemoveElement, CanRemoveElement);
            LoadImageCommand = new RelayCommand(LoadImage);
            ClearImageCommand = new RelayCommand(ClearImage);
            SaveProjectCommand = new RelayCommand(SaveProject);
            LoadProjectCommand = new RelayCommand(LoadProject);
            ClearAllCommand = new RelayCommand(ClearAll);
            NewProjectCommand = new RelayCommand(NewProject);
            ToggleModeCommand = new RelayCommand(_ => IsSelectionMode = !IsSelectionMode);
            EditScoresCommand = new RelayCommand(EditScores);

            Recalculate();
            IsDirty = false;
        }

        // ===== Свойства =====

        public ProjectEstimation CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject == value) return;
                if (_currentProject != null)
                {
                    _currentProject.PropertyChanged -= _projectPropertyChangedHandler;
                    _currentProject.Elements.CollectionChanged -= _elementsCollectionChangedHandler;
                    UnsubscribeFromElements();
                }
                _currentProject = value;
                if (_currentProject != null)
                {
                    SubscribeToProjectEvents();
                    SubscribeToElements();
                }
                OnPropertyChanged(nameof(CurrentProject));
                OnPropertyChanged(nameof(CurrentProject.ImagePath));
                OnPropertyChanged(nameof(ElementsCount));
                Recalculate();
            }
        }

        public LayoutElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                if (_selectedElement == value) return;
                if (_selectedElement != null) _selectedElement.IsSelected = false;
                _selectedElement = value;
                if (_selectedElement != null) _selectedElement.IsSelected = true;
                OnPropertyChanged(nameof(SelectedElement));
                if (_selectedElement != null && _elementTypes != null)
                    SelectedElementType = _elementTypes.FirstOrDefault(et => et.Name == _selectedElement.Type);
                else
                    SelectedElementType = null;
            }
        }

        public ElementType SelectedElementType
        {
            get => _selectedElementType;
            set
            {
                if (value == _selectedElementType) return;
                _selectedElementType = value;
                OnPropertyChanged(nameof(SelectedElementType));
                if (_selectedElement != null && _selectedElementType != null)
                {
                    if (_selectedElement.Type != _selectedElementType.Name)
                    {
                        _selectedElement.Type = _selectedElementType.Name;
                        IsDirty = true;
                    }
                }
            }
        }

        public ObservableCollection<ElementType> ElementTypes
        {
            get => _elementTypes;
            set { _elementTypes = value; OnPropertyChanged(nameof(ElementTypes)); }
        }

        public ObservableCollection<ComplexityCategory> ComplexityCategories
        {
            get => _complexityCategories;
            set { _complexityCategories = value; OnPropertyChanged(nameof(ComplexityCategories)); }
        }

        public ComplexityCategory CurrentComplexityCategory
        {
            get
            {
                if (_currentProject == null || _currentProject.ComplexityCategoryId == null)
                    return null;
                return _complexityCategories?.FirstOrDefault(c => c.Id == _currentProject.ComplexityCategoryId);
            }
        }

        public double TotalHours
        {
            get => _totalHours;
            set { _totalHours = value; OnPropertyChanged(nameof(TotalHours)); OnPropertyChanged(nameof(DisplayTime)); }
        }

        public double TotalScore
        {
            get => _totalScore;
            set { _totalScore = value; OnPropertyChanged(nameof(TotalScore)); }
        }

        public bool IsSelectionMode
        {
            get => _isSelectionMode;
            set
            {
                if (_isSelectionMode == value) return;
                _isSelectionMode = value;
                OnPropertyChanged(nameof(IsSelectionMode));
                StatusMessage = value ? "✏️ Режим выделения: нарисуйте прямоугольник на холсте" : "🖱️ Режим перемещения: перетаскивайте элементы";
            }
        }

        public string DisplayTime
        {
            get
            {
                var hours = (int)_totalHours;
                var minutes = (int)((_totalHours - hours) * 60);
                return $"{hours} ч {minutes} мин";
            }
        }

        public int ElementsCount => CurrentProject?.Elements.Count ?? 0;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public bool FitImageToCanvas
        {
            get => _fitImageToCanvas;
            set { _fitImageToCanvas = value; OnPropertyChanged(nameof(FitImageToCanvas)); }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

        public string WindowTitle => IsDirty ? $"{CurrentProject?.Name}* - Layout Time Estimator" : $"{CurrentProject?.Name} - Layout Time Estimator";

        // ===== Подписки на события =====

        private void SubscribeToProjectEvents()
        {
            _projectPropertyChangedHandler = (s, e) =>
            {
                IsDirty = true;
                if (e.PropertyName == nameof(ProjectEstimation.AdaptivityMultiplier) ||
                    e.PropertyName == nameof(ProjectEstimation.HoursPerPoint) ||
                    e.PropertyName == nameof(ProjectEstimation.BaseHours))
                {
                    Recalculate();
                }
                else if (e.PropertyName == nameof(ProjectEstimation.ImagePath))
                {
                    OnPropertyChanged(nameof(CurrentProject));
                }
                else if (e.PropertyName == nameof(ProjectEstimation.Name))
                {
                    OnPropertyChanged(nameof(WindowTitle));
                }
            };

            _elementsCollectionChangedHandler = (s, e) =>
            {
                IsDirty = true;
                if (e.NewItems != null)
                {
                    foreach (LayoutElement element in e.NewItems)
                        element.PropertyChanged += _elementPropertyChangedHandler;
                }
                if (e.OldItems != null)
                {
                    foreach (LayoutElement element in e.OldItems)
                        element.PropertyChanged -= _elementPropertyChangedHandler;
                }
                Recalculate();
                OnPropertyChanged(nameof(ElementsCount));
            };

            _elementPropertyChangedHandler = (s, e) =>
            {
                IsDirty = true;
                if (e.PropertyName == nameof(LayoutElement.Type) ||
                    e.PropertyName == nameof(LayoutElement.HasAnimation) ||
                    e.PropertyName == nameof(LayoutElement.X) ||
                    e.PropertyName == nameof(LayoutElement.Y) ||
                    e.PropertyName == nameof(LayoutElement.Width) ||
                    e.PropertyName == nameof(LayoutElement.Height))
                {
                    Recalculate();
                }
            };

            _currentProject.PropertyChanged += _projectPropertyChangedHandler;
            _currentProject.Elements.CollectionChanged += _elementsCollectionChangedHandler;
        }

        private void SubscribeToElements()
        {
            foreach (var element in _currentProject.Elements)
                element.PropertyChanged += _elementPropertyChangedHandler;
        }

        private void UnsubscribeFromElements()
        {
            if (_currentProject != null)
                foreach (var element in _currentProject.Elements)
                    element.PropertyChanged -= _elementPropertyChangedHandler;
        }

        // ===== Методы команд =====

        private void AddElement(object parameter)
        {
            var element = new LayoutElement { Type = "Текст", X = 50, Y = 50, Width = 100, Height = 30 };
            CurrentProject.Elements.Add(element);
            SelectedElement = element;
            StatusMessage = $"Добавлен элемент: {element.DisplayName}";
        }
        private bool CanAddElement(object parameter) => CurrentProject != null;

        private void RemoveElement(object parameter)
        {
            if (SelectedElement != null)
            {
                var elementName = SelectedElement.DisplayName;
                CurrentProject.Elements.Remove(SelectedElement);
                SelectedElement = CurrentProject.Elements.FirstOrDefault();
                StatusMessage = $"Элемент удалён: {elementName}";
            }
        }
        private bool CanRemoveElement(object parameter) => SelectedElement != null;

        private void LoadImage(object parameter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Все файлы|*.*",
                Title = "Выберите изображение макета"
            };
            if (dialog.ShowDialog() == true)
            {
                CurrentProject.ImagePath = dialog.FileName;
                StatusMessage = $"Загружено изображение: {System.IO.Path.GetFileName(dialog.FileName)}";
                OnPropertyChanged(nameof(CurrentProject));
            }
        }
        private void EditScores(object parameter)
        {
            var dialog = new EditScoresWindow(_elementTypes);
            if (dialog.ShowDialog() == true)
            {
                // Обновляем коллекцию ElementTypes из диалога
                ElementTypes = new ObservableCollection<ElementType>(dialog.ElementTypes);
                // Сохраняем изменения в базу данных (опционально)
                // После изменения баллов пересчитываем время
                Recalculate();
                StatusMessage = "Баллы типов элементов обновлены";
            }
        }

        private void ClearImage(object parameter)
        {
            if (CurrentProject != null)
            {
                CurrentProject.ImagePath = null;
                StatusMessage = "Изображение удалено";
                OnPropertyChanged(nameof(CurrentProject));
            }
        }

        private async void SaveProject(object parameter)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Проект LayoutEstimator|*.lproj|Все файлы|*.*",
                Title = "Сохранить проект",
                FileName = $"{CurrentProject.Name}.lproj"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await ProjectFileService.SaveProjectAsync(dialog.FileName, CurrentProject);
                    IsDirty = false;
                    StatusMessage = $"Проект сохранён: {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка сохранения: {ex.Message}";
                }
            }
        }

        private async void LoadProject(object parameter)
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(
                    "Текущий проект содержит несохранённые изменения. Сохранить перед загрузкой?",
                    "Несохранённые изменения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProjectCommand.Execute(null);
                    if (IsDirty) return;
                }
                else if (result == MessageBoxResult.Cancel) return;
            }

            var dialog = new OpenFileDialog
            {
                Filter = "Проект LayoutEstimator|*.lproj|Все файлы|*.*",
                Title = "Загрузить проект"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var project = await ProjectFileService.LoadProjectAsync(dialog.FileName);
                    CurrentProject = project;
                    IsDirty = false;
                    StatusMessage = $"Проект загружен: {dialog.FileName}";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка загрузки: {ex.Message}";
                }
            }
        }

        private void ClearAll(object parameter)
        {
            if (CurrentProject.Elements.Any())
            {
                var result = MessageBox.Show("Удалить все элементы с макета?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    CurrentProject.Elements.Clear();
                    SelectedElement = null;
                    StatusMessage = "Все элементы удалены";
                }
            }
        }

        private void NewProject(object parameter)
        {
            if (IsDirty)
            {
                var result = MessageBox.Show(
                    "Текущий проект содержит несохранённые изменения. Сохранить перед созданием нового?",
                    "Несохранённые изменения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProjectCommand.Execute(null);
                    if (IsDirty) return;
                }
                else if (result == MessageBoxResult.Cancel) return;
            }
            CurrentProject = new ProjectEstimation { Name = "Новый проект" };
            IsDirty = false;
            StatusMessage = "Создан новый проект";
        }

        private void Recalculate()
        {
            if (CurrentProject == null) return;
            try
            {
                double totalScore = 0;
                foreach (var element in CurrentProject.Elements)
                {
                    var elementType = _elementTypes.FirstOrDefault(et => et.Name == element.Type);
                    if (elementType != null)
                    {
                        double elementScore = elementType.BaseScore * (element.HasAnimation ? 1.5 : 1.0);
                        totalScore += elementScore;
                    }
                }
                TotalScore = Math.Round(totalScore, 2);
                double hours = totalScore * CurrentProject.AdaptivityMultiplier * CurrentProject.HoursPerPoint + CurrentProject.BaseHours;
                TotalHours = Math.Round(hours, 2);
                UpdateComplexityCategory();
                StatusMessage = $"Расчёт выполнен. Всего элементов: {ElementsCount}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка расчёта: {ex.Message}";
            }
        }

        private void UpdateComplexityCategory()
        {
            if (CurrentProject == null || _complexityCategories == null) return;
            var category = _complexityCategories.FirstOrDefault(c => c.MinScore <= TotalScore && (c.MaxScore == null || TotalScore <= c.MaxScore));
            if (category != null)
                CurrentProject.ComplexityCategoryId = category.Id;
            OnPropertyChanged(nameof(CurrentComplexityCategory));
        }

        private void LoadElementTypes()
        {
            ElementTypes = new ObservableCollection<ElementType>
            {
                new ElementType { Id = 1, Name = "Текст", BaseScore = 1, Category = "Текст", Description = "Обычный текст" },
                new ElementType { Id = 2, Name = "Заголовок H1", BaseScore = 3, Category = "Текст", Description = "Главный заголовок" },
                new ElementType { Id = 3, Name = "Заголовок H2", BaseScore = 2, Category = "Текст", Description = "Подзаголовок" },
                new ElementType { Id = 4, Name = "Кнопка", BaseScore = 3, Category = "Интерактив", Description = "Кнопка с обработкой" },
                new ElementType { Id = 5, Name = "Картинка", BaseScore = 2, Category = "Графика", Description = "Изображение" },
                new ElementType { Id = 6, Name = "Иконка", BaseScore = 1, Category = "Графика", Description = "Маленькая иконка" },
                new ElementType { Id = 7, Name = "Поле ввода", BaseScore = 4, Category = "Формы", Description = "Текстовое поле" },
                new ElementType { Id = 8, Name = "Карточка", BaseScore = 5, Category = "Блоки", Description = "Карточка товара" },
                new ElementType { Id = 9, Name = "Сложный блок", BaseScore = 15, Category = "Блоки", Description = "Сложный составной блок" }
            };
        }

        private void LoadComplexityCategories()
        {
            ComplexityCategories = new ObservableCollection<ComplexityCategory>
            {
                new ComplexityCategory { Id = 1, Name = "Просто", MinScore = 0, MaxScore = 10, Color = "#4CAF50", Description = "Не требует усилий" },
                new ComplexityCategory { Id = 2, Name = "Средне", MinScore = 10.01, MaxScore = 30, Color = "#FF9800", Description = "Требует внимания" },
                new ComplexityCategory { Id = 3, Name = "Сложно", MinScore = 30.01, MaxScore = null, Color = "#F44336", Description = "Высокая сложность" }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}