
namespace SolidTodo
{
    public class TodoSnapshot
    {
        private List<TodoItem> todoItems;

        /// <summary>
        /// Performs a deep copy of a TodoItem list
        /// </summary>
        public static List<TodoItem> CloneItemList(IReadOnlyList<TodoItem> items)
        {
            return items.Select(item => new TodoItem(
                item.Id, item.IsCompleted, item.Title, item.Priority,
                item.CreatedAt, item.LastModified, item.DueDate)
            ).ToList();
        }

        public DateTime CreatedAt { get; private set; }
        public List<TodoItem> TodoItems => CloneItemList(todoItems);

        /// <summary>
        /// Construct a snapshot from a list of TodoItems, performs a deep copy to ensure
        /// that any modification to the source items does not influence the snapshot
        /// </summary>
        public TodoSnapshot(IReadOnlyList<TodoItem> todoItems)
        {
            this.todoItems = CloneItemList(todoItems);
            CreatedAt = DateTime.Now;
        }
    }
}