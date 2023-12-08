namespace EstanteManiaWebAssembly.Models.DTO_s
{
    public class AuthorWithBooksDTO
    {
        public AuthorDTO? Author { get; set; }
        public IEnumerable<BookDTO>? Books { get; set; }
    }
}
