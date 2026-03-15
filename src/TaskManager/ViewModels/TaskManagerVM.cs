using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Model;
using TaskManager.Utils;

namespace TaskManager.ViewModels;
public class TaskManagerVM : ViewModel
{
    private const string DialogHostId = "RootDialogHostId";


    private ObservableCollection<BusinessTask> _tasks;
    public ICollectionView TasksView { get; }

    public ObservableCollection<ValueDescription> Statuses { get; }

    private string _searchText;
    public string SearchText 
    { 
        get => _searchText;
        set 
        {
            SetValue(ref _searchText, value);
            TasksView?.Refresh();
        } 
    }

    private ValueDescription _statusFilter;
    public ValueDescription StatusFilter
    {
        get => _statusFilter;
        set
        {
            SetValue(ref _statusFilter, value);
            TasksView?.Refresh();
        }
    }

    private SortType _sortType;
    public SortType SortType
    {
        get => _sortType;
        set
        {
            SetValue(ref _sortType, value);
            UpdateSortDescription();
            TasksView?.Refresh();
        }
    }

    public ICommand CreateCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }

    public TaskManagerVM()
    {
        _tasks = new()
        {
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Completed},
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.Medium, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.Low, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть \n с очень длинным текстом. Заодно проверим враппинг, вдруг чего не то", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now.AddDays(-5), Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now.AddDays(5), Priority = Priority.High, Status = Status.Active }
        };

        Statuses = new(EnumHelper
            .GetAllValuesAndDescriptions(typeof(Status))
            .Prepend(new() { Value = null, Description = "Все"}));

        TasksView = CollectionViewSource.GetDefaultView(_tasks);
        TasksView.Filter = q => Filter((BusinessTask)q);


        CreateCommand = new RelayCommand(OnCreateCommand, _ => true);
        UpdateCommand = new RelayCommand(OnUpdateCommand, _ => true);
        DeleteCommand = new RelayCommand(OnDeleteCommand, _ => true);
    }

    private async void OnCreateCommand(object? _)
    {
        var newTask = new BusinessTask();
        var vm = new TaskEditDialogVM() { BusinessTask = newTask };
        var result = await DialogHost.Show(vm, DialogHostId);

        if (result?.ToString() == "Confirm")
        {
            _tasks.Add(newTask);
            TasksView?.Refresh();
        }
    }

    private async void OnUpdateCommand(object? taskObj)
    {
        if (taskObj is not BusinessTask task)
            return;

        var newTask = new BusinessTask()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };

        var vm = new TaskEditDialogVM() { BusinessTask = newTask };
        var result = await DialogHost.Show(vm, DialogHostId);

        if (result?.ToString() == "Confirm")
        {
            task.Title = newTask.Title;
            task.Description = newTask.Description;
            task.Status = newTask.Status;
            task.Priority = newTask.Priority;
            task.DueDate = newTask.DueDate;

            TasksView?.Refresh();
        }
    }

    private async void OnDeleteCommand(object? taskObj)
    {
        if (taskObj is not BusinessTask task)
            return;

        var vm = new ConfirmDialogVM() { Message = "Вы уверены, что хотите удалить эту задачу?" };
        var result = await DialogHost.Show(vm, DialogHostId);

        if (result?.ToString() == "Confirm")
        {
            _tasks.Remove(task);
            TasksView?.Refresh();
        }
    }

    private void UpdateSortDescription()
    {
        TasksView.SortDescriptions.Clear();

        if (SortType == SortType.None)
        {
            return;
        }

        SortDescription sortDescription = SortType switch
        {
            SortType.ByCreatedAt => new(nameof(BusinessTask.CreatedAt), ListSortDirection.Ascending),
            SortType.ByPriority => new(nameof(BusinessTask.Priority), ListSortDirection.Descending),
            SortType.ByDueDate => new(nameof(BusinessTask.DueDate), ListSortDirection.Ascending),
            _ => throw new NotImplementedException()
        };


        TasksView.SortDescriptions.Add(sortDescription);
    }

    private bool Filter(BusinessTask task)
    {
        var isStatusMatched = StatusFilter is null || StatusFilter.Value is null || StatusFilter.Value.Equals(task.Status);
        var isSearchTextPresent = string.IsNullOrWhiteSpace(SearchText) || task.Title.Contains(SearchText) || task.Description?.Contains(SearchText) == true;

        return isStatusMatched && isSearchTextPresent;
    }

}
