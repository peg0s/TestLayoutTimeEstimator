using System.ComponentModel;

namespace TestLayoutTimeEstimator.Models
{
    public class ElementType : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private double _baseScore;
        private string _description;
        private string _category;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); OnPropertyChanged(nameof(DisplayName)); }
        }

        public double BaseScore
        {
            get => _baseScore;
            set { _baseScore = value; OnPropertyChanged(nameof(BaseScore)); OnPropertyChanged(nameof(DisplayName)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public string DisplayName => $"{Name} ({BaseScore} балл{(BaseScore != 1 ? "а" : "")})";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}