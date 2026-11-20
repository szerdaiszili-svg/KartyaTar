using SQLite;

namespace KartyaTar2.Models;


public class Card
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Series { get; set; } = string.Empty;
    public int PriceFt { get; set; }
    public string PhotoPath { get; set; } = string.Empty;
}
