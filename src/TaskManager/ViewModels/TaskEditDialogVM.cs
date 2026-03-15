using TaskManager.Model;

namespace TaskManager.ViewModels;

public class TaskEditDialogVM : ViewModel
{
    private BusinessTask _businessTask;
    public BusinessTask BusinessTask { get => _businessTask; set => SetValue(ref _businessTask, value); }
}
