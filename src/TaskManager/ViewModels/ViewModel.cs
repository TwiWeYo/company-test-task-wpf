using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.ViewModels;

public class ViewModel : INotifyPropertyChanged
{
    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        field = value;
        PropertyChanged?.Invoke(this, new(propertyName));
    }


    public event PropertyChangedEventHandler? PropertyChanged;
}
