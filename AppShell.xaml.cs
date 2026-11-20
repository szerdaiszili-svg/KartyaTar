using KartyaTar2.Views;

namespace KartyaTar2;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Routes.CardEdit, typeof(CardEditPage));
    }
}
