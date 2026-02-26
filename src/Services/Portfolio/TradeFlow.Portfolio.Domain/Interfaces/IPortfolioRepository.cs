using TradeFlow.Portfolio.Domain.Entities;

namespace TradeFlow.Portfolio.Domain.Interfaces;

public interface IPortfolioRepository
{
    Task<UserPortfolio?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<UserPortfolio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<UserPortfolio> AddAsync(UserPortfolio portfolio, CancellationToken ct = default);
    Task UpdateAsync(UserPortfolio portfolio, CancellationToken ct = default);
    Task<IReadOnlyList<Trade>> GetTradeHistoryAsync(Guid portfolioId, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetTradeCountAsync(Guid portfolioId, CancellationToken ct = default);
}
