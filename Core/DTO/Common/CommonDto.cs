using Entities.Enums;

namespace DTO.Common
{
    /// <summary>Standard pagination request — same pattern as your HipRss reference.</summary>
    public class PaginationRequest
    {
        /// <summary>Page number (1-based)</summary>
        public int Index { get; set; } = 1;

        /// <summary>Records per page</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Optional search/filter text</summary>
        public string? Search { get; set; }
    }

    /// <summary>Base class for all paginated responses.</summary>
    public class PaginationResponse
    {
        public int Index { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public BinCollectionResult Result { get; set; }
    }

    /// <summary>Standard API envelope wrapping any payload.</summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, int total, string message = "Records retrieved successfully.")
            => new() { Success = true, Message = message, TotalRecords = total, Data = data };

        public static ApiResponse<T> Fail(string message)
            => new() { Success = false, Message = message, TotalRecords = 0 };
    }
}
