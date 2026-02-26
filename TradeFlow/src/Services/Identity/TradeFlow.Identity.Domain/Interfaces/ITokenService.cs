namespace TradeFlow.Identity.Domain.Interfaces;

using TradeFlow.Identity.Domain.Entities;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateRefreshToken(string token);
}
