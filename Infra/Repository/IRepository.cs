using Infra.Domain.Entities;

namespace Infra.Repository
{
    public interface IRepository<T> where T : class, IEntity
    {
        IEnumerable<T> Filter(Func<T, bool> expression);
        IEnumerable<T> All();
        IEntity ById(string identifier);
        IEntity ById(long id);
        IEnumerable<T> Upsert(params T[] entities);
        void Remove(params T[] entities);
    }
}
