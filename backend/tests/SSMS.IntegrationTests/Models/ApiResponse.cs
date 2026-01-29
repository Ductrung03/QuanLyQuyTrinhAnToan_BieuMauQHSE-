namespace SSMS.IntegrationTests.Models;

/// <summary>
/// Generic API response wrapper (matches controller response)
/// </summary>
public record ApiResponse<T>(bool Success, T? Data, string? Error);
