using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoList.Data;
using ToDoList.Interfaces;
using ToDoList.Models;

namespace ToDoList.ViewModels
{
    public class AddToDoTaskViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ToDoListContext _context;
        private readonly IDialogService _dialogService;
        private ToDoTask _toDoTask = new ToDoTask();

        public string Error
        {
            get { return string.Empty; }
        }
        public string this[string columnName]
        {
            get
            {
                if (columnName == "Title")
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return "Title is Required";
                    }
                }
                if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return "Description is Required";
                    }
                }
                if (columnName == "StartDate")
                {
                    if (StartDate is null)
                    {
                        return "Start Date is Required";
                    }
                }
                if (columnName == "EndDate")
                {
                    if (EndDate is null)
                    {
                        return "End Date is Required";
                    }
                }
                if (columnName == "Priority")
                {
                    if (!int.TryParse(Priority.ToString(), out int priorityValue) || priorityValue <= -1)
                    {
                        return "Enter a valid positive number for Priority";
                    }
                }

                return string.Empty;
            }
        }
        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private string _description = string.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        private DateTime? _startDate = null;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }
        private DateTime? _endDate = null;
        public DateTime? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }
        private int _priority;
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }
        private List<string> _availableStatuses = new List<string> { "To Do", "Done"};
        public List<string> AvailableStatuses
        {
            get { return _availableStatuses; }
            set
            {
                _availableStatuses = value;
                OnPropertyChanged(nameof(AvailableStatuses));
            }
        }
        private string _selectedStatus;
        public string SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));

            }
        }
        private string _response = string.Empty;
        public string Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                OnPropertyChanged(nameof(Response));
            }
        }
        private ICommand? _back = null;
        public ICommand? Back
        {
            get
            {
                if (_back is null)
                {
                    _back = new RelayCommand<object>(NavigateBack);
                }
                return _back;
            }
        }

        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ToDoTasksSubView = new ToDoTasksViewModel(_context, _dialogService);
            }
        }
        private ICommand? _save = null;
        public ICommand? Save
        {
            get
            {
                if (_save is null)
                {
                    _save = new RelayCommand<object>(SaveData);
                }
                return _save;
            }
        }
        private User? _selectedUser;
        public User? SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));


                if (_selectedUser != null)
                {
                    _toDoTask.UserId = null;
                }
                else
                {
                    _toDoTask.UserId = _selectedUser?.UserId;
                }
            }
        }
        private ObservableCollection<User>? _availableUsers = null;
        public ObservableCollection<User> AvailableUsers
        {
            get
            {
                if (_availableUsers is null)
                {
                    _availableUsers = LoadUsers();
                    return _availableUsers;
                }
                return _availableUsers;
            }
            set
            {
                _availableUsers = value;
                OnPropertyChanged(nameof(AvailableUsers));
            }
        }
        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            ToDoTask task = new ToDoTask
            {
                Title = this.Title,
                Description = this.Description,
                StartDate = this.StartDate,
                EndDate = this.EndDate,
                Priority = this.Priority,
                Status = this.SelectedStatus,
                UserId = this.SelectedUser?.UserId
            };

            _context.ToDoTasks.Add(task);
            _context.SaveChanges();

            Response = "Data Saved";
        }
        public AddToDoTaskViewModel(ToDoListContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }
        private ObservableCollection<User> LoadUsers()
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            return _context.Users.Local.ToObservableCollection();
        }
        private bool IsValid()
        {
            string[] properties = { "Title", "Description", "StartDate", "EndDate", "Priority" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
