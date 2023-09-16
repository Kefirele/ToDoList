using System.Windows;
using ToDoList.ViewModels;

namespace ToDoList.Main
{
    public partial class MainWindow : Window
    {

        private readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent();
            DataContext = _mainWindowViewModel;
        }
    }
}
