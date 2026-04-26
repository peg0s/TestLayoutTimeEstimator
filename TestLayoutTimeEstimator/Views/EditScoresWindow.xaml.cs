using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TestLayoutTimeEstimator.Models;

namespace TestLayoutTimeEstimator.Views
{
    public partial class EditScoresWindow : Window
    {
        // Коллекция, с которой работает окно (копия оригинальных данных)
        public ObservableCollection<ElementType> ElementTypes { get; set; }

        public EditScoresWindow(ObservableCollection<ElementType> elementTypes)
        {
            InitializeComponent();
            // Создаём глубокую копию, чтобы откатить при отмене
            ElementTypes = new ObservableCollection<ElementType>(
                elementTypes.Select(et => new ElementType
                {
                    Id = et.Id,
                    Name = et.Name,
                    BaseScore = et.BaseScore,
                    Category = et.Category,
                    Description = et.Description
                }));
            DataContext = this;
            UpdateStatus("Готов к редактированию");
        }

        private void UpdateStatus(string message, bool isError = false)
        {
            StatusMessageText.Text = message;
            StatusMessageText.Foreground = isError
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.DarkGreen;
        }

        private void AddNewType_Click(object sender, RoutedEventArgs e)
        {
            string newName = NewTypeName.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                UpdateStatus("Ошибка: название типа не может быть пустым", true);
                return;
            }
            if (ElementTypes.Any(et => et.Name.Equals(newName, System.StringComparison.OrdinalIgnoreCase)))
            {
                UpdateStatus($"Ошибка: тип \"{newName}\" уже существует", true);
                return;
            }

            int newId = ElementTypes.Any() ? ElementTypes.Max(et => et.Id) + 1 : 1;
            var newType = new ElementType
            {
                Id = newId,
                Name = newName,
                BaseScore = 1.0,
                Category = "Пользовательские",
                Description = "Создан пользователем"
            };
            ElementTypes.Add(newType);
            NewTypeName.Clear();
            UpdateStatus($"Тип \"{newName}\" добавлен с баллом 1.0");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация: проверка, что все баллы положительные
            foreach (var et in ElementTypes)
            {
                if (et.BaseScore <= 0)
                {
                    UpdateStatus($"Ошибка: балл для типа \"{et.Name}\" должен быть положительным", true);
                    return;
                }
                if (et.BaseScore > 100)
                {
                    UpdateStatus($"Ошибка: балл для типа \"{et.Name}\" не может превышать 100", true);
                    return;
                }
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}