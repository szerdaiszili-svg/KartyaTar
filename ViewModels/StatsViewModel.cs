using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KartyaTar2.Models;
using KartyaTar2.Services;

namespace KartyaTar2.ViewModels;

public partial class StatsViewModel : ObservableObject
{
    private const string AllSeries = "Összes sorozat";

    private readonly DatabaseService _db;
    private List<Card> _all = new();

    public StatsViewModel(DatabaseService db)
    {
        _db = db;
    }

    public ObservableCollection<string> SeriesOptions { get; } = new();
    public ObservableCollection<Card> Cards { get; } = new();

    [ObservableProperty] private string? selectedSeries = AllSeries;
    [ObservableProperty] private string minPriceText = "";

    [ObservableProperty] private string countText = "0 db";
    [ObservableProperty] private string totalText = "0 Ft";
    [ObservableProperty] private string topText = "-";
    [ObservableProperty] private string hintText = "";

    partial void OnSelectedSeriesChanged(string? value) => ApplyFilter();
    partial void OnMinPriceTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            _all = await _db.GetCardsAsync();

            var current = SelectedSeries ?? AllSeries;
            SeriesOptions.Clear();
            SeriesOptions.Add(AllSeries);
            foreach (var s in _all.Select(c => c.Series).Where(s => !string.IsNullOrWhiteSpace(s))
                                  .Distinct().OrderBy(s => s))
                SeriesOptions.Add(s);

            SelectedSeries = SeriesOptions.Contains(current) ? current : AllSeries;
            ApplyFilter();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK");
        }
    }
    
    private void ApplyFilter()
    {
        IEnumerable<Card> result = _all;

        if (!string.IsNullOrEmpty(SelectedSeries) && SelectedSeries != AllSeries)
            result = result.Where(c => c.Series == SelectedSeries);

        HintText = "";
        var text = MinPriceText?.Replace(" ", "") ?? "";
        if (text.Length > 0)
        {
            if (int.TryParse(text, out var min) && min >= 0)
                result = result.Where(c => c.PriceFt >= min);
            else
                HintText = "Az ár egy egész szám legyen (forintban).";
        }

        var list = result.OrderByDescending(c => c.PriceFt).ToList();

        Cards.Clear();
        foreach (var c in list) Cards.Add(c);

        CountText = $"{list.Count} db";
        TotalText = $"{list.Sum(c => (long)c.PriceFt):N0} Ft";
        var top = list.FirstOrDefault();
        TopText = top != null && top.PriceFt > 0 ? $"{top.Name} ({top.PriceFt:N0} Ft)" : "-";
    }

    [RelayCommand]
    private async Task EditAsync(Card? card)
    {
        if (card == null) return;
        try { await Shell.Current.GoToAsync($"{Routes.CardEdit}?Id={card.Id}"); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Hiba", ex.Message, "OK"); }
    }
}
