using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Maps to rfid_bins_37 table — bin master/metadata.
    /// Columns: id, name, zone, bin, type, rfid, lat, lng, time_stamp,
    ///          vehicle, weight, tweight, ccount, wcount, wash_time,
    ///          wash_vhcl, route, district, subdistrict, on, interval, Customer
    /// </summary>
    [Table("rfid_bins_37")]
    public class RfidBins37
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(64)]
        public string Name { get; set; } = string.Empty;

        [Column("zone")]
        [StringLength(64)]
        public string Zone { get; set; } = string.Empty;

        [Column("bin")]
        [StringLength(64)]
        public string Bin { get; set; } = string.Empty;

        [Column("type")]
        [StringLength(64)]
        public string Type { get; set; } = string.Empty;

        /// <summary>RFID tag — join key with rfid_bins_reports.rfid (use TRIM)</summary>
        [Column("rfid")]
        [StringLength(64)]
        public string Rfid { get; set; } = string.Empty;

        [Column("lat")]
        public double Lat { get; set; }

        [Column("lng")]
        public double Lng { get; set; }

        [Column("time_stamp")]
        public DateTime? TimeStamp { get; set; }

        /// <summary>Vehicle username — links to users.username</summary>
        [Column("vehicle")]
        [StringLength(64)]
        public string? Vehicle { get; set; }

        [Column("weight")]
        public int? Weight { get; set; }

        /// <summary>Tare weight in kg</summary>
        [Column("tweight")]
        public int? TWeight { get; set; }

        /// <summary>Collection count</summary>
        [Column("ccount")]
        public int? CCount { get; set; }

        [Column("wcount")]
        public int? WCount { get; set; }

        [Column("route")]
        [StringLength(128)]
        public string? Route { get; set; }

        [Column("district")]
        [StringLength(128)]
        public string? District { get; set; }

        [Column("subdistrict")]
        [StringLength(128)]
        public string? Subdistrict { get; set; }

        /// <summary>Customer name — filter field</summary>
        [Column("Customer")]
        [StringLength(128)]
        public string? Customer { get; set; }
    }
}
