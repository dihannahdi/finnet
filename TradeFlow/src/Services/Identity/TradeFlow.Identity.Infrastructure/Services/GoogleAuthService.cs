using System.Net.Http.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TradeFlow.Common.Application;
using TradeFlow.Identity.Application.Commands;
using TradeFlow.Identity.Domain.Entities;
using TradeFlow.Identity.Domain.Interfaces;

namespace TradeFlow.Identity.Infrastructure.Services;

public class GoogleAuthService : IRequestHandler<GoogleLoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<GoogleAuthService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<GoogleUserInfo>(
                $"https://oauth2.googleapis.com/tokeninfo?id_token={request.IdToken}", cancellationToken);

            if (response is null || string.IsNullOrEmpty(response.Email))
                return Result<AuthResponse>.Failure("Invalid Google token.");

            var user = await _userRepository.GetByGoogleIdAsync(response.Sub, cancellationToken);

            if (user is null)
            {
                user = await _userRepository.GetByEmailAsync(response.Email, cancellationToken);
                if (user is null)
                {
                    user = User.CreateFromGoogle(response.Email, response.Name ?? response.Email, response.Sub, response.Picture);
                    await _userRepository.AddAsync(user, cancellationToken);
                }
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);

            return Result<AuthResponse>.Success(new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddHours(1)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google authentication failed");
            return Result<AuthResponse>.Failure("Google authentication failed.");
        }
    }

    private record GoogleUserInfo
    {
        public string Sub { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string? Name { get; init; }
        public string? Picture { get; init; }
    }
}
