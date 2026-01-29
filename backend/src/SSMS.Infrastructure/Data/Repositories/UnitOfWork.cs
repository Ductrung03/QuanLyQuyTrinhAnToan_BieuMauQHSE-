using Microsoft.EntityFrameworkCore.Storage;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Infrastructure.Data.Repositories;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = new();

    public IRepository<AppUser> Users { get; }
    public IRepository<Unit> Units { get; }
    public IRepository<OpsProcedure> Procedures { get; }
    public IRepository<OpsProcedureDocument> ProcedureDocuments { get; }
    public IRepository<OpsTemplate> Templates { get; }
    public IRepository<OpsSubmission> Submissions { get; }
    public IRepository<OpsSubmissionFile> SubmissionFiles { get; }
    public IRepository<OpsSubmissionRecipient> SubmissionRecipients { get; }
    public IRepository<OpsApproval> Approvals { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new Repository<AppUser>(context);
        Units = new Repository<Unit>(context);
        Procedures = new Repository<OpsProcedure>(context);
        ProcedureDocuments = new Repository<OpsProcedureDocument>(context);
        Templates = new Repository<OpsTemplate>(context);
        Submissions = new Repository<OpsSubmission>(context);
        SubmissionFiles = new Repository<OpsSubmissionFile>(context);
        SubmissionRecipients = new Repository<OpsSubmissionRecipient>(context);
        Approvals = new Repository<OpsApproval>(context);
    }

    /// <summary>
    /// Get or create repository for any entity type
    /// </summary>
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new Repository<TEntity>(_context);
        }

        return (IRepository<TEntity>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
