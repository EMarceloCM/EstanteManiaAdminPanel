using Blazored.LocalStorage;
using EstanteManiaWebAssembly;
using EstanteManiaWebAssembly.Services.API;
using EstanteManiaWebAssembly.Services.API.Interfaces;
using EstanteManiaWebAssembly.Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("EstanteManiaApi", options => options.BaseAddress = new Uri("https://localhost:7146/"))
    .AddHttpMessageHandler<CustomHttpHandler>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<CustomHttpHandler>();
builder.Services.AddBlazorBootstrap();

await builder.Build().RunAsync();
