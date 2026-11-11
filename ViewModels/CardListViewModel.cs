using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KartyaTar2.Models;
using KartyaTar2.Services;

namespace KartyaTar2.ViewModels;

// VIEWMODEL = "a pincér". Nem ő főz (adatbázis), és nem ő az asztal (a képernyő),
// hanem ő viszi az információt a kettő között, és ő intézi, mi történik egy gombnyomásra.
public partial class CardListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    // Az a lista, amit a képernyő mutat. Ha ebben változik valami, a képernyő magától frissül.
    public ObservableCollection<Card> Cards { get; } = new();

    public CardListViewModel(DatabaseService db)
    {
        _db = db;
    }

    // [RelayCommand] = ebből a metódusból "gombra köthető parancs" lesz (LoadCommand)
    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            var list = await _db.GetCardsAsync();
            Cards.Clear();
            foreach (var card in list)
                Cards.Add(card);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", $"Nem sikerült betölteni: {ex.Message}", "OK");
        }
    }

    // "Új kártya" gomb: megnyitjuk a szerkesztő oldalt, üresen
    [RelayCommand]
    private async Task AddAsync()
    {
        try { await Shell.Current.GoToAsync(Routes.CardEdit); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK"); }
    }

    // Egy kártyára koppintás: megnyitjuk a szerkesztőt, és átadjuk, melyik kártya az (Id)
    [RelayCommand]
    private async Task EditAsync(Card? card)
    {
        if (card == null) return;
        try { await Shell.Current.GoToAsync($"{Routes.CardEdit}?Id={card.Id}"); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK"); }
    }
}
