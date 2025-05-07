
namespace SolidTodo
{
    /// <summary>
    /// A simple service provider for dependency injection
    /// Following the Dependency Inversion Principle by providing dependencies
    /// </summary>
    public class ServiceProvider
    {
        private readonly Dictionary<Type, object> services = new();

        /// <summary>
        /// Registers a service with the service provider
        /// </summary>
        /// <typeparam name="TInterface">The interface type</typeparam>
        /// <typeparam name="TImplementation">The implementation type</typeparam>
        /// <param name="instance">The service instance</param>
        public void Register<TInterface, TImplementation>(TImplementation instance)
            where TImplementation : class, TInterface
            where TInterface : class
        {
            services[typeof(TInterface)] = instance;
        }

        /// <summary>
        /// Gets a service from the service provider
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The service instance</returns>
        public T GetService<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }

        /// <summary>
        /// Configures the default services for the application
        /// </summary>
        /// <param name="dataFilePath">The path to the data file</param>
        /// <returns>The configured service provider</returns>
        public static ServiceProvider ConfigureServices(string dataFilePath)
        {
            var serviceProvider = new ServiceProvider();

            var storageProvider = new FileStorageProvider(dataFilePath);
            serviceProvider.Register<IStorageProvider, FileStorageProvider>(storageProvider);

            var todoRepository = new TodoRepository(storageProvider);
            serviceProvider.Register<ITodoRepository, TodoRepository>(todoRepository);

            var userInterface = new ConsoleUserInterface(todoRepository);
            serviceProvider.Register<IUserInterface, ConsoleUserInterface>(userInterface);

            return serviceProvider;
        }
    }
}
