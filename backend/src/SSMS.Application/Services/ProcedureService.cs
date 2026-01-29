using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service xử lý logic cho Procedure
/// </summary>
public class ProcedureService : IProcedureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _uploadPath;

    public ProcedureService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _uploadPath = configuration["FileUpload:UploadPath"] ?? "uploads";
        
        // Tạo thư mục upload nếu chưa có
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<IEnumerable<ProcedureListDto>> GetAllAsync()
    {
        var procedures = await _unitOfWork.Procedures.GetAllAsync();
        
        var result = new List<ProcedureListDto>();
        foreach (var proc in procedures)
        {
            // Load related data
            var owner = proc.OwnerUserId.HasValue 
                ? await _unitOfWork.Users.GetByIdAsync(proc.OwnerUserId.Value) 
                : null;
            
            var documents = await _unitOfWork.ProcedureDocuments
                .FindAsync(d => d.ProcedureId == proc.Id);
            
            var templates = await _unitOfWork.Templates
                .FindAsync(t => t.ProcedureId == proc.Id);

            result.Add(new ProcedureListDto
            {
                Id = proc.Id,
                Code = proc.Code,
                Name = proc.Name,
                Version = proc.Version,
                State = proc.State,
                OwnerUserName = owner?.Username,
                ReleasedDate = proc.ReleasedDate,
                DocumentCount = documents.Count(),
                TemplateCount = templates.Count(),
                CreatedAt = proc.CreatedAt
            });
        }

        return result.OrderByDescending(p => p.CreatedAt);
    }

    public async Task<ProcedureDto?> GetByIdAsync(int id)
    {
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(id);
        if (procedure == null) return null;

        // Load related users
        var owner = procedure.OwnerUserId.HasValue 
            ? await _unitOfWork.Users.GetByIdAsync(procedure.OwnerUserId.Value) 
            : null;
        
        var author = procedure.AuthorUserId.HasValue 
            ? await _unitOfWork.Users.GetByIdAsync(procedure.AuthorUserId.Value) 
            : null;
        
        var approver = procedure.ApproverUserId.HasValue 
            ? await _unitOfWork.Users.GetByIdAsync(procedure.ApproverUserId.Value) 
            : null;

        // Load documents
        var documents = await _unitOfWork.ProcedureDocuments
            .FindAsync(d => d.ProcedureId == id);
        
        // Load templates
        var templates = await _unitOfWork.Templates
            .FindAsync(t => t.ProcedureId == id);

        return new ProcedureDto
        {
            Id = procedure.Id,
            Code = procedure.Code,
            Name = procedure.Name,
            Version = procedure.Version,
            State = procedure.State,
            Description = procedure.Description,
            CreatedDate = procedure.CreatedDate,
            ReleasedDate = procedure.ReleasedDate,
            OwnerUserId = procedure.OwnerUserId,
            OwnerUserName = owner?.Username,
            AuthorUserId = procedure.AuthorUserId,
            AuthorUserName = author?.Username,
            ApproverUserId = procedure.ApproverUserId,
            ApproverUserName = approver?.Username,
            Documents = documents.Select(d => new ProcedureDocumentDto
            {
                Id = d.Id,
                ProcedureId = d.ProcedureId,
                DocVersion = d.DocVersion,
                FileName = d.FileName,
                FilePath = d.FilePath,
                FileSize = d.FileSize,
                ContentType = d.ContentType,
                UploadedAt = d.UploadedAt
            }).ToList(),
            Templates = templates.Select(t => new TemplateDto
            {
                Id = t.Id,
                ProcedureId = t.ProcedureId,
                TemplateKey = t.TemplateKey,
                TemplateNo = t.TemplateNo,
                Name = t.Name,
                TemplateType = t.TemplateType,
                State = t.State,
                FileName = t.FileName,
                FilePath = t.FilePath,
                FileSize = t.FileSize,
                ContentType = t.ContentType,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            }).ToList(),
            CreatedAt = procedure.CreatedAt,
            UpdatedAt = procedure.UpdatedAt
        };
    }

    public async Task<ProcedureDto> CreateAsync(ProcedureCreateDto dto)
    {
        // Tự động sinh mã quy trình nếu không có
        string code;
        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            code = await GenerateProcedureCodeAsync();
        }
        else
        {
            code = dto.Code;
            // Kiểm tra Code đã tồn tại chưa
            var existing = await _unitOfWork.Procedures
                .FirstOrDefaultAsync(p => p.Code == code);

            if (existing != null)
            {
                throw new InvalidOperationException($"Mã quy trình '{code}' đã tồn tại");
            }
        }

        var procedure = new OpsProcedure
        {
            Code = code,
            Name = dto.Name,
            Version = dto.Version ?? "1.0",
            Description = dto.Description,
            OwnerUserId = dto.OwnerUserId,
            AuthorUserId = dto.AuthorUserId,
            State = "Draft",
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Procedures.AddAsync(procedure);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(procedure.Id))!;
    }

    /// <summary>
    /// Tự động sinh mã quy trình theo format OPS-XX
    /// </summary>
    private async Task<string> GenerateProcedureCodeAsync()
    {
        var allProcedures = await _unitOfWork.Procedures.GetAllAsync();

        // Tìm số lớn nhất hiện có
        int maxNumber = 0;
        foreach (var proc in allProcedures)
        {
            if (proc.Code.StartsWith("OPS-") && proc.Code.Length > 4)
            {
                var numPart = proc.Code.Substring(4);
                if (int.TryParse(numPart, out int num) && num > maxNumber)
                {
                    maxNumber = num;
                }
            }
        }

        // Sinh mã mới
        return $"OPS-{(maxNumber + 1):D2}";
    }

    public async Task<ProcedureDto> UpdateAsync(int id, ProcedureUpdateDto dto)
    {
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(id);
        if (procedure == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quy trình với ID {id}");
        }

        procedure.Name = dto.Name;
        procedure.Version = dto.Version;
        procedure.State = dto.State;
        procedure.Description = dto.Description;
        procedure.OwnerUserId = dto.OwnerUserId;
        procedure.ApproverUserId = dto.ApproverUserId;
        procedure.ReleasedDate = dto.ReleasedDate;

        _unitOfWork.Procedures.Update(procedure);
        await _unitOfWork.SaveChangesAsync();

        return (await GetByIdAsync(id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(id);
        if (procedure == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quy trình với ID {id}");
        }

        // Soft delete
        procedure.IsDeleted = true;
        _unitOfWork.Procedures.Update(procedure);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ProcedureDocumentDto> UploadDocumentAsync(int procedureId, IFormFile file, string? docVersion)
    {
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(procedureId);
        if (procedure == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quy trình với ID {procedureId}");
        }

        // Validate file
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File extension '{extension}' không được phép");
        }

        var maxFileSize = 20 * 1024 * 1024; // 20MB
        if (file.Length > maxFileSize)
        {
            throw new InvalidOperationException($"File vượt quá kích thước cho phép (20MB)");
        }

        // Tạo tên file unique
        var fileName = $"{procedureId}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, "procedures", fileName);
        
        // Tạo thư mục nếu chưa có
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Lưu file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Tạo record trong database
        var document = new OpsProcedureDocument
        {
            ProcedureId = procedureId,
            DocVersion = docVersion,
            FileName = file.FileName,
            FilePath = filePath,
            FileSize = file.Length,
            ContentType = file.ContentType,
            UploadedAt = DateTime.UtcNow
        };

        await _unitOfWork.ProcedureDocuments.AddAsync(document);
        await _unitOfWork.SaveChangesAsync();

        return new ProcedureDocumentDto
        {
            Id = document.Id,
            ProcedureId = document.ProcedureId,
            DocVersion = document.DocVersion,
            FileName = document.FileName,
            FilePath = document.FilePath,
            FileSize = document.FileSize,
            ContentType = document.ContentType,
            UploadedAt = document.UploadedAt
        };
    }

    public async Task DeleteDocumentAsync(int documentId)
    {
        var document = await _unitOfWork.ProcedureDocuments.GetByIdAsync(documentId);
        if (document == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy tài liệu với ID {documentId}");
        }

        // Xóa file vật lý
        if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
        }

        // Xóa record
        _unitOfWork.ProcedureDocuments.Remove(document);
        await _unitOfWork.SaveChangesAsync();
    }
}
