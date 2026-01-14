using System.ComponentModel.DataAnnotations;

namespace APIEcommerce.Models.Dtos {

    public class UserLoginDto {

        [Required(ErrorMessage = "El campo username es requerido")]
        public string? username { get; set; }


        [Required(ErrorMessage = "El campo password es requerido")]
        public string? password { get; set; }

    }

}
