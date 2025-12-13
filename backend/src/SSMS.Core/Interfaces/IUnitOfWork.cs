using SSMS.Core.Entities;

namespace SSMS.Core.Interfaces;

/// <summary>
/// Unit of Work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IRepository<AppUser> Users { get; }
    IRepository<Unit> Units { get; }
    IRepository<OpsProcedure> Procedures { get; }
    IRepository<OpsProcedureDocument> ProcedureDocuments { get; }
    IRepository<OpsTemplate> Templates { get; }
    IRepository<OpsSubmission> Submissions { get; }
    IRepository<OpsSubmissionFile> SubmissionFiles { get; }
    IRepository<OpsSubmissionRecipient> SubmissionRecipients { get; }
    IRepository<OpsApproval> Approvals { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
