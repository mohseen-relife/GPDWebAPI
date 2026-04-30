using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Maps to users table.
    /// Key columns: username (join key), name (vehicle plate), 
    ///              password, password_hash, password_salt, active
    /// </summary>
    [Table("users")]
    public class AppUser
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("username")]
        [StringLength(255)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Vehicle plate number, e.g. "KHR/55001 (CR-Khor Fakkan)".
        /// Also used as display name — filter vehicleId matches against this.
        /// </summary>
        [Column("name")]
        [StringLength(30)]
        public string? Name { get; set; }

        [Column("surname")]
        [StringLength(30)]
        public string? Surname { get; set; }

        /// <summary>MD5 password hash (legacy, char 32)</summary>
        [Column("password")]
        [StringLength(32)]
        public string? Password { get; set; }

        [Column("email")]
        [StringLength(128)]
        public string? Email { get; set; }

        /// <summary>1 = active, must be 1 to allow login</summary>
        [Column("active")]
        public short Active { get; set; } = 1;

        [Column("description")]
        [StringLength(255)]
        public string? Description { get; set; }

        /// <summary>Newer salted hash</summary>
        [Column("password_hash")]
        [StringLength(28)]
        public string? PasswordHash { get; set; }

        /// <summary>Salt for password_hash</summary>
        [Column("password_salt")]
        [StringLength(28)]
        public string? PasswordSalt { get; set; }
    }
}
