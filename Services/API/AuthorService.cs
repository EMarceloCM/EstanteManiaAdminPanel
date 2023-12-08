using EstanteManiaWebAssembly.Models.DTO_s;
using EstanteManiaWebAssembly.Pages.Categories;
using EstanteManiaWebAssembly.Services.API.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace EstanteManiaWebAssembly.Services.API
{
    public class AuthorService : IAuthorService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ILogger<AuthorService> _logger;
        private JsonSerializerOptions _options;

        private const string apiEndpoint = "/api/author/";

        private AuthorDTO? _author;
        private IEnumerable<AuthorDTO>? _authors;
        private PaginationAuthorResponseDTO _responsePaginationDTO;
        private int TotalPages;

        public AuthorService(IHttpClientFactory httpClientFactory, ILogger<AuthorService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<AuthorDTO> GetAuthorByIdAsync(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var response = await httpClient.GetAsync(apiEndpoint + id);

                if (response.IsSuccessStatusCode)
                {
                    _author = await response.Content.ReadFromJsonAsync<AuthorDTO>();
                    return _author!;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while trying to get author by id = {id} - {message}");
                    throw new Exception($"Status code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get author by id = {id} \n\n " + ex.Message);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<List<AuthorDTO>> GetAuthorsAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var result = await httpClient.GetFromJsonAsync<List<AuthorDTO>>(apiEndpoint + "get-all-authors");
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get authors: {apiEndpoint} " + ex.Message);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            StringContent content = new(JsonSerializer.Serialize(authorDTO), System.Text.Encoding.UTF8, "application/json");

            try
            {
                using (var response = await httpClient.PostAsync(apiEndpoint, content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStreamAsync();
                        _author = await JsonSerializer.DeserializeAsync<AuthorDTO>(apiResponse, _options);
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

            return _author;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
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

        public Task<IEnumerable<AuthorDTO>> FindAuthor(string criterio)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthorDTO> UpdateAuthorAsync(int authorId, AuthorDTO authorDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            AuthorDTO authorUpdated = new();

            using (var response = await httpClient.PutAsJsonAsync(apiEndpoint + $"{authorId}", authorDTO))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    authorUpdated = await JsonSerializer.DeserializeAsync<AuthorDTO>(apiResponse, _options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return authorUpdated!;
        }

        public async Task<PaginationAuthorResponseDTO> GetAuthorWithPagination(int pageNumber, int pageSize)
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
                    _responsePaginationDTO = JsonSerializer.Deserialize<PaginationAuthorResponseDTO>(responseStr, _options);
                    TotalPages = _responsePaginationDTO!.TotalPages;
                    _authors = _responsePaginationDTO.Authors;
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
