using Microsoft.AspNetCore.Identity;

namespace APIEcommerce.Models {

    public class ApplicationUser : IdentityUser {

        public string? name { get; set; }

    }

}

