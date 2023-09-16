using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ToDoList.Models;
using ToDoList.Data;
using ToDoList.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.ViewModels
{
    public class ToDoTasksViewModel : ViewModelBase
    {
        private readonly ToDoListContext _context;
        private readonly IDialogService _dialogService;

        private bool? _dialogResult = null;
        public bool? DialogResult
        {
            get
            {
                return _dialogResult;
            }
            set
            {
                _dialogResult = value;
            }
        }

        private ObservableCollection<ToDoTask>? _toDoTasks = null;
        public ObservableCollection<ToDoTask>? ToDoTasks
        {
            get
            {
                if (_toDoTasks is null)
                {
                    _toDoTasks = new ObservableCollection<ToDoTask>();
                    return _toDoTasks;
                }
                return _toDoTasks;
            }
            set
            {
                _toDoTasks = value;
                OnPropertyChanged(nameof(ToDoTasks));
            }
        }
        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewTask);
                }
                return _add;
            }
        }

        private void AddNewTask(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ToDoTasksSubView = new AddToDoTaskViewModel(_context, _dialogService);

            }
        }
        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditBook);
                }
                return _edit;
            }
        }

        private void EditBook(object? obj)
        {
            if (obj is not null)
            {
                long taskId = (long)obj;
                EditToDoTaskViewModel editToDoTaskViewModel = new EditToDoTaskViewModel(_context, _dialogService)
                {
                    TaskId = taskId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.ToDoTasksSubView = editToDoTaskViewModel;
                }
            }
        }
        private ICommand? _remove = null;
        public ICommand? Remove
        {
            get
            {
                if (_remove is null)
                {
                    _remove = new RelayCommand<object>(RemoveTask);
                }
                return _remove;
            }
        }

        private void RemoveTask(object? obj)
        {
            if (obj is not null)
            {
                long taskId = (long)obj;
                ToDoTask? task = _context.ToDoTasks.Find(taskId);
                if (task is not null)
                {
                    DialogResult = _dialogService.Show(task.Title);
                    if (DialogResult == false)
                    {
                        return;
                    }

                    _context.ToDoTasks.Remove(task);
                    _context.SaveChanges();
                }
            }
        }
        public ToDoTasksViewModel(ToDoListContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.ToDoTasks.Load();
            ToDoTasks = _context.ToDoTasks.Local.ToObservableCollection();
        }
    }

}
