namespace Todo.Domain;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    private TodoItem() {}

    public static TodoItem Create(string title, DateTime createdAt)
    {
        return new TodoItem()
        {
            Id = Guid.NewGuid(),
            Title = title,
            IsCompleted = false,
            CreatedAt = createdAt
        };
    }
}
