using System.ComponentModel.DataAnnotations;

public class CreateCategoryDto {

    [Required(ErrorMessage = "El nombre de la categoria es obligatorio")]
    [MaxLength(50, ErrorMessage = "El nombre de la categoria no puede tener mas de 50 caracteres")]
    [MinLength(3, ErrorMessage = "El nombre de la categoria no puede tener menos de 3 caracteres")]
    public string name { get; set; }

}

