using Microsoft.AspNetCore.Http;
using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Interface cho Template Service
/// </summary>
public interface ITemplateService
{
    Task<IEnumerable<TemplateDto>> GetAllAsync();
    Task<IEnumerable<TemplateDto>> GetByProcedureIdAsync(int procedureId);
    Task<TemplateDto?> GetByIdAsync(int id);
    Task<TemplateDto> CreateAsync(TemplateCreateDto dto, IFormFile? file);
    Task<TemplateDto> UpdateAsync(int id, TemplateUpdateDto dto);
    Task DeleteAsync(int id);
    Task<TemplateDto> UploadFileAsync(int templateId, IFormFile file);
}
