
namespace SolidTodo
{
    /// <summary>
    /// Implements the IUserInterface interface for console-based interaction
    /// Following the Single Responsibility Principle by focusing only on user interface concerns
    /// </summary>
    public class ConsoleUserInterface : IUserInterface
    {
        private ITodoRepository todoRepository;
        private TodoUndoProvider undoProvider;

#region Private helpers
        /// <summary>
        /// Truncates a string to a specified length
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maximum length</param>
        /// <returns>The truncated string</returns>
        private string TruncateString(string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
            {
                return str;
            }

            return str.Substring(0, maxLength - 3) + "...";
        }

        private string ConsoleReadString(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();
                if (!string.IsNullOrEmpty(s))
                {
                    return s;
                }
                Console.WriteLine("Please, try again");
            }
        }
        /// <summary>
        /// Select an option from a list of available options.
        /// NOTE: This function is CASE-INSENSITIVE
        /// </summary>
        /// <param name="prompt">Prompt for the user</param>
        /// <param name="options">Valid options to return</param>
        /// <returns>One of the selected options</returns>
        private string ConsoleSelect(string prompt, string[] options, string? defaultOpt = null)
        {
            var opts = string.Join('/', options);
            while (true)
            {
                if (defaultOpt != null)
                {
                    Console.Write($"{prompt} ({opts}) [{defaultOpt}]: ");
                }
                else
                {
                    Console.Write($"{prompt} ({opts}): ");
                }
                string? input = Console.ReadLine()?.ToLower();
                if (defaultOpt != null && string.IsNullOrWhiteSpace(input))
                {
                    return defaultOpt;
                }
                if (!string.IsNullOrEmpty(input) && options.Any(o => o.ToLower() == input))
                {
                    return input;
                }
                Console.WriteLine("Please, enter one of the listed options");
            }
        }

        private int ConsoleReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? s = Console.ReadLine();
                if (int.TryParse(s, out int i))
                {
                    return i;
                }
                Console.WriteLine("Please, try again");
            }
        }

        /// <summary>
        /// Writes a line to the console with the given ConsoleColor.
        /// Restores the previously set color before returning
        /// </summary>
        /// <param name=""></param>
        private void WriteLineWithColor(string line, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.WriteLine(line);
            Console.ForegroundColor = originalColor;
        }

        private bool GetConfirmation(string message)
        {
            var input = ConsoleSelect(message, ["y", "yes", "n", "no"], "n");
            return input == "y" || input == "yes";
        }

        private string ToFriendlyString(TimeSpan span)
        {
            string result = string.Empty;
            if (span.Days > 0)
            {
                result += $"{span.Days} day(s)";
            }
            if (span.Hours > 0)
            {
                result += $"{span.Hours} hour(s)";
            }
            if (span.Minutes > 0)
            {
                result += $"{span.Minutes} minute(s)";
            }
            if (span.Seconds > 0)
            {
                result += $"{span.Seconds} second(s)";
            }
            return result;
        }
