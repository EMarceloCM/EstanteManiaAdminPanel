namespace EstanteManiaWebAssembly.Models.DTO_s
{
    public class CategoryWithBooksDTO
    {
        public CategoryDTO? Category { get; set; }
        public IEnumerable<BookDTO>? Books { get; set; }
    }
}
