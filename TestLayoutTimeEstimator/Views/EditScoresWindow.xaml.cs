using System.Collections.ObjectModel;
using System.Windows;
using TestLayoutTimeEstimator.Models;

namespace TestLayoutTimeEstimator.Views
{
    public partial class EditScoresWindow : Window
    {
        public ObservableCollection<ElementType> ElementTypes { get; set; }

        public EditScoresWindow(ObservableCollection<ElementType> elementTypes)
        {
            InitializeComponent();
            ElementTypes = new ObservableCollection<ElementType>(elementTypes);
            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
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