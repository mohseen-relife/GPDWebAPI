using DTO.BinCollections;

namespace Contracts.Managers
{
    /// <summary>
    /// Business logic contract for bin-collection operations.
    /// Implementation lives in BI.BinCollectionManager.
    /// </summary>
    public interface IBinCollectionManager
    {
        /// <summary>
        /// Returns a paginated list of bin-collection records,
        /// with optional filters on customer, vehicle, RFID and date range.
        /// </summary>
        Task<BinCollectionPaginationResponse> GetBinCollectionsAsync(BinCollectionFilterRequest request);
    }
}
