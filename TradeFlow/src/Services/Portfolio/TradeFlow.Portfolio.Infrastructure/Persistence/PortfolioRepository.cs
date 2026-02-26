using Microsoft.EntityFrameworkCore;
using TradeFlow.Portfolio.Domain.Entities;
using TradeFlow.Portfolio.Domain.Interfaces;

namespace TradeFlow.Portfolio.Infrastructure.Persistence;

public class PortfolioRepository : IPortfolioRepository
{
    private readonly PortfolioDbContext _context;
    public PortfolioRepository(PortfolioDbContext context) => _context = context;

    public async Task<UserPortfolio?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Portfolios
            .Include(p => p.Positions)
            .Include(p => p.Trades.OrderByDescending(t => t.ExecutedAt).Take(10))
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task<UserPortfolio?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Portfolios
            .Include(p => p.Positions)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<UserPortfolio> AddAsync(UserPortfolio portfolio, CancellationToken ct = default)
    {
        await _context.Portfolios.AddAsync(portfolio, ct);
        await _context.SaveChangesAsync(ct);
        return portfolio;
    }

    public async Task UpdateAsync(UserPortfolio portfolio, CancellationToken ct = default)
    {
        _context.Portfolios.Update(portfolio);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Trade>> GetTradeHistoryAsync(Guid portfolioId, int page, int pageSize, CancellationToken ct = default) =>
        await _context.Trades
            .Where(t => t.PortfolioId == portfolioId)
            .OrderByDescending(t => t.ExecutedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> GetTradeCountAsync(Guid portfolioId, CancellationToken ct = default) =>
        await _context.Trades.CountAsync(t => t.PortfolioId == portfolioId, ct);
}
