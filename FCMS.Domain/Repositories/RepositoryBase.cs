using Microsoft.Extensions.Logging;

namespace FCMS.Domain.Repositories
{
    /// <summary>
    /// Base repository class which exposes the ILogger to the inheriting repository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryBase<T>
    {
        protected ILogger<T> Logger { get; }
        public RepositoryBase(ILogger<T> logger)
        {
            this.Logger = logger;
        }
    }
}
