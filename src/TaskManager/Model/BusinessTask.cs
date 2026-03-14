using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Model;

// Gosh i miss ReactiveUI
public class BusinessTask : INotifyPropertyChanged
{
    public Guid Id { get; } = Guid.NewGuid();

    private string _title;
    public string Title { get => _title; set => SetValue(ref _title, value); }

    private string? _description;
    public string? Description { get => _description; set => SetValue(ref _description, value); }
    
    private Status _status;
    public Status Status { get => _status; set => SetValue(ref _status, value); }

    private Priority _priority;
    public Priority Priority { get => _priority; set => SetValue(ref _priority, value); }

    private DateTime? _dueDate;
    public DateTime? DueDate { get => _dueDate; set => SetValue(ref _dueDate, value); }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        field = value;
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
