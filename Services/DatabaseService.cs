using KartyaTar2.Models;
using SQLite;

namespace KartyaTar2.Services;


public class DatabaseService : IDatabaseService
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_db != null) return _db;

        var path = Path.Combine(FileSystem.AppDataDirectory, "kartyatar2.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Card>();
        return _db;
    }

    public async Task<List<Card>> GetCardsAsync()
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Card>().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Card?> GetCardAsync(int id)
    {
        var db = await GetDatabaseAsync();
        return await db.Table<Card>().Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveCardAsync(Card card)
    {
        var db = await GetDatabaseAsync();
        if (card.Id != 0) await db.UpdateAsync(card);
        else await db.InsertAsync(card);
    }

    public async Task DeleteCardAsync(Card card)
    {
        var db = await GetDatabaseAsync();
        await db.DeleteAsync(card);
    }
}
