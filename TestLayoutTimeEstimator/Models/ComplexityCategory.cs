using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class ComplexityCategory : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private double _minScore;
        private double? _maxScore;
        private string _color;
        private string _description;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public double MinScore
        {
            get => _minScore;
            set { _minScore = value; OnPropertyChanged(nameof(MinScore)); }
        }

        public double? MaxScore
        {
            get => _maxScore;
            set { _maxScore = value; OnPropertyChanged(nameof(MaxScore)); }
        }

        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(nameof(Color)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
