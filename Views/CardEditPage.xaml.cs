using KartyaTar2.ViewModels;

namespace KartyaTar2.Views;

public partial class CardEditPage : ContentPage
{
    private readonly CardEditViewModel _vm;

    public CardEditPage(CardEditViewModel vm)
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
