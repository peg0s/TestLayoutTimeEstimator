using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TestLayoutTimeEstimator.Models;
using TestLayoutTimeEstimator.ViewModels;

namespace TestLayoutTimeEstimator.Views
{
    public partial class MainWindow : Window
    {
        private Canvas _layoutCanvas;       // основной Canvas для элементов
        private Canvas _selectionCanvas;    // верхний Canvas для выделения
        private bool _isDraggingPotential;
        private bool _isDragging;
        private LayoutElement _draggedElement;
        private Point _dragStartMousePos;
        private Point _dragOffset;

        // Для выделения области
        private bool _isSelecting;
        private Point _selectionStart;
        private Rectangle _selectionRectangle;

        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
        }

        private void MainItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            _layoutCanvas = FindVisualChild<Canvas>(MainItemsControl);
            _selectionCanvas = SelectionCanvas;

            if (_layoutCanvas != null && _selectionCanvas != null)
            {
                System.Diagnostics.Debug.WriteLine("Canvas найден и подписки установлены");
                ViewModel.StatusMessage = "Canvas готов. Режим выделения выключен.";
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Один из Canvas не найден!");
                ViewModel.StatusMessage = "Ошибка: Canvas не найден!";
            }
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) return t;
                var result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        // === Логика выделения (на верхнем Canvas) ===
        private void SelectionCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null || !ViewModel.IsSelectionMode) return;

            Point pos = e.GetPosition(_layoutCanvas);
            var element = FindElementAtPosition(pos);
            if (element != null) return; // нажат элемент – не начинаем выделение

            Point startPoint = e.GetPosition(_selectionCanvas);
            StartSelection(startPoint);
            e.Handled = true;
            System.Diagnostics.Debug.WriteLine("Выделение начато (верхний слой)");
        }

        private void SelectionCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting && _selectionCanvas != null)
            {
                Point currentPoint = e.GetPosition(_selectionCanvas);
                UpdateSelection(currentPoint);
                e.Handled = true;
            }
        }

        private void SelectionCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting && _selectionCanvas != null)
            {
                Point endPoint = e.GetPosition(_selectionCanvas);
                EndSelection(endPoint);
                e.Handled = true;
            }
        }

        private LayoutElement FindElementAtPosition(Point point)
        {
            if (_layoutCanvas == null) return null;
            foreach (var child in _layoutCanvas.Children)
            {
                if (child is FrameworkElement element && element.DataContext is LayoutElement layoutElement)
                {
                    double left = Canvas.GetLeft(element);
                    double top = Canvas.GetTop(element);
                    double width = element.Width;
                    double height = element.Height;
                    if (point.X >= left && point.X <= left + width && point.Y >= top && point.Y <= top + height)
                        return layoutElement;
                }
            }
            return null;
        }

        private void StartSelection(Point startPoint)
        {
            _isSelecting = true;
            _selectionStart = startPoint;
            _selectionRectangle = new Rectangle
            {
                Stroke = Brushes.DodgerBlue,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(80, 66, 133, 244)),
                StrokeDashArray = new DoubleCollection { 4, 2 }
            };
            Canvas.SetLeft(_selectionRectangle, startPoint.X);
            Canvas.SetTop(_selectionRectangle, startPoint.Y);
            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;
            _selectionCanvas.Children.Add(_selectionRectangle);

            if (ViewModel != null)
                ViewModel.StatusMessage = "Выделение области...";
        }

        private void UpdateSelection(Point currentPoint)
        {
            if (_selectionRectangle == null) return;
            double left = Math.Min(_selectionStart.X, currentPoint.X);
            double top = Math.Min(_selectionStart.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - _selectionStart.X);
            double height = Math.Abs(currentPoint.Y - _selectionStart.Y);
            Canvas.SetLeft(_selectionRectangle, left);
            Canvas.SetTop(_selectionRectangle, top);
            _selectionRectangle.Width = width;
            _selectionRectangle.Height = height;
        }

        private void EndSelection(Point endPoint)
        {
            if (_selectionRectangle == null) return;
            _selectionCanvas.Children.Remove(_selectionRectangle);
            _selectionRectangle = null;

            // Вычисляем координаты относительно основного Canvas
            double left = Math.Min(_selectionStart.X, endPoint.X);
            double top = Math.Min(_selectionStart.Y, endPoint.Y);
            double width = Math.Abs(endPoint.X - _selectionStart.X);
            double height = Math.Abs(endPoint.Y - _selectionStart.Y);
            if (width > 5 && height > 5)
                CreateElementAt(left, top, width, height);

            _isSelecting = false;
        }

        private void CreateElementAt(double x, double y, double width, double height)
        {
            if (ViewModel != null)
            {
                var newElement = new LayoutElement
                {
                    Type = "Текст",
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    HasAnimation = false
                };
                ViewModel.CurrentProject.Elements.Add(newElement);
                ViewModel.SelectedElement = newElement;
                ViewModel.StatusMessage = $"Добавлен элемент выделением: {newElement.DisplayName}";
            }
        }

        // === Перетаскивание элементов (без изменений) ===
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel != null && ViewModel.IsSelectionMode) return;

            if (sender is FrameworkElement element && element.DataContext is LayoutElement layoutElement)
            {
                if (ViewModel != null) ViewModel.SelectedElement = layoutElement;
                _isDraggingPotential = true;
                _draggedElement = layoutElement;
                _dragStartMousePos = e.GetPosition(this);
                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel != null && ViewModel.IsSelectionMode) return;

            if (_isDraggingPotential && _draggedElement != null)
            {
                Point currentPos = e.GetPosition(this);
                Vector diff = currentPos - _dragStartMousePos;
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    _isDragging = true;
                    _isDraggingPotential = false;
                    if (_layoutCanvas != null)
                    {
                        Point mouseOnCanvas = e.GetPosition(_layoutCanvas);
                        _dragOffset = new Point(mouseOnCanvas.X - _draggedElement.X,
                                                mouseOnCanvas.Y - _draggedElement.Y);
                    }
                }
            }

            if (_isDragging && _draggedElement != null && _layoutCanvas != null)
            {
                Point mouseOnCanvas = e.GetPosition(_layoutCanvas);
                _draggedElement.X = mouseOnCanvas.X - _dragOffset.X;
                _draggedElement.Y = mouseOnCanvas.Y - _dragOffset.Y;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel != null && ViewModel.IsSelectionMode) return;
            _isDraggingPotential = false;
            _isDragging = false;
            _draggedElement = null;
            Mouse.Capture(null);
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel != null && ViewModel.IsDirty)
            {
                var result = MessageBox.Show(
                    "Проект содержит несохранённые изменения. Сохранить перед выходом?",
                    "Несохранённые изменения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    ViewModel.SaveProjectCommand.Execute(null);
                    if (ViewModel.IsDirty) e.Cancel = true;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}