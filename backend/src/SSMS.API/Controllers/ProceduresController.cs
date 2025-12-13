using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller quản lý Quy trình
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureService _procedureService;
    private readonly ILogger<ProceduresController> _logger;

    public ProceduresController(
        IProcedureService procedureService,
        ILogger<ProceduresController> logger)
    {
        _procedureService = procedureService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả quy trình
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var procedures = await _procedureService.GetAllAsync();
            return Ok(new
            {
                Success = true,
                Data = procedures
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting procedures");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách quy trình"
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết quy trình theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var procedure = await _procedureService.GetByIdAsync(id);
            if (procedure == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Không tìm thấy quy trình"
                });
            }

            return Ok(new
            {
                Success = true,
                Data = procedure
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting procedure {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy thông tin quy trình"
            });
        }
    }

    /// <summary>
    /// Tạo quy trình mới
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create([FromBody] ProcedureCreateDto dto)
    {
        try
        {
            var procedure = await _procedureService.CreateAsync(dto);
            
            _logger.LogInformation("Created procedure {Code} by user {User}", 
                procedure.Code, User.Identity?.Name);

            return CreatedAtAction(
                nameof(GetById),
                new { id = procedure.Id },
                new
                {
                    Success = true,
                    Data = procedure,
                    Message = "Tạo quy trình thành công"
                });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating procedure");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi tạo quy trình"
            });
        }
    }

    /// <summary>
    /// Cập nhật quy trình
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] ProcedureUpdateDto dto)
    {
        try
        {
            var procedure = await _procedureService.UpdateAsync(id, dto);
            
            _logger.LogInformation("Updated procedure {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Data = procedure,
                Message = "Cập nhật quy trình thành công"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating procedure {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi cập nhật quy trình"
            });
        }
    }

    /// <summary>
    /// Xóa quy trình
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _procedureService.DeleteAsync(id);
            
            _logger.LogInformation("Deleted procedure {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Message = "Xóa quy trình thành công"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting procedure {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi xóa quy trình"
            });
        }
    }

    /// <summary>
    /// Upload tài liệu đính kèm
    /// </summary>
    [HttpPost("{id}/documents")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> UploadDocument(
        int id,
        [FromForm] IFormFile file,
        [FromForm] string? docVersion)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "File không hợp lệ"
                });
            }

            var document = await _procedureService.UploadDocumentAsync(id, file, docVersion);
            
            _logger.LogInformation("Uploaded document for procedure {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Data = document,
                Message = "Upload tài liệu thành công"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document for procedure {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi upload tài liệu"
            });
        }
    }

    /// <summary>
    /// Xóa tài liệu đính kèm
    /// </summary>
    [HttpDelete("documents/{documentId}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> DeleteDocument(int documentId)
    {
        try
        {
            await _procedureService.DeleteDocumentAsync(documentId);
            
            _logger.LogInformation("Deleted document {Id} by user {User}", 
                documentId, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Message = "Xóa tài liệu thành công"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {Id}", documentId);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi xóa tài liệu"
            });
        }
    }
}
