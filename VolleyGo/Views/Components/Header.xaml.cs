using System.Windows.Input;

namespace VolleyGo.Views.Components;

public partial class Header : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(Header),
            default(string),
            propertyChanged: OnTitleChanged);

    public string? Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty ShowBackButtonProperty =
        BindableProperty.Create(
            nameof(ShowBackButton),
            typeof(bool),
            typeof(Header),
            false);

    public bool ShowBackButton
    {
        get => (bool)GetValue(ShowBackButtonProperty);
        set => SetValue(ShowBackButtonProperty, value);
    }

    public static readonly BindableProperty ShowLogoutButtonProperty =
        BindableProperty.Create(
            nameof(ShowLogoutButton),
            typeof(bool),
            typeof(Header),
            false);

    public bool ShowLogoutButton
    {
        get => (bool)GetValue(ShowLogoutButtonProperty);
        set => SetValue(ShowLogoutButtonProperty, value);
    }

    public static readonly BindableProperty GoBackCommandProperty =
    BindableProperty.Create(
        nameof(GoBackCommand),
        typeof(ICommand),
        typeof(Header),
        null);

    public ICommand? GoBackCommand
    {
        get => (ICommand?)GetValue(GoBackCommandProperty);
        set => SetValue(GoBackCommandProperty, value);
    }

    public static readonly BindableProperty SaveCommandProperty =
    BindableProperty.Create(
        nameof(SaveCommand),
        typeof(ICommand),
        typeof(Header),
        null);

    public ICommand? SaveCommand
    {
        get => (ICommand?)GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }

    public bool ShowSaveButton
    {
        get => (bool)GetValue(ShowSaveButtonProperty);
        set => SetValue(ShowSaveButtonProperty, value);
    }

    public static readonly BindableProperty ShowSaveButtonProperty =
        BindableProperty.Create(
            nameof(ShowSaveButton),
            typeof(bool),
            typeof(Header),
            false);

    // Propiedad calculada
    public bool ShowLogo => string.IsNullOrEmpty(Title);
    public bool ShowTitle => !string.IsNullOrEmpty(Title);

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (Header)bindable;
        // Notifica que ShowLogo también cambió
        control.OnPropertyChanged(nameof(ShowLogo));
        control.OnPropertyChanged(nameof(ShowTitle));
    }

    public Header()
    {
        InitializeComponent();
    }
}