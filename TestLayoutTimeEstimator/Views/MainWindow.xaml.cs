using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TestLayoutTimeEstimator.Models;
using TestLayoutTimeEstimator.ViewModels;

namespace TestLayoutTimeEstimator.Views
{
    public partial class MainWindow : Window
    {
        private Canvas _layoutCanvas;
        private bool _isDraggingPotential;
        private bool _isDragging;
        private LayoutElement _draggedElement;
        private Point _dragStartMousePos;
        private Point _dragOffset;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Находим Canvas внутри ItemsControl после полной загрузки шаблона
            _layoutCanvas = FindVisualChild<Canvas>(MainItemsControl);
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is LayoutElement layoutElement)
            {
                if (DataContext is MainViewModel vm)
                {
                    vm.SelectedElement = layoutElement;
                }

                _isDraggingPotential = true;
                _draggedElement = layoutElement;
                _dragStartMousePos = e.GetPosition(this);
                Mouse.Capture(this);
                e.Handled = true;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
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
                        _dragOffset = new Point(
                            mouseOnCanvas.X - _draggedElement.X,
                            mouseOnCanvas.Y - _draggedElement.Y);
                    }
                }
            }

            if (_isDragging && _draggedElement != null && _layoutCanvas != null)
            {
                Point mouseOnCanvas = e.GetPosition(_layoutCanvas);
                double newX = mouseOnCanvas.X - _dragOffset.X;
                double newY = mouseOnCanvas.Y - _dragOffset.Y;
                _draggedElement.X = newX;
                _draggedElement.Y = newY;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDraggingPotential = false;
            _isDragging = false;
            _draggedElement = null;
            Mouse.Capture(null);
        }

        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}