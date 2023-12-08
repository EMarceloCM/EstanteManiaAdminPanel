using EstanteManiaWebAssembly.Models.DTO_s;

namespace EstanteManiaWebAssembly.Services.API.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDTO>> GetAuthorsAsync();
        Task<AuthorDTO> GetAuthorByIdAsync(int id);
        Task<PaginationAuthorResponseDTO> GetAuthorWithPagination(int pageNumber, int pageSize);
        Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO);
        Task<AuthorDTO> UpdateAuthorAsync(int authorId, AuthorDTO authorDTO);
        Task<bool> DeleteAuthorAsync(int id);
        Task<IEnumerable<AuthorDTO>> FindAuthor(string criterio);
    }
}
