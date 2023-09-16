using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Interfaces;

namespace ToDoList.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ToDoListContext _context;
        private readonly IDialogService _dialogService;

        private int _selectedTab;
        public int SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }
        private object? _toDoTasksSubView = null;
        public object? ToDoTasksSubView
        {
            get
            {
                return _toDoTasksSubView;
            }
            set
            {
                _toDoTasksSubView = value;
                OnPropertyChanged(nameof(ToDoTasksSubView));
            }
        }
        private object? _usersSubView = null;
        public object? UsersSubView
        {
            get
            {
                return _usersSubView;
            }
            set
            {
                _usersSubView = value;
                OnPropertyChanged(nameof(UsersSubView));
            }
        }



        private static MainWindowViewModel? _instance = null;
        public static MainWindowViewModel? Instance()
        {
            return _instance;
        }

        public MainWindowViewModel(ToDoListContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            if (_instance is null)
            {
                _instance = this;
            }
            ToDoTasksSubView = new ToDoTasksViewModel(_context, _dialogService);
        
            UsersSubView = new UsersViewModel(_context, _dialogService);
        }
    }
}
