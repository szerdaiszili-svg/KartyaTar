using KartyaTar2.Models;
using SQLite;

namespace KartyaTar2.Services;

// SERVICE = "a raktár". Itt van minden, ami az adatbázissal kapcsolatos:
// kártya mentése, kikeresése, törlése. A többi rész nem tud az SQLite-ról, csak ezt kéri meg.
public class DatabaseService
{
    private SQLiteAsyncConnection? _db;

    // Az adatbázis fájlt az első használatkor nyitjuk meg, és ekkor hozzuk létre a táblát is.
    private async Task<SQLiteAsyncConnection> GetDbAsync()
    {
        if (_db != null) return _db;

        var path = Path.Combine(FileSystem.AppDataDirectory, "kartyatar2.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Card>();
        return _db;
    }

    // R = Read (olvasás): az összes kártya névsorrendben
    public async Task<List<Card>> GetCardsAsync()
    {
        var db = await GetDbAsync();
        return await db.Table<Card>().OrderBy(c => c.Name).ToListAsync();
    }

    // R = Read: egyetlen kártya az Id-ja alapján
    public async Task<Card?> GetCardAsync(int id)
    {
        var db = await GetDbAsync();
        return await db.Table<Card>().Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    // C = Create (új) és U = Update (módosítás): ha még nincs Id, akkor új, különben módosítjuk
    public async Task SaveCardAsync(Card card)
    {
        var db = await GetDbAsync();
        if (card.Id != 0) await db.UpdateAsync(card);
        else await db.InsertAsync(card);
    }

    // D = Delete (törlés)
    public async Task DeleteCardAsync(Card card)
    {
        var db = await GetDbAsync();
        await db.DeleteAsync(card);
    }
}
