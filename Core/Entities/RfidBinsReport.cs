using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Maps to rfid_bins_reports table — primary transaction/event table.
    /// Columns: id, app, rfid, vehicle, time_stamp, mode, weight
    /// </summary>
    [Table("rfid_bins_reports")]
    public class RfidBinsReport
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>App identifier — 37 = Tandeef</summary>
        [Column("app")]
        public int? App { get; set; }

        /// <summary>RFID tag — links to rfid_bins_37.rfid</summary>
        [Column("rfid")]
        [StringLength(64)]
        public string? Rfid { get; set; }

        /// <summary>Vehicle username — links to users.username</summary>
        [Column("vehicle")]
        [StringLength(64)]
        public string? Vehicle { get; set; }

        /// <summary>Collection timestamp stored as UTC</summary>
        [Column("time_stamp")]
        public DateTime? TimeStamp { get; set; }

        [Column("mode")]
        public int? Mode { get; set; }

        /// <summary>Gross weight in grams</summary>
        [Column("weight")]
        public int? Weight { get; set; }
    }
}
