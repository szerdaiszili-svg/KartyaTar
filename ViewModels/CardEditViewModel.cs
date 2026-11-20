using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KartyaTar2.Models;
using KartyaTar2.Services;

namespace KartyaTar2.ViewModels;

[QueryProperty(nameof(CardIdText), "Id")]
public partial class CardEditViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly PhotoService _photos;
    private Card _card = new();
    private bool _loaded;

    public CardEditViewModel(DatabaseService db, PhotoService photos)
    {
        _db = db;
        _photos = photos;
    }

    public string CardIdText { get; set; } = "";

    
    [ObservableProperty] private string title = "Új kártya";
    [ObservableProperty] private bool isExisting;
    [ObservableProperty] private string name = "";
    [ObservableProperty] private string series = "";
    [ObservableProperty] private string priceText = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPhoto))]
    private string photoPath = "";

    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);

    private static Task ShowError(string message) => Shell.Current.DisplayAlert("Hiba", message, "OK");

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_loaded) return;
        try
        {
            if (int.TryParse(CardIdText, out var id) && id > 0)
            {
                var card = await _db.GetCardAsync(id);
                if (card != null)
                {
                    _card = card;
                    IsExisting = true;
                    Title = "Kártya szerkesztése";
                    Name = card.Name;
                    Series = card.Series;
                    PriceText = card.PriceFt > 0 ? card.PriceFt.ToString() : "";
                    PhotoPath = card.PhotoPath;
                }
            }
            _loaded = true;
        }
        catch (Exception ex)
        {
            await ShowError($"Nem sikerült betölteni: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task TakePhotoAsync()
    {
        try
        {
            var path = await _photos.TakePhotoAsync();
            if (path != null) PhotoPath = path;
        }
        catch (Exception ex) { await ShowError(ex.Message); }
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        try
        {
            var path = await _photos.PickPhotoAsync();
            if (path != null) PhotoPath = path;
        }
        catch (Exception ex) { await ShowError(ex.Message); }
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowError("Előbb add meg a kártya nevét.");
            return;
        }
        try
        {
            var text = $"{Name.Trim()}\nSorozat: {Series.Trim()}\nÁr: {PriceText} Ft";
            await Share.Default.RequestAsync(new ShareTextRequest { Title = "Kártya megosztása", Text = text });
        }
        catch (Exception ex) { await ShowError($"A megosztás nem sikerült: {ex.Message}"); }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowError("A név megadása kötelező.");
            return;
        }

        int price = 0;
        if (!string.IsNullOrWhiteSpace(PriceText) && (!int.TryParse(PriceText.Replace(" ", ""), out price) || price < 0))
        {
            await ShowError("Az ár egy egész szám legyen (forintban).");
            return;
        }

        _card.Name = Name.Trim();
        _card.Series = Series.Trim();
        _card.PriceFt = price;
        _card.PhotoPath = PhotoPath;

        try
        {
            await _db.SaveCardAsync(_card);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await ShowError($"A mentés nem sikerült: {ex.Message}"); }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (!IsExisting) return;

        bool ok = await Shell.Current.DisplayAlert("Törlés", $"Biztosan törlöd: {_card.Name}?", "Igen", "Nem");
        if (!ok) return;

        try
        {
            await _db.DeleteCardAsync(_card);
            PhotoService.DeleteFile(_card.PhotoPath);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex) { await ShowError($"A törlés nem sikerült: {ex.Message}"); }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        try { await Shell.Current.GoToAsync(".."); }
        catch (Exception ex) { await ShowError(ex.Message); }
    }
}
