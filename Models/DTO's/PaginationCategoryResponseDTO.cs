namespace EstanteManiaWebAssembly.Models.DTO_s
{
    public class PaginationCategoryResponseDTO
    {
        public List<CategoryDTO>? Categories { get; set; }
        public int TotalPages { get; set; }
    }
}
