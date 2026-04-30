using Contracts.Managers;
using Contracts.Repositories;
using DTO.BinCollections;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace BI
{
    /// <summary>
    /// Business logic for bin-collection queries.
    /// Follows the same pattern as HipRssManager:
    ///   - injects IRepo via constructor
    ///   - applies pagination: TotalCount → Skip/Take → PageCount
    ///   - returns typed response with Result enum
    /// </summary>
    public class BinCollectionManager : IBinCollectionManager
    {
        private readonly IBinCollectionRepository _repo;

        public BinCollectionManager(IBinCollectionRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Returns a paginated list of bin-collection transactions
        /// with optional filters on customer, vehicle, RFID and date range.
        /// </summary>
        /*public async Task<BinCollectionPaginationResponse> GetBinCollectionsAsync(
            BinCollectionFilterRequest request)
        {
            var response = new BinCollectionPaginationResponse
            {
                Index    = request.Index,
                PageSize = request.PageSize
            };

            // Validate date inputs before hitting the DB
            try
            {
                ValidateDateRange(request.From, request.To);
            }
            catch (ArgumentException ex)
            {
                response.Result = BinCollectionResult.InvalidDateRange;
                // Caller (controller) surfaces the message
                throw new ArgumentException(ex.Message);
            }

            // Get filtered, ordered IQueryable (no data fetched yet)
            var query = _repo.GetBinCollections(request);

            // Count total matching rows (one DB call)
            response.TotalCount = await query.CountAsync();

            if (response.TotalCount == 0)
            {
                response.Result = BinCollectionResult.NotFound;
                return response;
            }

            // Fetch the requested page
            response.Records = await query
                .Skip((request.Index - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            response.PageCount = (response.TotalCount / request.PageSize)
                + (response.TotalCount % request.PageSize > 0 ? 1 : 0);

            response.Result = BinCollectionResult.Success;
            return response;
        }*/
        public async Task<BinCollectionPaginationResponse> GetBinCollectionsAsync(
    BinCollectionFilterRequest request)
        {
            // Prevent invalid pagination
            request.Index = request.Index <= 0 ? 1 : request.Index;
            request.PageSize = request.PageSize <= 0 ? 10 : request.PageSize;

            // Validate dates
            ValidateDateRange(request.From, request.To);

            var response = new BinCollectionPaginationResponse
            {
                Index = request.Index,
                PageSize = request.PageSize
            };

            // Get optimized query from repo
            var query = _repo.GetBinCollections(request);

            // Total records count
            response.TotalCount = await query.CountAsync();

            if (response.TotalCount == 0)
            {
                response.Result = BinCollectionResult.NotFound;
                response.Records = new List<BinCollectionDetails>();
                response.PageCount = 0;
                return response;
            }

            // Fetch paginated records
            response.Records = await query
                .Skip((request.Index - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Faster page count calculation
            response.PageCount = (int)Math.Ceiling(
                (double)response.TotalCount / request.PageSize);

            response.Result = BinCollectionResult.Success;

            return response;
        }
        // ── Helpers ───────────────────────────────────────────────────────────

        private static void ValidateDateRange(string? from, string? to)
        {
            // Individual date format validation happens inside the repository.
            // Here we check logical range consistency.
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return;

            if (DateTime.TryParse(from, out var f) && DateTime.TryParse(to, out var t))
            {
                if (f > t)
                    throw new ArgumentException(
                        "'from' date cannot be later than 'to' date.");
            }
        }
    }
}
