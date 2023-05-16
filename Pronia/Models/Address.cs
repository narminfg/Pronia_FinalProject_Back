using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Pronia.Models
{
    public class Address : BaseEntity
    {

        [AllowNull]
        public bool IsMain { get; set; }
        public string? Country { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? Street { get; set; }
        [StringLength(100)]
        public string? State { get; set; }
        [StringLength(100)]
        public string? ZipCode { get; set; }
        [StringLength(100)]
        public string? PhoneNumber { get; set; }
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
    }
}
