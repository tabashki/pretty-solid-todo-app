
namespace SolidTodo
{
    /// <summary>
    /// Defines the contract for todo item repositories
    /// Following the Interface Segregation Principle by keeping the interface focused on todo operations
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// The amount of todo items currently contained
        /// </summary>
        public int ItemCount { get; }

        /// <summary>
        /// Gets all todo items
        /// </summary>
        /// <returns>An enumerable of todo items</returns>
        IEnumerable<TodoItem> GetAll();

        /// <summary>
        /// Gets a todo item by its Guid
        /// </summary>
        /// <param name="id">The Guid of the todo item to get</param>
        /// <returns>The todo item if found, null otherwise</returns>
        TodoItem? GetById(Guid id);

        /// <summary>
        /// Gets a todo item by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The todo item if found, null otherwise</returns>
        TodoItem? GetByIndex(int index);

        /// <summary>
        /// Returns the index of a contained todo item
        /// </summary>
        int GetIndexForItem(TodoItem item);

        /// <summary>
        /// Adds a new todo item
        /// </summary>
        /// <param name="item">The todo item to add</param>
        void Add(TodoItem item);

        /// <summary>
        /// Updates an existing todo item
        /// </summary>
        void Update(TodoItem item);

        /// <summary>
        /// Deletes a todo item by its Guid
        /// </summary>
        void Delete(Guid id);

        /// <summary>
        /// Deletes a todo item by its index
        /// </summary>
        void DeleteByIndex(int index);

        /// <summary>
        /// Saves all changes to the storage
        /// </summary>
        void SaveChanges();
    }
}
