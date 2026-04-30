using Contracts.Managers;
using DTO.BinCollections;
using DTO.Common;
using Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GpdWebApi.Controllers
{
    /// <summary>
    /// Bin Collections API — returns paginated collection transaction records.
    ///
    /// Primary table  : rfid_bins_reports  (transaction events)
    /// Joined tables  : rfid_bins_37       (bin metadata)
    ///                  users              (vehicle plate via users.name)
    ///
    /// All endpoints require Bearer JWT authentication.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")]
    public class BinCollectionsController : ControllerBase
    {
        private readonly IBinCollectionManager _manager;

        public BinCollectionsController(IBinCollectionManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Returns paginated bin-collection transaction records.
        /// All filter parameters are optional and combinable.
        /// </summary>
        /// <remarks>
        /// Examples:
        ///
        ///     GET /api/bincollections
        ///     GET /api/bincollections?customerId=CTest01
        ///     GET /api/bincollections?vehicleId=KHR/55001
        ///     GET /api/bincollections?BinRFID=E200534A4245454148130808
        ///     GET /api/bincollections?from=2026-04-01&amp;to=2026-04-25
        ///     GET /api/bincollections?customerId=CTest01&amp;from=2026-04-01&amp;to=2026-04-25&amp;index=1&amp;pageSize=20
        ///
        /// </remarks>
        /// <param name="filter">
        ///   customerId  – exact customer name (rfid_bins_37.Customer)
        ///   vehicleId   – partial vehicle plate (users.name LIKE)
        ///   BinRFID     – exact RFID tag (rfid_bins_reports.rfid)
        ///   from        – start date YYYY-MM-DD (inclusive, UTC)
        ///   to          – end date   YYYY-MM-DD (inclusive, UTC)
        ///   index       – page number, 1-based (default: 1)
        ///   pageSize    – records per page (default: 20)
        /// </param>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<BinCollectionPaginationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBinCollections([FromQuery] BinCollectionFilterRequest filter)
        {
            // Default pagination guard
            if (filter.Index    <= 0) filter.Index    = 1;
            if (filter.PageSize <= 0) filter.PageSize = 20;
            if (filter.PageSize > 200) filter.PageSize = 200;   // safety cap

            var result = await _manager.GetBinCollectionsAsync(filter);

            if (result.Result == BinCollectionResult.NotFound)
                return Ok(ApiResponse<BinCollectionPaginationResponse>.Ok(
                    result, 0, "No records found for the provided filters."));

            return Ok(ApiResponse<BinCollectionPaginationResponse>.Ok(
                result, result.TotalCount));
        }

        /// <summary>
        /// Liveness probe — no DB call, confirms the API is running.
        /// Does not require authentication.
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Health()
        {
            return Ok(new
            {
                status    = "healthy",
                service   = "GPD Web API — Bin Collections",
                version   = "1.0.0",
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")
            });
        }
    }
}
