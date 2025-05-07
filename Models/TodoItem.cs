
using System.Text.Json.Serialization;

namespace SolidTodo
{
    /// <summary>
    /// Represents the priority level of a todo item
    /// </summary>
    public enum Priority
    {
        Low,
        Normal,
        High,
        Urgent
    }

    /// <summary>
    /// Represents a single todo item in the application
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Unique identifier for the todo item
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Indicates whether the todo item has been completed
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// The title or description of the todo item
        /// </summary>
        public string Title { get; private set; } = string.Empty;

        /// <summary>
        /// Priority level of the todo item
        /// </summary>
        public Priority Priority { get; private set; }

        /// <summary>
        /// The date and time when the todo item was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// The date and time when the todo item was last modified
        /// </summary>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Optional due date for the todo item
        /// </summary>
        public DateTime? DueDate { get; private set; }

        // Encourage use of the NewTodoItemBuilder
        private TodoItem() {}

        // Internal constructor for the builder and JSON deserialization
        [JsonConstructor]
        internal TodoItem(Guid id, bool isCompleted, string title, Priority priority,
            DateTime createdAt, DateTime lastModified, DateTime? dueDate)
        {
            Id = id;
            IsCompleted = isCompleted;
            Title = title;
            Priority = priority;
            CreatedAt = createdAt;
            LastModified = lastModified;
            DueDate = dueDate;
        }

        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException("Title cannot be empty");
            }
            Title = newTitle;
            LastModified = DateTime.Now;
        }

        public void UpdatePriority(Priority newPriority)
        {
            Priority = newPriority;
            LastModified = DateTime.Now;
        }

        public void UpdateCompleted(bool completed)
        {
            IsCompleted = completed;
            LastModified = DateTime.Now;
        }

        // Or pass null to remove due date
        public void UpdateDueDate(DateTime? newDueDaate)
        {
            DueDate = newDueDaate;
            LastModified = DateTime.Now;
        }

        public void UpdateFrom(TodoItem item)
        {
            Title = item.Title;
            IsCompleted = item.IsCompleted;
            DueDate = item.DueDate;
            Priority = item.Priority;
            LastModified = DateTime.Now;
        }
    }
}
