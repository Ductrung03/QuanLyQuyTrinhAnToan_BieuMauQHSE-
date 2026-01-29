namespace SSMS.Application.Common;

/// <summary>
/// Standardized API response wrapper for all endpoints
/// </summary>
/// <typeparam name="T">Type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates if the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The actual data payload (null if error)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// User-friendly message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// List of validation or error messages
    /// </summary>
    public IEnumerable<string>? Errors { get; set; }

    /// <summary>
    /// Pagination metadata (for list responses)
    /// </summary>
    public PaginationMeta? Meta { get; set; }

    /// <summary>
    /// Create a successful response
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Create a successful response with pagination
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, PaginationMeta meta, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Meta = meta,
            Message = message
        };
    }

    /// <summary>
    /// Create an error response
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    /// <summary>
    /// Create an error response with single error
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = new[] { error }
        };
    }
}

/// <summary>
/// Pagination metadata for list responses
/// </summary>
public class PaginationMeta
{
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// Calculate pagination metadata
    /// </summary>
    public static PaginationMeta Create(int page, int pageSize, int totalCount)
    {
        return new PaginationMeta
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
