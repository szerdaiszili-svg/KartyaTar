using KartyaTar2.Models;

namespace KartyaTar2.Services
{
    public interface IDatabaseService
    {
        Task<List<Card>> GetCardsAsync();
        Task<Card?> GetCardAsync(int id);
        Task SaveCardAsync(Card card);
        Task DeleteCardAsync(Card card);
    }
}
