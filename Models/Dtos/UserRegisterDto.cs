namespace APIEcommerce.Models.Dtos {

    public class UserRegisterDto {

        public string? id { get; set; }

        public required string username { get; set; }

        public required string password { get; set; }

        public string? name { get; set; }

        public string? role { get; set; }

    }

}

