using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIEcommerce.Models {

    [Table("User")]
    public class User {

        [Key]
        public int id { get; set; }

        
        [Column("name", TypeName = "varchar(200)")]
        public string? name { get; set; }


        [Column("username", TypeName = "varchar(200)")]
        public string username { get; set; } = string.Empty;


        [Column("password", TypeName = "varchar(200)")]
        public string? password { get; set; }


        [Column("role", TypeName = "varchar(50)")]
        public string? role { get; set; }

    }

}

