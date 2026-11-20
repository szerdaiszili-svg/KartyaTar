using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KartyaTar2.Models;
using KartyaTar2.Services;

namespace KartyaTar2.ViewModels;

public partial class CardListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ObservableCollection<Card> Cards { get; } = new();

    public CardListViewModel(DatabaseService db)
    {
        _db = db;
    }

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

    [RelayCommand]
    private async Task AddAsync()
    {
        try { await Shell.Current.GoToAsync(Routes.CardEdit); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK"); }
    }

    [RelayCommand]
    private async Task EditAsync(Card? card)
    {
        if (card == null) return;
        try { await Shell.Current.GoToAsync($"{Routes.CardEdit}?Id={card.Id}"); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK"); }
    }
}
