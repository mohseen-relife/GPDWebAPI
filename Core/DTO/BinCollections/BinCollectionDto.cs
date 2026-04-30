using DTO.Common;

namespace DTO.BinCollections
{
    // ═══════════════════════════════════════════════════════════════════════
    // REQUEST — filter + pagination
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Query-string filter parameters for GET /api/bin-collections.
    /// All fields are optional and combinable.
    /// </summary>
    public class BinCollectionFilterRequest : PaginationRequest
    {
        /// <summary>
        /// Filter by customer name.
        /// Matched against rfid_bins_37.Customer (exact, case-insensitive).
        /// Example: ?customerId=CTest01
        /// </summary>
        public string? CustomerId { get; set; }

        /// <summary>
        /// Filter by vehicle plate number.
        /// Matched against users.name (LIKE partial match).
        /// Example: ?vehicleId=KHR/55001
        /// </summary>
        public string? VehicleId { get; set; }

        /// <summary>
        /// Filter by bin RFID tag.
        /// Matched against rfid_bins_reports.rfid (exact match, trimmed).
        /// Example: ?BinRFID=E200534A4245454148130808
        /// </summary>
        public string? BinRFID { get; set; }

        /// <summary>
        /// Date range start — YYYY-MM-DD format (inclusive, UTC comparison).
        /// Example: ?from=2026-04-01
        /// </summary>
        public string? From { get; set; }

        /// <summary>
        /// Date range end — YYYY-MM-DD format (inclusive, UTC comparison).
        /// Example: ?to=2026-04-25
        /// </summary>
        public string? To { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // RESPONSE — single record detail
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// One bin-collection transaction row assembled from 3 joined tables.
    /// </summary>
    public class BinCollectionDetails
    {
        // ── From rfid_bins_reports ──────────────────────────────────────────
        /// <summary>Collection event timestamp (UTC)</summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>RFID tag of the bin</summary>
        public string? Rfid { get; set; }

        /// <summary>Gross weight in grams</summary>
        public int? Weight { get; set; }

        // ── From rfid_bins_37 ───────────────────────────────────────────────
        /// <summary>Bin label e.g. "A130808"</summary>
        public string? Name { get; set; }

        /// <summary>Bin type e.g. "1.1 CBM Plastic Green Bin"</summary>
        public string? Type { get; set; }

        /// <summary>Operational zone</summary>
        public string? Zone { get; set; }

        /// <summary>Latitude of bin location</summary>
        public double? Lat { get; set; }

        /// <summary>Longitude of bin location</summary>
        public double? Lng { get; set; }

        /// <summary>Tare weight in kg</summary>
        public int? TWeight { get; set; }

        /// <summary>Collection count</summary>
        public int? CCount { get; set; }

        /// <summary>Customer name</summary>
        public string? Customer { get; set; }

        /// <summary>Assigned collection route</summary>
        public string? Route { get; set; }

        /// <summary>District</summary>
        public string? District { get; set; }

        /// <summary>Sub-district</summary>
        public string? Subdistrict { get; set; }

        // ── From users (via rfid_bins_reports.vehicle → users.username) ─────
        /// <summary>
        /// Vehicle plate/description from users.name.
        /// e.g. "KHR/55001 (CR-Khor Fakkan)"
        /// Falls back to raw vehicle username if no user row matched.
        /// </summary>
        public string? Vehicle { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // PAGINATED RESPONSE
    // ═══════════════════════════════════════════════════════════════════════

    /// <summary>Paginated list of bin-collection records.</summary>
    public class BinCollectionPaginationResponse : PaginationResponse
    {
        public List<BinCollectionDetails> Records { get; set; } = new();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // AUTH DTOs (kept in same project for simplicity)
    // ═══════════════════════════════════════════════════════════════════════

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? Expires { get; set; }
        public string? Username { get; set; }
        public string? Message { get; set; }
    }
}
