using SSMS.Core.Entities;

namespace SSMS.Core.Interfaces;

/// <summary>
/// Unit of Work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<AppUser> Users { get; }
    IRepository<Unit> Units { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
