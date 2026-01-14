using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Category")]
public class Category {

    [Key]
    public int id { get; set; }

    [Required]
    [Column("name", TypeName = "varchar(200)")]
    public string name { get; set; }

    [Required]
    [Column("creation_date", TypeName = "datetime")]
    public DateTime creationDate { get; set; }


}

