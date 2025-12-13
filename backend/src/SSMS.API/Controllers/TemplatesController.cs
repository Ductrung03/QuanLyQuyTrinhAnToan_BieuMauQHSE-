using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSMS.Application.DTOs;
using SSMS.Application.Services;

namespace SSMS.API.Controllers;

/// <summary>
/// Controller quản lý Template/Biểu mẫu
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;
    private readonly ILogger<TemplatesController> _logger;

    public TemplatesController(
        ITemplateService templateService,
        ILogger<TemplatesController> logger)
    {
        _templateService = templateService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả templates
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var templates = await _templateService.GetAllAsync();
            return Ok(new
            {
                Success = true,
                Data = templates
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Lấy templates theo procedure ID
    /// </summary>
    [HttpGet("procedure/{procedureId}")]
    public async Task<IActionResult> GetByProcedureId(int procedureId)
    {
        try
        {
            var templates = await _templateService.GetByProcedureIdAsync(procedureId);
            return Ok(new
            {
                Success = true,
                Data = templates
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates for procedure {Id}", procedureId);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy danh sách biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết template theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var template = await _templateService.GetByIdAsync(id);
            if (template == null)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = "Không tìm thấy biểu mẫu"
                });
            }

            return Ok(new
            {
                Success = true,
                Data = template
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi lấy thông tin biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Tạo template mới
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create(
        [FromForm] TemplateCreateDto dto,
        [FromForm] IFormFile? file)
    {
        try
        {
            var template = await _templateService.CreateAsync(dto, file);
            
            _logger.LogInformation("Created template {Name} by user {User}", 
                template.Name, User.Identity?.Name);

            return CreatedAtAction(
                nameof(GetById),
                new { id = template.Id },
                new
                {
                    Success = true,
                    Data = template,
                    Message = "Tạo biểu mẫu thành công"
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
            _logger.LogError(ex, "Error creating template");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi tạo biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Cập nhật template
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Update(int id, [FromBody] TemplateUpdateDto dto)
    {
        try
        {
            var template = await _templateService.UpdateAsync(id, dto);
            
            _logger.LogInformation("Updated template {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Data = template,
                Message = "Cập nhật biểu mẫu thành công"
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
            _logger.LogError(ex, "Error updating template {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi cập nhật biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Xóa template
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _templateService.DeleteAsync(id);
            
            _logger.LogInformation("Deleted template {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Message = "Xóa biểu mẫu thành công"
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
            _logger.LogError(ex, "Error deleting template {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi xóa biểu mẫu"
            });
        }
    }

    /// <summary>
    /// Upload file cho template
    /// </summary>
    [HttpPost("{id}/upload")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> UploadFile(int id, [FromForm] IFormFile file)
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

            var template = await _templateService.UploadFileAsync(id, file);
            
            _logger.LogInformation("Uploaded file for template {Id} by user {User}", 
                id, User.Identity?.Name);

            return Ok(new
            {
                Success = true,
                Data = template,
                Message = "Upload file thành công"
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
            _logger.LogError(ex, "Error uploading file for template {Id}", id);
            return StatusCode(500, new
            {
                Success = false,
                Message = "Lỗi khi upload file"
            });
        }
    }
}
