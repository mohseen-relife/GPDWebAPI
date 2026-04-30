using DTO.BinCollections;
using Entities;

namespace BI.Maps
{
    /// <summary>
    /// Manual mapper for BinCollection — following the same pattern as HipRssMap.
    /// Converts raw entity/join data to BinCollectionDetails DTO.
    /// </summary>
    public class BinCollectionMap
    {
        /// <summary>
        /// Maps a raw report + bin metadata + user into a BinCollectionDetails DTO.
        /// Called when doing in-memory mapping outside of EF projection.
        /// </summary>
        public BinCollectionDetails ToDetails(
            RfidBinsReport report,
            RfidBins37?    bin,
            AppUser?       user)
        {
            return new BinCollectionDetails
            {
                // From rfid_bins_reports
                TimeStamp   = report.TimeStamp,
                Rfid        = report.Rfid?.Trim(),
                Weight      = report.Weight,

                // From rfid_bins_37
                Name        = bin?.Name.Trim(),
                Type        = bin?.Type,
                Zone        = bin?.Zone,
                Lat         = bin?.Lat,
                Lng         = bin?.Lng,
                TWeight     = bin?.TWeight,
                CCount      = bin?.CCount,
                Customer    = bin?.Customer,
                Route       = bin?.Route,
                District    = bin?.District,
                Subdistrict = bin?.Subdistrict,

                // From users (prefer plate; fall back to raw vehicle username)
                Vehicle     = !string.IsNullOrWhiteSpace(user?.Name)
                                  ? user!.Name
                                  : report.Vehicle
            };
        }
    }
}
