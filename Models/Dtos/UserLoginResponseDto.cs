namespace APIEcommerce.Models.Dtos {

    public class UserLoginResponseDto {

        //Se permiten nulos en caso de no tener un login exitoso
        //public UserRegisterDto? user { get; set; }
        public UserDataDto? user { get; set; } //para identity

        public string? token { get; set; }

        //Mensajes de error o de exito
        public string? message { get; set; }
    }

}

