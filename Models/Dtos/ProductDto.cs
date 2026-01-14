using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIEcommerce.Models.Dtos {

    public class ProductDto {

        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;

        public decimal price { get; set; }

        public string? imgUrl { get; set; }

        public string? imgUrlLocal { get; set; }

        public string sku { get; set; } = string.Empty;

        public int stock { get; set; }

        public DateTime creationDate { get; set; } = DateTime.Now;

        public DateTime? updateDate { get; set; } = null;

        public int category_id { get; set; }

        public string categoryName { get; set; } = string.Empty;

    }

}
