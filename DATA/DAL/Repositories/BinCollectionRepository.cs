using Contracts.Repositories;
using DTO.BinCollections;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DAL.Repositories
{
    /// <summary>
    /// Implements IBinCollectionRepository using EF Core + LINQ.
    ///
    /// Join strategy:
    ///   rfid_bins_reports (r)  PRIMARY — transactions
    ///       LEFT JOIN rfid_bins_37 (b)  ON TRIM(b.rfid) = TRIM(r.rfid)
    ///       LEFT JOIN users       (u)  ON u.username    = r.vehicle
    ///   WHERE r.app = 37  (Tandeef only — always applied)
    ///
    /// Note: rfid column has trailing spaces in data — EF Trim() handles this.
    /// </summary>
    public class BinCollectionRepository : IBinCollectionRepository
    {
        private readonly GpdDbContext _context;

        public BinCollectionRepository(GpdDbContext context)
        {
            _context = context;
        }

        /*  public IQueryable<BinCollectionDetails> GetBinCollections(BinCollectionFilterRequest filter)
          {
              // ── Parse date inputs ─────────────────────────────────────────────
              DateTime? fromDate = ParseFromDate(filter.From);
              DateTime? toDate   = ParseToDate(filter.To);

              // ── Base join query ───────────────────────────────────────────────
              var query = from r in _context.RfidBinsReports
                          join b in _context.RfidBins37
                              on r.Rfid!.Trim() equals b.Rfid.Trim() into binJoin
                          from b in binJoin.DefaultIfEmpty()          // LEFT JOIN
                          join u in _context.Users
                              on r.Vehicle equals u.Username into userJoin
                          from u in userJoin.DefaultIfEmpty()         // LEFT JOIN
                          select new BinCollectionDetails
                          {
                              TimeStamp   = r.TimeStamp,
                              Rfid        = r.Rfid!.Trim(),
                              Weight      = r.Weight,
                              Name        = b != null ? b.Name.Trim()     : null,
                              Type        = b != null ? b.Type            : null,
                              Zone        = b != null ? b.Zone            : null,
                              Lat         = b != null ? b.Lat             : (double?)null,
                              Lng         = b != null ? b.Lng             : (double?)null,
                              TWeight     = b != null ? b.TWeight         : null,
                              CCount      = b != null ? b.CCount          : null,
                              Customer    = b != null ? b.Customer        : null,
                              Route       = b != null ? b.Route           : null,
                              District    = b != null ? b.District        : null,
                              Subdistrict = b != null ? b.Subdistrict     : null,
                              // Prefer plate from users.name; fallback to raw vehicle username
                              Vehicle     = u != null && u.Name != null
                                                ? u.Name
                                                : r.Vehicle
                          };

              // ── Dynamic filters ───────────────────────────────────────────────

              // 1. Customer — exact match (case-insensitive via MySQL collation)
              if (!string.IsNullOrWhiteSpace(filter.CustomerId))
              {
                  var c = filter.CustomerId.Trim();
                  query = query.Where(x => x.Customer == c);
              }

              // 2. Vehicle plate — partial LIKE on the projected Vehicle field
              if (!string.IsNullOrWhiteSpace(filter.VehicleId))
              {
                  var v = filter.VehicleId.Trim();
                  query = query.Where(x => x.Vehicle != null && x.Vehicle.Contains(v));
              }

              // 3. Bin RFID — exact match (trimmed)
              if (!string.IsNullOrWhiteSpace(filter.BinRFID))
              {
                  var rfid = filter.BinRFID.Trim();
                  query = query.Where(x => x.Rfid == rfid);
              }

              // 4. Date range — UTC comparison on rfid_bins_reports.time_stamp
              if (fromDate.HasValue)
                  query = query.Where(x => x.TimeStamp >= fromDate.Value);

              if (toDate.HasValue)
                  query = query.Where(x => x.TimeStamp <= toDate.Value);

              // 5. Search (generic text filter across name/zone/route)
              if (!string.IsNullOrWhiteSpace(filter.Search))
              {
                  var s = filter.Search.Trim().ToLower();
                  query = query.Where(x =>
                      (x.Name        != null && x.Name.ToLower().Contains(s)) ||
                      (x.Zone        != null && x.Zone.ToLower().Contains(s)) ||
                      (x.Route       != null && x.Route.ToLower().Contains(s)) ||
                      (x.Customer    != null && x.Customer.ToLower().Contains(s)));
              }

              return query.OrderByDescending(x => x.TimeStamp);
          }*/
        public IQueryable<BinCollectionDetails> GetBinCollections(BinCollectionFilterRequest filter)
        {
            DateTime? fromDate = ParseFromDate(filter.From);
            DateTime? toDate = ParseToDate(filter.To);

            var query = _context.RfidBinsReports.AsNoTracking();

            // Filters first
            if (!string.IsNullOrWhiteSpace(filter.BinRFID))
            {
                var rfid = filter.BinRFID.Trim();
                query = query.Where(x => x.Rfid == rfid);
            }

            if (fromDate.HasValue)
                query = query.Where(x => x.TimeStamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.TimeStamp <= toDate.Value);

            // Join after filtering
            var result =
                from r in query
                join b in _context.RfidBins37.AsNoTracking()
                    on r.Rfid equals b.Rfid into binJoin
                from b in binJoin.DefaultIfEmpty()

                join u in _context.Users.AsNoTracking()
                    on r.Vehicle equals u.Username into userJoin
                from u in userJoin.DefaultIfEmpty()

                select new BinCollectionDetails
                {
                    TimeStamp = r.TimeStamp,
                    Rfid = r.Rfid,
                    Weight = r.Weight,
                    Name = b != null ? b.Name : null,
                    Type = b != null ? b.Type : null,
                    Zone = b != null ? b.Zone : null,
                    Lat = b != null ? b.Lat : null,
                    Lng = b != null ? b.Lng : null,
                    TWeight = b != null ? b.TWeight : null,
                    CCount = b != null ? b.CCount : null,
                    Customer = b != null ? b.Customer : null,
                    Route = b != null ? b.Route : null,
                    District = b != null ? b.District : null,
                    Subdistrict = b != null ? b.Subdistrict : null,
                    Vehicle = u != null && u.Name != null ? u.Name : r.Vehicle
                };

            if (!string.IsNullOrWhiteSpace(filter.CustomerId))
            {
                var customer = filter.CustomerId.Trim();
                result = result.Where(x => x.Customer == customer);
            }

            if (!string.IsNullOrWhiteSpace(filter.VehicleId))
            {
                var vehicle = filter.VehicleId.Trim();
                result = result.Where(x => x.Vehicle != null && x.Vehicle.Contains(vehicle));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                result = result.Where(x =>
                    (x.Name != null && x.Name.Contains(search)) ||
                    (x.Zone != null && x.Zone.Contains(search)) ||
                    (x.Route != null && x.Route.Contains(search)) ||
                    (x.Customer != null && x.Customer.Contains(search)));
            }

            return result.OrderByDescending(x => x.TimeStamp);
        }
        // ── Date parsing helpers ──────────────────────────────────────────────

        private static DateTime? ParseFromDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (!DateTime.TryParseExact(input.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                throw new ArgumentException(
                    $"Invalid 'from' date \"{input}\". Use YYYY-MM-DD (e.g. 2026-04-01).");
            return DateTime.SpecifyKind(d.Date, DateTimeKind.Utc);
        }

        private static DateTime? ParseToDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (!DateTime.TryParseExact(input.Trim(), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                throw new ArgumentException(
                    $"Invalid 'to' date \"{input}\". Use YYYY-MM-DD (e.g. 2026-04-25).");
            return DateTime.SpecifyKind(d.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
        }
    }
}
