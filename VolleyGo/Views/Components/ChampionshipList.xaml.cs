using System.Windows.Input;

namespace VolleyGo.Views.Components;

public partial class ChampionshipList : ContentView
{
    public static readonly BindableProperty ShowMapForChampionshipCommandProperty =
        BindableProperty.Create(nameof(ShowMapForChampionshipCommand), typeof(ICommand), typeof(ChampionshipList));

    public ICommand ShowMapForChampionshipCommand
    {
        get => (ICommand)GetValue(ShowMapForChampionshipCommandProperty);
        set => SetValue(ShowMapForChampionshipCommandProperty, value);
    }

    public static readonly BindableProperty RegisterCommandProperty =
        BindableProperty.Create(nameof(RegisterCommand), typeof(ICommand), typeof(ChampionshipList));

    public ICommand RegisterCommand
    {
        get => (ICommand)GetValue(RegisterCommandProperty);
        set => SetValue(RegisterCommandProperty, value);
    }

    public static readonly BindableProperty DeleteCommandProperty =
        BindableProperty.Create(nameof(DeleteCommand), typeof(ICommand), typeof(ChampionshipList));

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public static readonly BindableProperty ShowChampionshipCommandProperty =
        BindableProperty.Create(nameof(ShowChampionshipCommand), typeof(ICommand), typeof(ChampionshipList));

    public ICommand ShowChampionshipCommand
    {
        get => (ICommand)GetValue(ShowChampionshipCommandProperty);
        set => SetValue(ShowChampionshipCommandProperty, value);
    }

    public ChampionshipList()
    {
        InitializeComponent();
    }
}