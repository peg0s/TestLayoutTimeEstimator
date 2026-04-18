using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class ProjectEstimation : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _name;
        private string _imagePath;
        private double _adaptivityMultiplier = 1.0;
        private double _hoursPerPoint = 0.25;
        private double _baseHours = 2.0;
        private DateTime _updatedAt;
        private ObservableCollection<LayoutElement> _elements = new();
        private int? _complexityCategoryId;

        public Guid Id { get; } = Guid.NewGuid();

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath != value)
                {
                    _imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public ObservableCollection<LayoutElement> Elements
        {
            get => _elements;
            set
            {
                _elements = value ?? new ObservableCollection<LayoutElement>();
                OnPropertyChanged(nameof(Elements));
            }
        }

        public double AdaptivityMultiplier
        {
            get => _adaptivityMultiplier;
            set
            {
                if (Math.Abs(_adaptivityMultiplier - value) > 0.001)
                {
                    _adaptivityMultiplier = value;
                    OnPropertyChanged(nameof(AdaptivityMultiplier));
                }
            }
        }

        public double HoursPerPoint
        {
            get => _hoursPerPoint;
            set
            {
                if (Math.Abs(_hoursPerPoint - value) > 0.001)
                {
                    _hoursPerPoint = value;
                    OnPropertyChanged(nameof(HoursPerPoint));
                }
            }
        }

        public double BaseHours
        {
            get => _baseHours;
            set
            {
                if (Math.Abs(_baseHours - value) > 0.001)
                {
                    _baseHours = value;
                    OnPropertyChanged(nameof(BaseHours));
                }
            }
        }

        public DateTime CreatedAt { get; } = DateTime.Now;

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
                OnPropertyChanged(nameof(UpdatedAt));
            }
        }

        public int? ComplexityCategoryId
        {
            get => _complexityCategoryId;
            set
            {
                _complexityCategoryId = value;
                OnPropertyChanged(nameof(ComplexityCategoryId));
            }
        }

        public virtual ComplexityCategory ComplexityCategory { get; set; }

        // Реализация IDataErrorInfo
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrWhiteSpace(Name))
                            error = "Название проекта не может быть пустым";
                        else if (Name.Length > 200)
                            error = "Название проекта не должно превышать 200 символов";
                        break;

                    case nameof(HoursPerPoint):
                        if (double.IsNaN(HoursPerPoint) || double.IsInfinity(HoursPerPoint))
                            error = "Часов за балл должно быть числом";
                        else if (HoursPerPoint <= 0)
                            error = "Часов за балл должно быть больше 0";
                        else if (HoursPerPoint > 10)
                            error = "Часов за балл не может превышать 10";
                        break;

                    case nameof(BaseHours):
                        if (double.IsNaN(BaseHours) || double.IsInfinity(BaseHours))
                            error = "Базовые часы должны быть числом";
                        else if (BaseHours < 0)
                            error = "Базовые часы не могут быть отрицательными";
                        else if (BaseHours > 100)
                            error = "Базовые часы не могут превышать 100";
                        break;

                    case nameof(AdaptivityMultiplier):
                        if (double.IsNaN(AdaptivityMultiplier) || double.IsInfinity(AdaptivityMultiplier))
                            error = "Коэффициент адаптивности должен быть числом";
                        else if (AdaptivityMultiplier < 0.5)
                            error = "Коэффициент адаптивности не может быть меньше 0.5";
                        else if (AdaptivityMultiplier > 3)
                            error = "Коэффициент адаптивности не может превышать 3";
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