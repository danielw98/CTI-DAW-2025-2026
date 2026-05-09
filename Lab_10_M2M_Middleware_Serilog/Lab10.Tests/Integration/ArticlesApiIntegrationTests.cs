using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Lab10.DTOs;

namespace Lab10.Tests.Integration;

public class ArticlesApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    private const string AdminEmail = "admin@newsportal.com";
    private const string AdminPassword = "Admin@123";

    public ArticlesApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Seed-ul aplicatiei (admin + articole) ruleaza la startup via Program.cs.
        // Aici adaugam user-ul non-admin necesar pentru testele de ownership.
        await _factory.SeedExtraTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAll_ReturnsOkAndJsonArray()
    {
        var response = await _client.GetAsync("/api/articlesapi");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var articles = await response.Content.ReadFromJsonAsync<List<ArticleDto>>();
        Assert.NotNull(articles);
    }

    [Fact]
    public async Task GetById_ForNonexistentId_Returns404()
    {
        var response = await _client.GetAsync("/api/articlesapi/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutToken_Returns401()
    {
        var dto = new CreateArticleDto("Test Title", "Continut destul de lung pentru validare", 1);

        var response = await _client.PostAsJsonAsync("/api/articlesapi", dto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithValidToken_Returns201AndArticleIsReadableAfterwards()
    {
        var token = await LoginAndGetTokenAsync(AdminEmail, AdminPassword);

        var dto = new CreateArticleDto(
            "Titlu din integration test",
            "Continut suficient de lung ca sa treaca validarea MinLength(20)",
            1);

        var createResponse = await SendAuthorizedAsync(HttpMethod.Post, "/api/articlesapi", token, dto);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdDto = await createResponse.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.NotNull(createdDto);
        Assert.Equal("Titlu din integration test", createdDto!.Title);

        // Round-trip: GET pe Location trebuie sa intoarca acelasi articol
        var getResponse = await _client.GetAsync(createResponse.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<ArticleDto>();
        Assert.Equal(createdDto.Id, fetched!.Id);
    }

    [Fact]
    public async Task Update_AsNonOwnerNonAdmin_Returns403()
    {
        // Admin creeaza articolul
        var adminToken = await LoginAndGetTokenAsync(AdminEmail, AdminPassword);
        var createDto = new CreateArticleDto(
            "Articol al admin-ului",
            "Continut suficient de lung pentru validarea MinLength(20)",
            1);
        var createResp = await SendAuthorizedAsync(HttpMethod.Post, "/api/articlesapi", adminToken, createDto);
        createResp.EnsureSuccessStatusCode();
        var created = await createResp.Content.ReadFromJsonAsync<ArticleDto>();

        // User-ul regular incearca sa-l modifice
        var userToken = await LoginAndGetTokenAsync(SeedTestData.RegularUserEmail, SeedTestData.RegularUserPassword);
        var updateDto = new UpdateArticleDto(
            "Incerc sa suprascriu titlul",
            "Continut suficient de lung pentru validarea MinLength(20)",
            1);

        var response = await SendAuthorizedAsync(HttpMethod.Put, $"/api/articlesapi/{created!.Id}", userToken, updateDto);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> LoginAndGetTokenAsync(string email, string password)
    {
        var loginDto = new LoginDto(email, password);
        var response = await _client.PostAsJsonAsync("/api/authapi/login", loginDto);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return payload!.Token;
    }

    private async Task<HttpResponseMessage> SendAuthorizedAsync<T>(HttpMethod method, string url, string token, T body)
    {
        var request = new HttpRequestMessage(method, url)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await _client.SendAsync(request);
    }

    private record LoginResponse(string Token, int ExpiresIn);
}
