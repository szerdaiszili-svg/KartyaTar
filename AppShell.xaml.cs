using KartyaTar2.Views;

namespace KartyaTar2;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // A szerkesztő oldal nem fül, hanem "ráhajtott" oldal: ide regisztráljuk az útvonalát
        Routing.RegisterRoute(Routes.CardEdit, typeof(CardEditPage));
    }
}
