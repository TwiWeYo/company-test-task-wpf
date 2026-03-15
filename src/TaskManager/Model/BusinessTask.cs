using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Model;

// Gosh i miss ReactiveUI
public class BusinessTask : INotifyPropertyChanged, IDataErrorInfo
{
    public Guid Id { get; init; } = Guid.NewGuid();

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

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string Error { get; }

    public string this[string columnName]
    {
        get
        {
            string? result = columnName switch
            {
                "Title" when string.IsNullOrWhiteSpace(Title) => $"Поле {nameof(Title)} не может быть пустым",
                "DueDate" when DueDate.HasValue && DueDate.Value < DateTime.UtcNow => $"{nameof(DueDate)} не может быть в прошлом",
                _ => null
            };

            return result;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        field = value;
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}
