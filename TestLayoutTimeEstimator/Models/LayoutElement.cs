using System;
using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class LayoutElement : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _type;
        private bool _hasAnimation;
        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private bool _isSelected;

        public Guid ID { get; } = Guid.NewGuid();

        public string Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public double X
        {
            get => _x;
            set
            {
                if (Math.Abs(_x - value) > 0.001)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (Math.Abs(_y - value) > 0.001)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                if (Math.Abs(_width - value) > 0.001)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (Math.Abs(_height - value) > 0.001)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public bool HasAnimation
        {
            get => _hasAnimation;
            set
            {
                if (_hasAnimation != value)
                {
                    _hasAnimation = value;
                    OnPropertyChanged(nameof(HasAnimation));
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public string DisplayName => $"{Type} ({X:F0}, {Y:F0})";

        // Реализация IDataErrorInfo
        public string Error => null; // Не используется, можно вернуть null

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(X):
                        if (double.IsNaN(X) || double.IsInfinity(X))
                            error = "Координата X должна быть числом";
                        // Допустимый диапазон от 0 до 5000 (можно настроить)
                        else if (X < 0)
                            error = "Координата X не может быть отрицательной";
                        else if (X > 5000)
                            error = "Координата X не может превышать 5000";
                        break;

                    case nameof(Y):
                        if (double.IsNaN(Y) || double.IsInfinity(Y))
                            error = "Координата Y должна быть числом";
                        else if (Y < 0)
                            error = "Координата Y не может быть отрицательной";
                        else if (Y > 5000)
                            error = "Координата Y не может превышать 5000";
                        break;

                    case nameof(Width):
                        if (double.IsNaN(Width) || double.IsInfinity(Width))
                            error = "Ширина должна быть числом";
                        else if (Width <= 0)
                            error = "Ширина должна быть положительной";
                        else if (Width > 2000)
                            error = "Ширина не может превышать 2000";
                        break;

                    case nameof(Height):
                        if (double.IsNaN(Height) || double.IsInfinity(Height))
                            error = "Высота должна быть числом";
                        else if (Height <= 0)
                            error = "Высота должна быть положительной";
                        else if (Height > 2000)
                            error = "Высота не может превышать 2000";
                        break;

                    case nameof(Type):
                        if (string.IsNullOrWhiteSpace(Type))
                            error = "Тип элемента не может быть пустым";
                        break;
                }

                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}