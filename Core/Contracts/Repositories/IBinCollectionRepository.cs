using DTO.BinCollections;
using Entities;

namespace Contracts.Repositories
{
    /// <summary>
    /// Data access contract for bin-collection queries.
    /// Implementation lives in DAL.Repositories.BinCollectionRepository.
    /// </summary>
    public interface IBinCollectionRepository
    {
        /// <summary>
        /// Returns a filtered, ordered IQueryable of bin-collection records
        /// from rfid_bins_reports joined with rfid_bins_37 and users.
        /// Caller applies pagination (Skip/Take).
        /// </summary>
        IQueryable<BinCollectionDetails> GetBinCollections(BinCollectionFilterRequest filter);
    }
}
