using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace NikGame.Service
{
    public interface IUnitOfWork
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DatabaseFacade DataFace { get; }
        Task<int> SaveChangeAsync();
        int SaveChanges();
    }
}
