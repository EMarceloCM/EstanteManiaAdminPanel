using EstanteManiaWebAssembly.Models.DTO_s;
using EstanteManiaWebAssembly.Pages.Books;
using EstanteManiaWebAssembly.Services.API.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace EstanteManiaWebAssembly.Services.API
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ILogger<CategoryService> _logger;
        private readonly JsonSerializerOptions _options;

        private const string apiEndpoint = "/api/category/";

        private CategoryDTO? _category;
        private IEnumerable<CategoryDTO>? _categories;
        private PaginationCategoryResponseDTO _responsePaginationDTO;
        private int TotalPages;

        public CategoryService(ILogger<CategoryService> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var result = await httpClient.GetFromJsonAsync<List<CategoryDTO>>(apiEndpoint + "get-all-categories");
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get categories: {apiEndpoint} " + ex.Message);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var response = await httpClient.GetAsync(apiEndpoint + id);
                
                if (response.IsSuccessStatusCode)
                {
                    _category = await response.Content.ReadFromJsonAsync<CategoryDTO>();
                    return _category;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while trying to get category by id = {id} - {message}");
                    throw new Exception($"Status code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get category by id = {id} \n\n " + ex.Message);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<CategoryWithBooksDTO?> GetCategoryWithBooksAsync(int categoryId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var response = await httpClient.GetAsync(apiEndpoint + "category-with-books/" + categoryId);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CategoryWithBooksDTO?>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while trying to get books by category id = {categoryId} - {message}");
                    throw new Exception($"Status code: {response.StatusCode} - {message}");
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get books by category id = {categoryId} \n\n " + ex.Message);
                throw;
            }
        }

        public async Task<CategoryDTO?> CreateCategoryAsync(CategoryDTO categoryDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            StringContent content = new(JsonSerializer.Serialize(categoryDTO), System.Text.Encoding.UTF8, "application/json");

            try
            {
                using (var response = await httpClient.PostAsync(apiEndpoint, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStreamAsync();
                        _category = await JsonSerializer.DeserializeAsync<CategoryDTO>(apiResponse, _options);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new UnauthorizedAccessException();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return _category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");

            using (var response = await httpClient.DeleteAsync(apiEndpoint + id))
            {
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            return false;
        }

        public Task<IEnumerable<CategoryDTO>> FindCategory(string criterio)
        {
            throw new NotImplementedException();
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            CategoryDTO categoryUpdated = new();

            using (var response = await httpClient.PutAsJsonAsync(apiEndpoint + $"{categoryId}", categoryDTO))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    categoryUpdated = await JsonSerializer.DeserializeAsync<CategoryDTO>(apiResponse, _options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return categoryUpdated!;
        }

        public async Task<PaginationCategoryResponseDTO> GetCategoryWithPagination(int pageNumber, int pageSize)
        {
            var parameters = $"?PageNumber={pageNumber}&PageSize={pageSize}";
            var apiUri = apiEndpoint + parameters;

            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var httpResponse = await httpClient.GetAsync(apiUri);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStr = await httpResponse.Content.ReadAsStringAsync();
                    _responsePaginationDTO = JsonSerializer.Deserialize<PaginationCategoryResponseDTO>(responseStr, _options);
                    TotalPages = _responsePaginationDTO!.TotalPages;
                    _categories = _responsePaginationDTO.Categories;
                }
                else
                {
                    _logger.LogWarning("Request falhou com status: " + httpResponse.StatusCode);
                }

                return _responsePaginationDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {apiEndpoint} + {ex.Message}");
                throw new UnauthorizedAccessException();
            }
        }
    }
}