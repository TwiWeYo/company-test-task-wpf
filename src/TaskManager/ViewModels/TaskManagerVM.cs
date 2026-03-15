using System.Collections.ObjectModel;
using System.Windows;
using TaskManager.Model;
using TaskManager.Utils;

namespace TaskManager.ViewModels;
public class TaskManagerVM
{
    public ObservableCollection<BusinessTask> Tasks { get; }
    public readonly IEnumerable<string> Statuses = Enum.GetNames<Status>();

    private RelayCommand _addCommand;
    public RelayCommand AddCommand => _addCommand ??= new(q => MessageBox.Show(q.ToString()), q => true);
    
    private RelayCommand _removeCommand;
    public RelayCommand RemoveCommand => _removeCommand ??= new(q => Tasks.Remove(q as BusinessTask), q => Tasks?.Any() == true);

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
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active },
            new BusinessTask { Title = "Тест", Description = "Посмотреть как оно будет выглядеть", DueDate = DateTime.Now, Priority = Priority.High, Status = Status.Active }
        };
    }

}
