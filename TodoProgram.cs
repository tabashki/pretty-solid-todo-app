
/// <summary>
/// The main entry point for the application
/// </summary>
public class TodoProgram
{
    /// <summary>
    /// The main method
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        // Configure services
        var dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "todos.json");
        var serviceProvider = SolidTodo.ServiceProvider.ConfigureServices(dataFilePath);

        // Get the user interface
        var userInterface = serviceProvider.GetService<SolidTodo.IUserInterface>();

        // Show the main menu
        userInterface.ShowMainMenu();
    }
}
