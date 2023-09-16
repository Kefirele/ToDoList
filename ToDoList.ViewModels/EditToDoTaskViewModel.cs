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
    public class EditToDoTaskViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly ToDoListContext _context;
        private readonly IDialogService _dialogService;
        private ToDoTask? _toDoTask = new ToDoTask();
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
        private List<string> _availableStatuses = new List<string> { "To Do", "Done" };
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
        private long _taskId = 0;
        public long TaskId
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
                OnPropertyChanged(nameof(TaskId));
                LoadTaskData();
            }
        }
        private ICommand? _back = null;
        public ICommand Back
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
        private void NavigateBack(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ToDoTasksSubView = new ToDoTasksViewModel(_context, _dialogService);
            }
        }

        private ICommand? _save = null;
        public ICommand Save
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
        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            if (_toDoTask is null)
            {
                return;
            }
            _toDoTask.Title = Title;
            _toDoTask.Description = Description;
            _toDoTask.StartDate = StartDate;
            _toDoTask.EndDate = EndDate;
            _toDoTask.Priority = Priority;
            _toDoTask.User = SelectedUser;
            _toDoTask.Status = SelectedStatus;

            _context.Entry(_toDoTask).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Updated";
        }
        private ObservableCollection<User> LoadUsers()
        {
            _context.Database.EnsureCreated();
            _context.Users.Load();
            return _context.Users.Local.ToObservableCollection();
        }
        public EditToDoTaskViewModel(ToDoListContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
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
        private void LoadTaskData()
        {
            if (_context?.ToDoTasks is null)
            {
                return;
            }
            _toDoTask = _context.ToDoTasks.Find(TaskId);
            if (_toDoTask is null)
            {
                return;
            }
            this.Title = _toDoTask.Title;
            this.Description = _toDoTask.Description;
            this.StartDate = _toDoTask.StartDate;
            this.EndDate = _toDoTask.EndDate;
            this.SelectedStatus = _selectedStatus;
            if (_toDoTask.Priority.HasValue)
            {
                this.Priority = _toDoTask.Priority.Value;
            }
            else
            {
                this.Priority = 0;
            }
        }
    }
}
