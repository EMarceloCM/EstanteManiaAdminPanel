using EstanteManiaWebAssembly.Services.API.Interfaces;

namespace EstanteManiaWebAssembly.Services.API
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ILogger<FileUploadService> _logger;

        public FileUploadService(IHttpClientFactory httpClientFactory, ILogger<FileUploadService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string enpoint, MultipartFormDataContent content)
        {
            HttpResponseMessage responseMessage = null;
            try
            {
                var httClient = _httpClientFactory.CreateClient("EstanteManiaApi");
                responseMessage = await httClient.PostAsync(enpoint, content);
                return responseMessage;
            }
            catch (Exception)
            {
                _logger.LogInformation($"Error while sending file. {enpoint}");
                throw;
            }
        }
    }
}
