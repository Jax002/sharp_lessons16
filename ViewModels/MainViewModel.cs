using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace sharp_lessons16
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Task> _allTasks;
        private ObservableCollection<Task> _filteredTasks;
        private string _searchText;
        private string _selectedFilter;
        private string _newTaskName;
        private string _newTaskDescription;
        private DateTime _newTaskDeadline;
        private bool _newTaskIsImportant;

        public ObservableCollection<Task> FilteredTasks
        {
            get => _filteredTasks;
            set { _filteredTasks = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterTasks();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged();
                    FilterTasks();
                }
            }
        }

        public string NewTaskName
        {
            get => _newTaskName;
            set { _newTaskName = value; OnPropertyChanged(); ((RelayCommand)AddTaskCommand).RaiseCanExecuteChanged(); }
        }

        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set { _newTaskDescription = value; OnPropertyChanged(); }
        }

        public DateTime NewTaskDeadline
        {
            get => _newTaskDeadline;
            set { _newTaskDeadline = value; OnPropertyChanged(); }
        }

        public bool NewTaskIsImportant
        {
            get => _newTaskIsImportant;
            set { _newTaskIsImportant = value; OnPropertyChanged(); }
        }

        public ICommand AddTaskCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        public ICommand ClearFiltersCommand { get; private set; }

        public MainViewModel()
        {
            _allTasks = new ObservableCollection<Task>();
            _filteredTasks = new ObservableCollection<Task>();
            _selectedFilter = "Всі";
            _newTaskDeadline = DateTime.Now.AddDays(1);

            AddTaskCommand = new RelayCommand(AddTask, CanAddTask);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            ClearFiltersCommand = new RelayCommand(ClearFilters);

            _allTasks.Add(new Task { Id = 1, Name = "Тест важливе", Description = "Опис", Deadline = DateTime.Now.AddDays(1), IsImportant = true, IsCompleted = false });
            _allTasks.Add(new Task { Id = 2, Name = "Тест виконане", Description = "Опис", Deadline = DateTime.Now.AddDays(2), IsImportant = false, IsCompleted = true });
            _allTasks.Add(new Task { Id = 3, Name = "Тест активне", Description = "Опис", Deadline = DateTime.Now.AddDays(3), IsImportant = false, IsCompleted = false });

            FilterTasks();
        }

        private void AddTask(object parameter)
        {
            if (string.IsNullOrWhiteSpace(NewTaskName)) return;
            _allTasks.Add(new Task
            {
                Id = _allTasks.Count + 1,
                Name = NewTaskName,
                Description = string.IsNullOrWhiteSpace(NewTaskDescription) ? "No description" : NewTaskDescription,
                Deadline = NewTaskDeadline,
                IsImportant = NewTaskIsImportant,
                IsCompleted = false
            });
            NewTaskName = string.Empty;
            NewTaskDescription = string.Empty;
            NewTaskDeadline = DateTime.Now.AddDays(1);
            NewTaskIsImportant = false;
            FilterTasks();
        }

        private bool CanAddTask(object parameter) => !string.IsNullOrWhiteSpace(NewTaskName);

        private void DeleteTask(object parameter)
        {
            if (parameter is Task task)
            {
                if (MessageBox.Show($"Видалити '{task.Name}'?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _allTasks.Remove(task);
                    FilterTasks();
                }
            }
        }

        private void ClearFilters(object parameter)
        {
            SearchText = string.Empty;
            SelectedFilter = "Всі";
        }

        private void FilterTasks()
        {
            if (_allTasks == null) return;

            var filtered = _allTasks.AsEnumerable();

            switch (SelectedFilter)
            {
                case "Виконані":
                    filtered = filtered.Where(t => t.IsCompleted == true);
                    break;
                case "Активні":
                    filtered = filtered.Where(t => t.IsCompleted == false);
                    break;
                case "Важливі":
                    filtered = filtered.Where(t => t.IsImportant == true);
                    break;
                case "Просрочені":
                    filtered = filtered.Where(t => t.IsOverdue == true);
                    break;
                default: 
                    break;
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(t => t.Name.ToLower().Contains(SearchText.ToLower()));
            }

            FilteredTasks.Clear();
            foreach (var task in filtered.OrderBy(t => t.Deadline))
            {
                FilteredTasks.Add(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}