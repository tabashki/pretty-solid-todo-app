
namespace SolidTodo
{
    /// <summary>
    /// Defines the contract for storage providers
    /// Following the Interface Segregation Principle by keeping the interface focused
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Loads all todo items from storage
        /// </summary>
        /// <returns>A collection of todo items</returns>
        IEnumerable<TodoItem> Load();

        /// <summary>
        /// Saves all todo items to storage
        /// </summary>
        /// <param name="items">The collection of todo items to save</param>
        void Save(IEnumerable<TodoItem> items);
    }
}
