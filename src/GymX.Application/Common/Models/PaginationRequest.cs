
namespace GymX.Application.Common.Models
{
    public record PaginationRequest
    {
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SortBy { get; init; }
        public bool IsDescending { get; init; } = false;
        public string? SearchTerm { get; init; }
    }
}
