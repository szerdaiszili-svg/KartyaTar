using KartyaTar2.ViewModels;

namespace KartyaTar2.Views;

// Az oldal "mögötti" kód: csak annyit tesz, hogy a ViewModelt rákapcsolja az oldalra,
// és minden megjelenéskor frissíti a listát.
public partial class CardListPage : ContentPage
{
    private readonly CardListViewModel _vm;

    public CardListPage(CardListViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try { await _vm.LoadCommand.ExecuteAsync(null); }
        catch (Exception ex) { await DisplayAlert("Hiba", ex.Message, "OK"); }
    }
}
