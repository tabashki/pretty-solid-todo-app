using System.Text.Json;

namespace SolidTodo
{
    /// <summary>
    /// Implements the IStorageProvider interface to store todo items in a local JSON file
    /// Following the Single Responsibility Principle by focusing only on file storage
    /// </summary>
    public class FileStorageProvider : IStorageProvider
    {
        private readonly string filePath;
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Initializes a new instance of the FileStorageProvider class
        /// </summary>
        /// <param name="filePath">The path to the JSON file</param>
        public FileStorageProvider(string filePath)
        {
            this.filePath = filePath;

            // Create the directory if it doesn't exist
            var directory = Path.GetDirectoryName(this.filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Loads all todo items from the JSON file
        /// </summary>
        /// <returns>A collection of todo items</returns>
        public IEnumerable<TodoItem> Load()
        {
            if (!File.Exists(filePath))
            {
                return Enumerable.Empty<TodoItem>();
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var items = JsonSerializer.Deserialize<List<TodoItem>>(json, jsonOptions);
                return items ?? new List<TodoItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading todo items: {ex.Message}");
                return Enumerable.Empty<TodoItem>();
            }
        }

        /// <summary>
        /// Saves all todo items to the JSON file
        /// </summary>
        /// <param name="items">The collection of todo items to save</param>
        public void Save(IEnumerable<TodoItem> items)
        {
            try
            {
                var json = JsonSerializer.Serialize(items, jsonOptions);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving todo items: {ex.Message}");
            }
        }
    }
}
