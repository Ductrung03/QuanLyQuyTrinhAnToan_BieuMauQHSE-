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

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
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
