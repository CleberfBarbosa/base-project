using Infra.Domain.Entities;
using Infra.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbContext dbContext;
        private readonly DbSet<T> dbSet;

        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = this.dbContext.Set<T>();
        }

        public IEnumerable<T> All()
        {
            return this.dbSet.AsEnumerable();
        }

        public IEntity ById(string identifier)
        {
            return this.dbSet.FirstOrDefault(f => f.Identifier.ToString() == identifier);
        }

        public IEntity ById(long id)
        {
            return this.dbSet.FirstOrDefault(f => f.Id == id);
        }

        public IEnumerable<T> Filter(Func<T, bool> expression)
        {
            return this.dbSet.Where(expression).AsEnumerable();
        }

        public void Remove(params T[] entities)
        {
            if (entities.IsNullOrEmpty())
                return;

            this.dbSet.RemoveRange(entities);
            dbContext.SaveChanges();
        }

        public IEnumerable<T> Upsert(params T[] entities)
        {
            if (entities.IsNullOrEmpty())
                return new List<T>();

            var updateList = entities.Where(n => n.Id > 0)?.ToArray() ?? Array.Empty<T>();
            var insertList = entities.Where(n => n.Id <= 0)?.ToArray() ?? Array.Empty<T>();
            this.dbSet.UpdateRange(updateList);
            this.dbSet.AddRange(insertList);
            dbContext.SaveChanges();
            return insertList.Concat(updateList);
        }
    }
}
