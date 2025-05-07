
namespace SolidTodo
{
    /// <summary>
    /// Builder class for creating TodoItem instances
    /// </summary>
    public class TodoItemBuilder
    {
        public Guid Id { get; private set; }
        public bool IsCompleted { get; private set; }
        public string Title { get; private set; }
        public Priority Priority { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ModifiedAt { get; private set; }
        public DateTime? DueDate { get; private set; }

        public TodoItemBuilder()
        {
            Id = Guid.NewGuid();
            IsCompleted = false;
            Title = string.Empty;
            Priority = Priority.Normal;
            CreatedAt = DateTime.Now;
            ModifiedAt = CreatedAt;
            DueDate = null;
        }

        public TodoItemBuilder WithId(Guid id)
        {
            Id = id;
            return this;
        }

        public TodoItemBuilder WithCompletion(bool isCompleted)
        {
            IsCompleted = isCompleted;
            return this;
        }

        public TodoItemBuilder WithTitle(string title)
        {
            Title = title;
            return this;
        }

        public TodoItemBuilder WithPriority(Priority priority)
        {
            Priority = priority;
            return this;
        }

        public TodoItemBuilder WithCreationDate(DateTime createdAt)
        {
            CreatedAt = createdAt;
            return this;
        }

        public TodoItemBuilder WithModificationDate(DateTime modifiedAt)
        {
            ModifiedAt = modifiedAt;
            return this;
        }

        public TodoItemBuilder WithDueDate(DateTime? dueDate)
        {
            DueDate = dueDate;
            return this;
        }

        public TodoItem Build()
        {
            return new TodoItem(Id, IsCompleted, Title, Priority, CreatedAt,
                ModifiedAt, DueDate);
        }
    }
}