namespace TaskManager.ViewModels;

public class ConfirmDialogVM : ViewModel
{
    private string _message;
    public string Message { get => _message; set => SetValue(ref _message, value); }
}
