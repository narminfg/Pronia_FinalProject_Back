using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Basket:BaseEntity
    {
        public string? Image { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        [Column(TypeName = "money")]
        public double? Price { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
    }
}
