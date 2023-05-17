using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        
        public bool IsDeleted { get; set; }
        [StringLength(1000)]
        public string? CreatedBy { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        [StringLength(1000)]
        public string? UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        [StringLength(1000)]
        public string? DeletedBy { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
