using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using TaskManager.Model;
using TaskManager.Utils;

namespace TaskManager.ViewModels;
public class TaskManagerVM : INotifyPropertyChanged
{
    public ICollectionView TasksView { get; }
    public ObservableCollection<BusinessTask> Tasks { get; }

    public ObservableCollection<ValueDescription> Statuses { get; }

    private string _searchText;
    public string SearchText { get => _searchText; set => SetValue(ref _searchText, value); }

    private ValueDescription _statusFilter;
    public ValueDescription StatusFilter { get => _statusFilter; set => SetValue(ref _statusFilter, value); }



    private RelayCommand _addCommand;
    public RelayCommand AddCommand => _addCommand ??= new(q => MessageBox.Show(q.ToString()), q => true);
    
    private RelayCommand _deleteCommand;
    public RelayCommand DeleteCommand => _deleteCommand ??= new(q => Tasks.Remove(q as BusinessTask), q => Tasks?.Any() == true);

    private RelayCommand _updateCommand;

    public RelayCommand UpdateCommand => _updateCommand ??= new(q => MessageBox.Show(q?.ToString()), q => Tasks.Any() == true);

    public TaskManagerVM()
    {
        Tasks = new()
        {
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть \n с очень длинным текстом. Заодно проверим враппинг, вдруг чего не то", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active }
        };

        Statuses = new(EnumHelper
            .GetAllValuesAndDescriptions(typeof(Status))
            .Prepend(new() { Value = null, Description = "Все"}));

        TasksView = CollectionViewSource.GetDefaultView(Tasks);
        TasksView.Filter = q => Filter(q as BusinessTask);

    }

    private bool Filter(BusinessTask task)
    {
        var isStatusMatched = StatusFilter is null || StatusFilter.Value is null || StatusFilter.Value.Equals(task.Status);
        var isSearchTextPresent = string.IsNullOrWhiteSpace(SearchText) || task.Title.Contains(SearchText) || task.Description?.Contains(SearchText) == true;

        return isStatusMatched && isSearchTextPresent;
    }

    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        field = value;
        PropertyChanged?.Invoke(this, new(propertyName));
        TasksView?.Refresh();
    }


    public event PropertyChangedEventHandler? PropertyChanged;

}
