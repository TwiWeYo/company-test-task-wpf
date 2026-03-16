using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Data;
using System.Windows.Input;
using TaskManager.Config;
using TaskManager.Model;
using TaskManager.Utils;

namespace TaskManager.ViewModels;
public class TaskManagerVM : ViewModel
{
    private const string DialogHostId = "RootDialogHostId";

    private readonly string _filePath;

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
    public ICommand LoadCommand { get; }
    public RelayCommand SaveCommand { get; }

    public TaskManagerVM()
    {
        _filePath = AppConfiguration.GetValue("FilePath") ?? string.Empty;

        Statuses = new(EnumHelper
            .GetAllValuesAndDescriptions(typeof(Status))
            .Prepend(new() { Value = null, Description = "Все" }));

        _tasks = new();
        TasksView = CollectionViewSource.GetDefaultView(_tasks);
        TasksView.Filter = q => Filter((BusinessTask)q);


        CreateCommand = new RelayCommand(OnCreateCommand, _ => true);
        UpdateCommand = new RelayCommand(OnUpdateCommand, _ => true);
        DeleteCommand = new RelayCommand(OnDeleteCommand, _ => true);

        LoadCommand = new RelayCommand(OnLoadCommand, _ => true);
        SaveCommand = new RelayCommand(OnSaveCommand, _ => _tasks?.Any() == true);
    }

    private async Task ShowDialog(ViewModel vm, Action onConfirm)
    {
        var result = await DialogHost.Show(vm, DialogHostId);

        if (result?.ToString() == "Confirm")
        {
            onConfirm?.Invoke();
            SaveCommand.RaiseCanExecuteChanged();
            TasksView?.Refresh();
        }
    }

    private async void OnCreateCommand(object? _)
    {
        var newTask = new BusinessTask();
        var vm = new TaskEditDialogVM() { BusinessTask = newTask };

        await ShowDialog(vm, () => _tasks.Add(newTask));
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
        await ShowDialog(vm, () =>
        {
            task.Title = newTask.Title;
            task.Description = newTask.Description;
            task.Status = newTask.Status;
            task.Priority = newTask.Priority;
            task.DueDate = newTask.DueDate;
        });
    }

    private async void OnDeleteCommand(object? taskObj)
    {
        if (taskObj is not BusinessTask task)
            return;

        var vm = new ConfirmDialogVM() { Message = "Вы уверены, что хотите удалить эту задачу?" };

        await ShowDialog(vm, () => _tasks.Remove(task));
    }

    private async void OnLoadCommand(object? _)
    {
        //TODO - создать отдельный диалог для ошибок или переписать существующий
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            await DialogHost.Show(new ConfirmDialogVM() { Message = "Путь файла не указан" }, DialogHostId);
            return;
        }

        if (!File.Exists(_filePath))
        {
            await DialogHost.Show(new ConfirmDialogVM() { Message = $"Файл {_filePath} не существует" }, DialogHostId);
            return;
        }

        var tasks = Enumerable.Empty<BusinessTask>();

        try
        {
            using var streamReader = new StreamReader(_filePath);
            tasks = await JsonSerializer.DeserializeAsync<IEnumerable<BusinessTask>>(streamReader.BaseStream);
        }
        catch (JsonException ex)
        {
            await DialogHost.Show(new ConfirmDialogVM() { Message = $"Ошибка чтения JSON \n {ex.Message}" }, DialogHostId);
            return;
        }

        foreach (var task in tasks)
        {
            _tasks.Add(task);
        }

        SaveCommand.RaiseCanExecuteChanged();
        TasksView?.Refresh();
    }

    private async void OnSaveCommand(object? _)
    {
        try
        {
            using var streamWriter = new StreamWriter(_filePath);
            await JsonSerializer.SerializeAsync(streamWriter.BaseStream, _tasks, new JsonSerializerOptions() { WriteIndented = true });
        }
        catch (Exception ex)
        {
            await DialogHost.Show(new ConfirmDialogVM() { Message = $"Ошибка сохранения файла: \n{ex.Message}" }, DialogHostId);
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
