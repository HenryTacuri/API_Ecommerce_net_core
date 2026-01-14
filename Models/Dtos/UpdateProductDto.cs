
namespace APIEcommerce.Models.Dtos {

    public class UpdateProductDto {

        public string name { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;

        public decimal price { get; set; }

        public string? imgUrl { get; set; }

        public string? imgUrlLocal { get; set; }

        public IFormFile? image { get; set; }

        public string sku { get; set; } = string.Empty;

        public int stock { get; set; }

        public DateTime? updateDate { get; set; } = null;

        public int category_id { get; set; }

    }

}
