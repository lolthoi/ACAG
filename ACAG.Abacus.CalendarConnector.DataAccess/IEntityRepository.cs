using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ACAG.Abacus.CalendarConnector.DataAccess
{
    public interface IEntityRepository<T> where T : class, new()
    {
        DbSet<T> Entities { get; }
        IQueryable<T> Query();

        IQueryable<T> Query(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Edit(T entity);

        void Delete(T entity);

        void UpdateRange(List<T> entities);

        void InsertRange(List<T> entities, int batchSize = 100);

        void DeleteRange(List<T> entities);

    }
}
