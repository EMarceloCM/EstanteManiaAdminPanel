using EstanteManiaWebAssembly.Models.DTO_s;

namespace EstanteManiaWebAssembly.Services.API.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryWithBooksDTO?> GetCategoryWithBooksAsync(int categoryId);
        Task<PaginationCategoryResponseDTO> GetCategoryWithPagination(int pageNumber, int pageSize);
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDTO);
        Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDTO);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryDTO>> FindCategory(string criterio);
    }
}
