namespace EstanteManiaWebAssembly.Services.API.Interfaces
{
    public interface IFileUploadService
    {
        Task<HttpResponseMessage> UploadFileAsync(string enpoint, MultipartFormDataContent content);
    }
}
