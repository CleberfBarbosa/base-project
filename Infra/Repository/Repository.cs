using Infra.Domain.Entities;
using Infra.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository
{
    public class Repository : IRepository
    {
        private readonly DbContext dbContext;

        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        IEnumerable<T> IRepository.All<T>()
        {
            return Table<T>().AsEnumerable();
        }

        IEntity IRepository.ById<T>(string identifier)
        {
            return Table<T>().FirstOrDefault(f => f.Identifier.ToString() == identifier);
        }

        IEntity IRepository.ById<T>(long id)
        {
            return Table<T>().FirstOrDefault(f => f.Id == id);
        }

        IEnumerable<T> IRepository.Filter<T>(Func<T, bool> expression)
        {
            return Table<T>().Where(expression).AsEnumerable();
        }

        void IRepository.Remove<T>(params T[] entities)
        {
            if (entities.IsNullOrEmpty())
                return;

            Table<T>().RemoveRange(entities);
            dbContext.SaveChanges();
        }

        IEnumerable<T> IRepository.Upsert<T>(params T[] entities)
        {
            if (entities.IsNullOrEmpty())
                return new List<T>();

            var updateList = entities.Where(n => n.Id > 0)?.ToArray() ?? Array.Empty<T>();
            var insertList = entities.Where(n => n.Id <= 0)?.ToArray() ?? Array.Empty<T>();
            Table<T>().UpdateRange(updateList);
            Table<T>().AddRange(insertList);
            dbContext.SaveChanges();
            return insertList.Concat(updateList);
        }

        private DbSet<T> Table<T>()
            where T : class, IEntity
        {
            return dbContext.Set<T>();
        }
    }
}
