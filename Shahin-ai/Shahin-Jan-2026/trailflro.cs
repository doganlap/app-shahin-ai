
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Volo.Abp.Modularity;
using Xunit;
using Shouldly;
using Newtonsoft.Json.Linq;

public class TrialFlowTests : IClassFixture<WebApplicationFactory<GrcMvc.HttpApi.Host.GrcMvcHttpApiHostModule>>
{
    private readonly HttpClient _client;

    public TrialFlowTests(WebApplicationFactory<GrcMvc.HttpApi.Host.GrcMvcHttpApiHostModule> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TrialFlow_Should_Create_Tenant_Admin_Login_And_Redirect()
    {
        var registrationPayload = new
        {
            OrganizationName = "TestCorp Ltd",
            FullName = "Alice Admin",
            Email = "alice.testcorp@example.com",
            Password = "SuperSecure123!",
            AcceptTerms = true
        };

        var response = await _client.PostAsJsonAsync("/trial/register", registrationPayload);
        response.EnsureSuccessStatusCode();

        var result = JObject.Parse(await response.Content.ReadAsStringAsync());
        result["success"]!.Value<bool>().ShouldBeTrue();

        var tenantName = result["tenantName"]!.ToString();
        var loginUrl = result["loginUrl"]!.ToString();

        var loginPayload = new
        {
            UserNameOrEmailAddress = registrationPayload.Email,
            Password = registrationPayload.Password,
            TenancyName = tenantName
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/account/login", loginPayload);
        loginResponse.EnsureSuccessStatusCode();

        var loginResult = JObject.Parse(await loginResponse.Content.ReadAsStringAsync());
        loginResult["accessToken"]!.ToString().ShouldNotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult["accessToken"]!.ToString());

        var onboardingResponse = await _client.GetAsync("/onboarding/wizard/fast-start");
        onboardingResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }
}
