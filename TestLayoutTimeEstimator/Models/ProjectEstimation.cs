using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class ProjectEstimation : INotifyPropertyChanged
    {
        private string _name;
        private string _imagePath;
        private double _adaptivityMultiplier = 1.0;
        private double _hoursPerPoint = 0.25;
        private double _baseHours = 2.0;
        private DateTime _updatedAt;
        private ObservableCollection<LayoutElement> _elements = new();
        private int? _complexityCategoryId;   // новое поле

        public Guid Id { get; } = Guid.NewGuid();

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
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
            set { _adaptivityMultiplier = value; OnPropertyChanged(nameof(AdaptivityMultiplier)); }
        }

        public double HoursPerPoint
        {
            get => _hoursPerPoint;
            set { _hoursPerPoint = value; OnPropertyChanged(nameof(HoursPerPoint)); }
        }

        public double BaseHours
        {
            get => _baseHours;
            set { _baseHours = value; OnPropertyChanged(nameof(BaseHours)); }
        }

        public DateTime CreatedAt { get; } = DateTime.Now;

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { _updatedAt = value; OnPropertyChanged(nameof(UpdatedAt)); }
        }

        // Новые свойства для категории сложности
        public int? ComplexityCategoryId
        {
            get => _complexityCategoryId;
            set { _complexityCategoryId = value; OnPropertyChanged(nameof(ComplexityCategoryId)); }
        }

        // Навигационное свойство (для Entity Framework, если используется)
        public virtual ComplexityCategory ComplexityCategory { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
