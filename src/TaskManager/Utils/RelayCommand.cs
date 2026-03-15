using System.Windows.Input;

namespace TaskManager.Utils;

public class RelayCommand(Action<object?> execute, Func<object?, bool> canExecute) : ICommand
{

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return canExecute?.Invoke(parameter) ?? false;
    }

    public void Execute(object? parameter)
    {
        execute?.Invoke(parameter);
    }
}
