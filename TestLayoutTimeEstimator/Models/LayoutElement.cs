using System;
using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class LayoutElement : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
