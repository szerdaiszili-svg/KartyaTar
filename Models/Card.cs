using SQLite;

namespace KartyaTar2.Models;


public class Card
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Series { get; set; } = "";
    public int PriceFt { get; set; }
    public string PhotoPath { get; set; } = "";

    [Ignore] public string PriceText => $"{PriceFt:N0} Ft";
    [Ignore] public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);
}
