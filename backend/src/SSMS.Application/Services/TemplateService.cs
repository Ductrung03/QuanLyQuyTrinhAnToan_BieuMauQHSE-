using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SSMS.Application.DTOs;
using SSMS.Core.Entities;
using SSMS.Core.Interfaces;

namespace SSMS.Application.Services;

/// <summary>
/// Service xử lý logic cho Template
/// </summary>
public class TemplateService : ITemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _uploadPath;

    public TemplateService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _uploadPath = configuration["FileUpload:UploadPath"] ?? "uploads";
        
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<IEnumerable<TemplateDto>> GetAllAsync()
    {
        var templates = await _unitOfWork.Templates.GetAllAsync();
        return templates.Select(MapToDto).OrderByDescending(t => t.CreatedAt);
    }

    public async Task<IEnumerable<TemplateDto>> GetByProcedureIdAsync(int procedureId)
    {
        var templates = await _unitOfWork.Templates.FindAsync(t => t.ProcedureId == procedureId);
        return templates.Select(MapToDto).OrderBy(t => t.Name);
    }

    public async Task<TemplateDto?> GetByIdAsync(int id)
    {
        var template = await _unitOfWork.Templates.GetByIdAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<TemplateDto> CreateAsync(TemplateCreateDto dto, IFormFile? file)
    {
        // Kiểm tra procedure tồn tại
        var procedure = await _unitOfWork.Procedures.GetByIdAsync(dto.ProcedureId);
        if (procedure == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy quy trình với ID {dto.ProcedureId}");
        }

        // Tự động sinh mã biểu mẫu nếu không có
        string templateNo;
        if (string.IsNullOrWhiteSpace(dto.TemplateNo))
        {
            templateNo = await GenerateTemplateNoAsync(procedure.Code ?? string.Empty, dto.TemplateType);
        }
        else
        {
            templateNo = dto.TemplateNo;
        }

        // Tự động sinh TemplateKey nếu không có
        string templateKey = dto.TemplateKey ?? string.Empty;
        if (string.IsNullOrWhiteSpace(templateKey))
        {
            templateKey = $"T{DateTime.UtcNow:yyMMddHHmmss}";
        }

        var template = new OpsTemplate
        {
            ProcedureId = dto.ProcedureId,
            TemplateKey = templateKey,
            TemplateNo = templateNo,
            Name = dto.Name,
            TemplateType = dto.TemplateType,
            State = "Draft",
            IsActive = true
        };

        // Upload file nếu có
        if (file != null)
        {
            await SaveFileAsync(template, file);
        }

        await _unitOfWork.Templates.AddAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(template);
    }

    /// <summary>
    /// Tự động sinh mã biểu mẫu theo format FM-OPS-XX hoặc CL-OPS-XX
    /// </summary>
    private async Task<string> GenerateTemplateNoAsync(string procedureCode, string templateType)
    {
        // Prefix: FM cho Form, CL cho Checklist
        var prefix = templateType == "Checklist" ? "CL" : "FM";
        var baseCode = $"{prefix}-{procedureCode}";

        // Lấy tất cả templates của procedure này
        var allTemplates = await _unitOfWork.Templates.GetAllAsync();

        // Tìm số lớn nhất hiện có
        int maxNumber = 0;
        foreach (var tpl in allTemplates)
        {
            if (!string.IsNullOrEmpty(tpl.TemplateNo) && tpl.TemplateNo.StartsWith(baseCode))
            {
                // Ví dụ: FM-OPS-01-03 -> lấy 03
                var parts = tpl.TemplateNo.Split('-');
                if (parts.Length >= 4 && int.TryParse(parts[^1], out int num) && num > maxNumber)
                {
                    maxNumber = num;
                }
            }
        }

        // Sinh mã mới: FM-OPS-01-01, FM-OPS-01-02...
        return $"{baseCode}-{(maxNumber + 1):D2}";
    }

    public async Task<TemplateDto> UpdateAsync(int id, TemplateUpdateDto dto)
    {
        var template = await _unitOfWork.Templates.GetByIdAsync(id);
        if (template == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy template với ID {id}");
        }

        template.Name = dto.Name;
        template.TemplateNo = dto.TemplateNo;
        template.TemplateType = dto.TemplateType;
        template.State = dto.State;
        template.IsActive = dto.IsActive;

        _unitOfWork.Templates.Update(template);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(template);
    }

    public async Task DeleteAsync(int id)
    {
        var template = await _unitOfWork.Templates.GetByIdAsync(id);
        if (template == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy template với ID {id}");
        }

        // Xóa file vật lý nếu có
        if (!string.IsNullOrEmpty(template.FilePath) && File.Exists(template.FilePath))
        {
            File.Delete(template.FilePath);
        }

        // Soft delete
        template.IsDeleted = true;
        _unitOfWork.Templates.Update(template);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<TemplateDto> UploadFileAsync(int templateId, IFormFile file)
    {
        var template = await _unitOfWork.Templates.GetByIdAsync(templateId);
        if (template == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy template với ID {templateId}");
        }

        // Xóa file cũ nếu có
        if (!string.IsNullOrEmpty(template.FilePath) && File.Exists(template.FilePath))
        {
            File.Delete(template.FilePath);
        }

        // Upload file mới
        await SaveFileAsync(template, file);

        _unitOfWork.Templates.Update(template);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(template);
    }

    private async Task SaveFileAsync(OpsTemplate template, IFormFile file)
    {
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
        var fileName = $"template_{template.ProcedureId}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(_uploadPath, "templates", fileName);
        
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

        template.FileName = file.FileName;
        template.FilePath = filePath;
        template.FileSize = file.Length;
        template.ContentType = file.ContentType;
    }

    private static TemplateDto MapToDto(OpsTemplate template)
    {
        return new TemplateDto
        {
            Id = template.Id,
            ProcedureId = template.ProcedureId,
            TemplateKey = template.TemplateKey,
            TemplateNo = template.TemplateNo,
            Name = template.Name,
            TemplateType = template.TemplateType,
            State = template.State,
            FileName = template.FileName,
            FilePath = template.FilePath,
            FileSize = template.FileSize,
            ContentType = template.ContentType,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt
        };
    }
}