#endregion

        #region Command handling
        private TodoItem? GetTodoItemFromUserInput()
        {
            var count = todoRepository.ItemCount;
            if (count < 1)
            {
                DisplayError("There are currently no items");
                return null;
            }
            if (count == 1)
            {
                return todoRepository.GetByIndex(0);
            }

            int displayId = ConsoleReadInt($"Todo item ID (1-{count}): ");
            int index = displayId - 1;
            return todoRepository.GetByIndex(index);
        }

        private void UpdateBuilderFromUserInput(TodoItemBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(builder.Title))
            {
                var title = ConsoleReadString("Title: ");
                builder.WithTitle(title);
            }
            else
            {
                Console.Write($"Title [{builder.Title}]: ");
                var newTitle = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newTitle))
                {
                    builder.WithTitle(newTitle);
                }
            }

            var priorityInput = ConsoleSelect("Priority",
                ["Low", "Normal", "High", "Urgent"],
                builder.Priority.ToString());
            if (Enum.TryParse<Priority>(priorityInput, true, out Priority priority))
            {
                builder.WithPriority(priority);
            }

            var currentDue = builder.DueDate.HasValue
                ? builder.DueDate.Value.ToString("dd/MM/yyyy") : "none";
            var dueDate = ConsoleReadString($"Due Date (dd/MM/yyyy or 'none' to clear) [{currentDue}]: ").ToLower();
            if (dueDate == "none")
            {
                builder.WithDueDate(null);
            }
            else if (DateTime.TryParse(dueDate, out var date))
            {
                builder.WithDueDate(date);
            }

            string completeInput = ConsoleSelect("Mark completed", ["y", "n"],
                builder.IsCompleted ? "y" : "n");
            builder.WithCompletion(completeInput == "y");
        }

        private void CommandViewAll()
        {
            var items = todoRepository.GetAll();
            DisplayTodoItems(items);
        }

        private void CommandAddNew()
        {
            Console.WriteLine("\n======== New Todo Item ========");

            undoProvider.RecordUndoState();

            var item = CreateNewTodoItem();
            todoRepository.Add(item);
            todoRepository.SaveChanges();

            DisplayMessage("Todo item added successfully.");
        }

        private void CommandViewDetails()
        {
            var item = GetTodoItemFromUserInput();
            if (item == null)
            {
                DisplayError("Todo item not found.");
                return;
            }

            DisplayTodoItem(item);
        }

        private void CommandUpdate()
        {
            var item = GetTodoItemFromUserInput();
            if (item == null)
            {
                DisplayError("Todo item not found.");
                return;
            }

            Console.WriteLine("\n======== Update Todo Item ========");

            undoProvider.RecordUndoState();

            item = UpdateTodoItem(item);
            todoRepository.Update(item);
            todoRepository.SaveChanges();

            DisplayMessage("Todo item updated successfully.");
        }

        private void CommandDelete()
        {
            var item = GetTodoItemFromUserInput();
            if (item == null)
            {
                DisplayError("Todo item not found.");
                return;
            }

            DisplayTodoItem(item);

            if (GetConfirmation("Are you sure you want to delete this todo item?"))
            {
                undoProvider.RecordUndoState();

                todoRepository.Delete(item.Id);
                todoRepository.SaveChanges();

                DisplayMessage("Todo item deleted successfully.");
            }
        }

        /// <summary>
        /// Marks a todo item as completed
        /// </summary>
        private void CommandComplete()
        {
            var item = GetTodoItemFromUserInput();
            if (item == null)
            {
                DisplayError("Todo item not found.");
                return;
            }

            undoProvider.RecordUndoState();

            item.UpdateCompleted(true);
            todoRepository.Update(item);
            todoRepository.SaveChanges();

            DisplayMessage("Todo item marked as completed.");
        }

        private void CommandUndo()
        {
            if (undoProvider.UndoLastChange(out DateTime prevStateTime))
            {
                todoRepository.SaveChanges();
                TimeSpan ago = DateTime.Now - prevStateTime;
                DisplayMessage($"Restored to state from {ToFriendlyString(ago)} ago.");
            }
            else
            {
                DisplayMessage("Already at the oldest change.");
            }
        }
