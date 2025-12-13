using Microsoft.AspNetCore.Http;
using SSMS.Application.DTOs;

namespace SSMS.Application.Services;

/// <summary>
/// Interface cho Procedure Service
/// </summary>
public interface IProcedureService
{
    Task<IEnumerable<ProcedureListDto>> GetAllAsync();
    Task<ProcedureDto?> GetByIdAsync(int id);
    Task<ProcedureDto> CreateAsync(ProcedureCreateDto dto);
    Task<ProcedureDto> UpdateAsync(int id, ProcedureUpdateDto dto);
    Task DeleteAsync(int id);
    Task<ProcedureDocumentDto> UploadDocumentAsync(int procedureId, IFormFile file, string? docVersion);
    Task DeleteDocumentAsync(int documentId);
}
