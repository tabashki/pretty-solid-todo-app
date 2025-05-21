
namespace SolidTodo
{
    /// <summary>
    /// Implements the ITodoRepository interface to manage todo items
    /// Following the Single Responsibility Principle by focusing only on todo item management
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly IStorageProvider storageProvider;
        private List<TodoItem> todoItems;
        private bool isLoaded;
        private bool hasUnsavedChanges = false;

        /// <summary>
        /// Ensures that todo items are loaded from the storage provider
        /// </summary>
        private void EnsureLoaded()
        {
            if (!isLoaded)
            {
                todoItems = storageProvider.Load().ToList();
                SortByPriority();
                isLoaded = true;
            }
        }

        public int ItemCount
        {
            get
            {
                EnsureLoaded();
                return todoItems.Count;
            }
        }

        /// <summary>
        /// Sorts the todo items by priority in descending order (Urgent first, Low last)
        /// </summary>
        private void SortByPriority()
        {
            todoItems = todoItems.OrderByDescending(item => item.Priority).ToList();
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < todoItems.Count;
        }

        public IEnumerable<TodoItem> GetAll()
        {
            EnsureLoaded();

            return todoItems;
        }

        public TodoItem? GetById(Guid id)
        {
            EnsureLoaded();

            return todoItems.FirstOrDefault(item => item.Id == id);
        }

        public TodoItem? GetByIndex(int index)
        {
            EnsureLoaded();

            if (!IsValidIndex(index))
            {
                return null;
            }
            return todoItems[index];
        }

        public int GetIndexForItem(TodoItem item)
        {
            EnsureLoaded();

            int index = todoItems.FindIndex(it => it == item);
            if (index < 0)
            {
                throw new ArgumentException("Item not contained in repository");
            }
            return index;
        }

        public void Add(TodoItem item)
        {
            EnsureLoaded();

            // Ensure the item has a unique ID
            if (todoItems.Any(i => i.Id == item.Id))
            {
                throw new ArgumentException("Cannot add todo item with duplicate ID");
            }

            todoItems.Add(item);
            SortByPriority();
            hasUnsavedChanges = true;
        }

        public void Update(TodoItem item)
        {
            EnsureLoaded();

            var existingItem = GetById(item.Id);
            if (existingItem == null)
            {
                throw new ArgumentException($"Item with id {item.Id} not found in repository");
            }
            if (existingItem != item)
            {
                // Update from a copy of the modified item
                existingItem.UpdateFrom(item);
            }

            SortByPriority();
            hasUnsavedChanges = true;
        }

        public void Delete(Guid id)
        {
            EnsureLoaded();

            var item = todoItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                todoItems.Remove(item);
            }

            hasUnsavedChanges = true;
        }

        public void DeleteByIndex(int index)
        {
            EnsureLoaded();

            if (!IsValidIndex(index))
            {
                throw new ArgumentException("Invalid index to delete by");
            }
            todoItems.RemoveAt(index);
            hasUnsavedChanges = true;
        }

        public void SaveChanges()
        {
            if (hasUnsavedChanges)
            {
                storageProvider.Save(todoItems);
            }
        }

        public TodoSnapshot CreateSnapshot()
        {
            return new TodoSnapshot(todoItems);
        }

        public void RestoreSnapshot(TodoSnapshot snapshot)
        {
            todoItems = snapshot.TodoItems;
            hasUnsavedChanges = true;
        }

        /// <summary>
        /// Initializes a new instance of the repository class with a given
        /// storage provider implementation
        /// </summary>
        public TodoRepository(IStorageProvider provider)
        {
            storageProvider = provider;
            todoItems = new List<TodoItem>();
            isLoaded = false;
        }
    }
}
