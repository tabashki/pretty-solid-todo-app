
namespace SolidTodo
{
    /// <summary>
    /// Defines the contract for user interfaces
    /// Following the Interface Segregation Principle by keeping the interface focused on UI operations
    /// </summary>
    public interface IUserInterface
    {
        /// <summary>
        /// Displays the main menu and handles user input
        /// </summary>
        void ShowMainMenu();

        /// <summary>
        /// Displays a list of todo items
        /// </summary>
        /// <param name="items">The collection of todo items to display</param>
        void DisplayTodoItems(IEnumerable<TodoItem> items);

        /// <summary>
        /// Displays a single todo item
        /// </summary>
        /// <param name="item">The todo item to display</param>
        void DisplayTodoItem(TodoItem item);

        /// <summary>
        /// Gets input from the user to create a new todo item
        /// </summary>
        /// <returns>A new todo item based on user input</returns>
        TodoItem CreateNewTodoItem();

        /// <summary>
        /// Gets input from the user to update an existing todo item
        /// </summary>
        /// <param name="item">The todo item to update</param>
        /// <returns>The updated todo item based on user input</returns>
        TodoItem UpdateTodoItem(TodoItem item);

        /// <summary>
        /// Displays an informational message to the user
        /// </summary>
        /// <param name="message">The message to display</param>
        void DisplayMessage(string message);

        /// <summary>
        /// Displays an error message to the user
        /// </summary>
        /// <param name="message">The error message to display</param>
        void DisplayError(string message);
    }
}