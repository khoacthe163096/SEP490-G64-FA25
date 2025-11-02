using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.vn.fpt.edu.models
{
    [Table("type_component")]
    public class TypeComponent
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(255)]
        [Column("description")]
        public string Description { get; set; }

        [Column("is_active")] //non hard delete
        public bool IsActive { get; set; } = true;

        // navigation property
        public virtual ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
