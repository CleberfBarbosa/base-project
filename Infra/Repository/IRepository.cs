using Infra.Domain.Entities;

namespace Infra.Repository
{
    public interface IRepository
    {
        IEnumerable<T> Filter<T>(Func<T, bool> expression) 
            where T : class, IEntity;
        IEnumerable<T> All<T>()
            where T : class, IEntity;
        IEntity ById<T>(string identifier)
            where T : class, IEntity;
        IEntity ById<T>(long id)
            where T : class, IEntity;
        IEnumerable<T> Upsert<T>(params T[] entities)
            where T : class, IEntity;
        void Remove<T>(params T[] entities)
            where T : class, IEntity;
    }
}
