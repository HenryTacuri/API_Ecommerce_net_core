namespace APIEcommerce.Models.Dtos.Responses {

    public class PaginationResponse<T> {
        
        public int pageNumber { get; set; }

        public int pageSize { get; set; }

        public int totalPages { get; set; }

        public ICollection<T> items { get; set; } = new List<T>();

    }

}

