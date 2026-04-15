using System;
using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class CalculationHistory : INotifyPropertyChanged
    {
        private int _id;
        private Guid _projectId;
        private int _totalElements;
        private double _totalScore;
        private double _adaptivityMultiplier;
        private double _hoursPerPoint;
        private double _baseHours;
        private double _calculatedHours;
        private DateTime _calculationDate;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public Guid ProjectId
        {
            get => _projectId;
            set { _projectId = value; OnPropertyChanged(nameof(ProjectId)); }
        }

        public int TotalElements
        {
            get => _totalElements;
            set { _totalElements = value; OnPropertyChanged(nameof(TotalElements)); }
        }

        public double TotalScore
        {
            get => _totalScore;
            set { _totalScore = value; OnPropertyChanged(nameof(TotalScore)); }
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

        public double CalculatedHours
        {
            get => _calculatedHours;
            set
            {
                _calculatedHours = value;
                OnPropertyChanged(nameof(CalculatedHours));
                OnPropertyChanged(nameof(DisplayTime));
            }
        }

        public DateTime CalculationDate
        {
            get => _calculationDate;
            set
            {
                _calculationDate = value;
                OnPropertyChanged(nameof(CalculationDate));
                OnPropertyChanged(nameof(DisplayDate));
            }
        }

        public string DisplayDate => CalculationDate.ToString("dd.MM.yyyy HH:mm");
        public string DisplayTime
        {
            get
            {
                var hours = (int)CalculatedHours;
                var minutes = (int)((CalculatedHours - hours) * 60);
                return $"{hours} ч {minutes} мин";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}