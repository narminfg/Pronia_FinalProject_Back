using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Product:BaseEntity
    {
        [StringLength(255)]
        public string? Title { get; set; }
        public int ReviewCount { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        public int Count { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsLatest { get; set; }
        public bool IsNew { get; set; }

        [StringLength(255)]
        
        public string? MainImage { get; set; }
        [StringLength(255)]
        
        public string? HoverImage { get; set; }


        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }

        public IEnumerable<Review>? Reviews { get; set; }

        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }

        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IFormFile? HoverFile { get; set; }
    }
}