#endregion

        #region IUserInterface impl
        public void ShowMainMenu()
        {
            bool exit = false;

            Console.WriteLine("===== Pretty Solid Todo List ========");
            Console.WriteLine("Available commands:");
            Console.WriteLine("    v - View all todos");
            Console.WriteLine("    a - Add new todo");
            Console.WriteLine("    i - View detailed todo info");
            Console.WriteLine("    u - Update todo");
            Console.WriteLine("    d - Delete todo");
            Console.WriteLine("    c - Mark todo as completed");
            Console.WriteLine("    z - Undo last action");
            Console.WriteLine("    x - Exit");

            while (!exit)
            {
                var choice = ConsoleSelect("Choose action", [
                    "v", "a", "i", "u", "d", "c", "z", "x",
                ]);

                switch (choice)
                {
                    case "v":
                        CommandViewAll();
                        break;
                    case "a":
                        CommandAddNew();
                        break;
                    case "i":
                        CommandViewDetails();
                        break;
                    case "u":
                        CommandUpdate();
                        break;
                    case "d":
                        CommandDelete();
                        break;
                    case "c":
                        CommandComplete();
                        break;
                    case "z":
                        CommandUndo();
                        break;
                    case "x":
                        exit = true;
                        break;
                }
            }

            Console.WriteLine("Goodbye!");
        }

        public void DisplayTodoItems(IEnumerable<TodoItem> items)
        {
            var todoItems = items.ToList();

            if (!todoItems.Any())
            {
                Console.WriteLine("No todo items found.");
                return;
            }

            Console.WriteLine("\n======== TODO ITEMS ========");
            Console.WriteLine($"Done | {"ID",-4} | {"Title",-30} | {"Priority",-8} | {"Due Date",-10}");
            Console.WriteLine(new string('-', 70));

            int displayId = 0;
            foreach (var item in todoItems)
            {
                displayId++;
                var checkbox = item.IsCompleted ? "[X]" : "[_]";
                var dueDate = item.DueDate.HasValue ? item.DueDate.Value.ToShortDateString() : "N/A";
                Console.WriteLine($"{checkbox,4} | {displayId,-4} | {TruncateString(item.Title, 30),-30} | {item.Priority,-8} | {dueDate,-10}");
            }
        }

        public void DisplayTodoItem(TodoItem item)
        {
            int index = todoRepository.GetIndexForItem(item);

            Console.WriteLine("\n======== Todo Item Details ========");
            Console.WriteLine($"Index: {index}");
            Console.WriteLine($"Internal GUID: {item.Id}");
            Console.WriteLine($"Title: {item.Title}");
            Console.WriteLine($"Status: {(item.IsCompleted ? "Completed" : "Pending")}");
            Console.WriteLine($"Priority: {item.Priority}");
            Console.WriteLine($"Created: {item.CreatedAt}");
            Console.WriteLine($"Last Modified: {item.LastModified}");

            if (item.DueDate.HasValue)
            {
                Console.WriteLine($"Due Date: {item.DueDate.Value.ToShortDateString()}");
            }
        }

        public TodoItem CreateNewTodoItem()
        {
            var builder = new TodoItemBuilder();

            var title = ConsoleReadString("Title: ");
            builder.WithTitle(title);

            var priorityInput = ConsoleSelect("Priority", ["Low", "Normal", "High", "Urgent"], "Normal");
            if (Enum.TryParse<Priority>(priorityInput, true, out Priority priority))
            {
                builder.WithPriority(priority);
            }

            Console.Write("Due Date (dd/MM/yyyy or leave empty): ");
            var dueDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dueDateInput) && DateTime.TryParse(dueDateInput, out var dueDate))
            {
                builder.WithDueDate(dueDate);
            }

            return builder.Build();
        }

        public TodoItem UpdateTodoItem(TodoItem item)
        {
            // Create a builder with the existing item's properties
            var builder = new TodoItemBuilder()
                .WithId(item.Id)
                .WithTitle(item.Title)
                .WithPriority(item.Priority)
                .WithCompletion(item.IsCompleted)
                .WithCreationDate(item.CreatedAt);

            UpdateBuilderFromUserInput(builder);
            builder.WithModificationDate(DateTime.Now);

            return builder.Build();
        }

        public void DisplayMessage(string message)
        {
            WriteLineWithColor(message, ConsoleColor.White);
        }

        public void DisplayError(string message)
        {
            WriteLineWithColor($"ERROR: {message}", ConsoleColor.Red);
        }

        #endregion

        public ConsoleUserInterface(ITodoRepository todoRepo)
        {
            todoRepository = todoRepo;
            undoProvider = new TodoUndoProvider(todoRepository);
        }
    }
}
