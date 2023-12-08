using EstanteManiaWebAssembly.Models.DTO_s;

namespace EstanteManiaWebAssembly.Services.API.Interfaces
{
    public interface IBookService
    {
        Task<List<BookDTO>> GetBooksAsync();
        Task<BookDTO> GetBookByIdAsync(int id);
        Task<IEnumerable<BookDTO>> GetBookByTitle(string title);
        Task<PaginationBookResponseDTO> GetBookWithPagination(int pageNumber, int pageSize);
        Task<BookWithAuthorDTO> GetBookWithAuthorAsync(int bookId);
        Task<BookDTO?> CreateBookAsync(BookDTO bookDTO);
        Task<BookDTO> UpdateBookAsync(int bookId, BookDTO bookDTO);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<BookDTO>> FindBook(string criterio);
    }
}
