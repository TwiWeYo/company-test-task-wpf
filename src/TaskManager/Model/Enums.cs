using System.ComponentModel;

namespace TaskManager.Model;

public enum Status
{
    [Description("Активно")]
    Active,
    [Description("Выполнено")]
    Completed
}

public enum Priority
{
    [Description("Низкий")]
    Low,
    [Description("Средний")]
    Medium,
    [Description("Высокий")]
    High
}
