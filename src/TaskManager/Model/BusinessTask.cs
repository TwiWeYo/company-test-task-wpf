namespace TaskManager.Model;

public class BusinessTask
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
