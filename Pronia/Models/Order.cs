using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Pronia.Enums;

namespace Pronia.Models
{
    public class Order:BaseEntity
    {
        public int No { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [Column(TypeName = "money")]
        public double TotalPrice { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurnName { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? Street { get; set; }
        [StringLength(100)]
        public string? ZipCode { get; set; }
        [StringLength(30)]
        public string? PhoneNumber { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        public OrderType Status { get; set; }
        public string? Comment { get; set; }
    }
}
