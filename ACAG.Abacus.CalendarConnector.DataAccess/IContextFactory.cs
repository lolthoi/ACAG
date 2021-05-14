using Microsoft.EntityFrameworkCore;

namespace ACAG.Abacus.CalendarConnector.DataAccess
{
    public interface IContextFactory
    {
        /// <summary>
        /// Gets the database context.
        /// </summary>
        /// <value>
        /// The database context.
        /// </value>
        DbContext GetDbContext<TContext>() where TContext : DbContext;
    }
}
