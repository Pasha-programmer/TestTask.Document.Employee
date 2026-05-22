using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using TestTask.Document.Employee.Contract.Interfaces.IdentityConnector;
using TestTask.Document.Employee.Infrastructure.Configuration;

namespace TestTask.Document.Employee.Infrastructure.Services.Keycloak;

internal class AuthenticatedHttpClientHandler(
    IIdentityConnectorService identityConnectorService,
    IOptions<AdminCredentialsConfiguration> adminCredentials
    ) : DelegatingHandler
{
    private readonly IIdentityConnectorService _identityConnectorService = identityConnectorService;
    private readonly IOptions<AdminCredentialsConfiguration> _adminCredentials = adminCredentials;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _identityConnectorService.GetUserAuthorizationToken(_adminCredentials.Value.Login, _adminCredentials.Value.Password, cancellationToken)
            ?? throw new HttpRequestException("The Keycloak administrator authorization token could not be obtained.");

        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token.AccessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
