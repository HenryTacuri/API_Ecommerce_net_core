using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIEcommerce.Models {

    [Table("Product")]
    public class Product {
        
        [Key]
        public int id { get; set; }


        [Required]
        [Column("name", TypeName = "varchar(200)")]
        public string name { get; set; } = string.Empty;


        [Column("description", TypeName = "varchar(200)")]
        public string description { get; set; } = string.Empty;


        [Range(0, double.MaxValue)]
        [Column("price", TypeName = "decimal(10, 2)")]
        public decimal price { get; set; }

        
        [Column("img_url", TypeName = "varchar(400)")]
        public string? imgUrl { get; set; }


        [Column("img_url_local", TypeName = "varchar(400)")]
        public string? imgUrlLocal { get; set; }


        [Required]
        [Column("sku", TypeName = "varchar(100)")]
        public string sku { get; set; } = string.Empty;


        [Range(0, int.MaxValue)]
        [Column("stock", TypeName = "decimal(10, 2)")]
        public int stock { get; set; }

        
        [Column("creation_date", TypeName = "datetime")]
        public DateTime creationDate { get; set; } = DateTime.Now;


        [Column("update_date", TypeName = "datetime")]
        public DateTime? updateDate { get; set; } = null;


        //Relacion con el modelo category
        public int category_id { get; set; }

        [ForeignKey("category_id")]
        public required Category category { get; set; }

    }

}
