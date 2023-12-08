using EstanteManiaWebAssembly.Models.DTO_s;
using EstanteManiaWebAssembly.Services.API.Interfaces;
using System.Data;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;

namespace EstanteManiaWebAssembly.Services.API
{
    public class BookService : IBookService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _options;
        public ILogger<BookService> _logger;

        private const string apiEndpoint = "/api/book/";

        private BookDTO? _book;
        private IEnumerable<BookDTO>? _books;
        private PaginationBookResponseDTO _responsePaginationDTO;
        private int TotalPages;

        public BookService(IHttpClientFactory httpClientFactory, ILogger<BookService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        public async Task<PaginationBookResponseDTO> GetBookWithPagination(int pageNumber, int pageSize)
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
                    _responsePaginationDTO = JsonSerializer.Deserialize<PaginationBookResponseDTO>(responseStr, _options);
                    TotalPages = _responsePaginationDTO!.TotalPages;
                    _books = _responsePaginationDTO.Books;
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

        public async Task<BookDTO> GetBookByIdAsync(int id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var response = await httpClient.GetAsync(apiEndpoint + id);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<BookDTO>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while trying to get book by id = {id} - {message}");
                    throw new Exception("Status Code : " + response.StatusCode + " - " + message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get book by id = {id} - {ex.Message}");
                throw;
            }
        }

        public async Task<List<BookDTO>> GetBooksAsync()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var result = await httpClient.GetFromJsonAsync<List<BookDTO>>(apiEndpoint + "get-all-books");
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get books: {apiEndpoint} " + ex.Message);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<BookWithAuthorDTO> GetBookWithAuthorAsync(int bookId)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var result = await httpClient.GetFromJsonAsync<BookWithAuthorDTO>(apiEndpoint + "get-books-with-author/" + bookId);
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while trying to get books: {apiEndpoint} " + ex.StackTrace);
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<BookDTO?> CreateBookAsync(BookDTO bookDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            StringContent content = new (JsonSerializer.Serialize(bookDTO), System.Text.Encoding.UTF8, "application/json");

            //var authorId = bookDTO.AuthorId;
            var categoriesId = bookDTO.CategoryIds;
            var serializedCategoriesId = string.Join("&categoryIds=", categoriesId.Select(id => id.ToString()));

            try
            {
                using (var response = await httpClient.PostAsync(apiEndpoint + $"?categoryIds={serializedCategoriesId}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStreamAsync();
                        _book = await JsonSerializer.DeserializeAsync<BookDTO>(apiResponse, _options);
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

            return _book;
        }

        public async Task<bool> DeleteBookAsync(int id)
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

        public Task<IEnumerable<BookDTO>> FindBook(string criterio)
        {
            throw new NotImplementedException();
        }

        public async Task<BookDTO> UpdateBookAsync(int bookId, BookDTO bookDTO)
        {
            var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
            //var authorId = bookDTO.AuthorId;
            var categoriesId = bookDTO.CategoryIds;
            var serializedCategoriesId = string.Join("&categoryIds=", categoriesId.Select(id => id.ToString()));
            BookDTO bookUpdated = new();

            using (var response = await httpClient.PutAsJsonAsync(apiEndpoint + $"{bookId}/?categoryIds={serializedCategoriesId}", bookDTO))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();
                    bookUpdated = await JsonSerializer.DeserializeAsync<BookDTO>(apiResponse, _options);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return bookUpdated!;
        }

        public async Task<IEnumerable<BookDTO>> GetBookByTitle(string title)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                var response = await httpClient.GetAsync(apiEndpoint + "search/" + title);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Array.Empty<BookDTO>();
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error while get the book by title={title} - {message}");
                    throw new Exception($"Status code: {response.StatusCode} - {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while get the book by title={title} \n\n {ex.Message}");
                throw;
            }
        }
    }
}
