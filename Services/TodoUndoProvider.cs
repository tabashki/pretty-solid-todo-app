
namespace SolidTodo
{
    class TodoUndoProvider
    {
        private ITodoRepository todoRepository;

        // NOTE: There's no enforced upper limit on the size of the history stack here,
        //       adding one would require switching to `LinkedList<T>` or another collection
        //       since `Stack<T>` doesn't support removing elements from the bottom.
        private Stack<TodoSnapshot> undoHistory = new();

        public bool HasUndoSteps => undoHistory.Count > 0;

        /// <summary>
        /// Undo the last operation by restoring the previous state of todo items
        /// </summary>
        public bool UndoLastChange(out DateTime prevStateTime)
        {
            if (undoHistory.TryPop(out TodoSnapshot snapshot))
            {
                prevStateTime = snapshot.CreatedAt;
                todoRepository.RestoreSnapshot(snapshot);
                return true;
            }
            prevStateTime = new DateTime();
            return false;
        }

        /// <summary>
        /// Record a state snapshot, call this before performing a modification
        /// </summary>
        public void RecordUndoState()
        {
            var snapshot = todoRepository.CreateSnapshot();
            undoHistory.Push(snapshot);
        }

        /// <summary>
        /// Construct an undo provider
        /// </summary>
        /// <param name="todoRepo"></param>
        public TodoUndoProvider(ITodoRepository todoRepo)
        {
            todoRepository = todoRepo;
            undoHistory = new Stack<TodoSnapshot>();
        }
    }
}