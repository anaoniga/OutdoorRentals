using OutdoorRentals.Mobile.Pages;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile;

public partial class MainPage : ContentPage
{
    private readonly ApiService _api;
    private bool _isLoggingIn;

    public MainPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isLoggingIn)
            return;

        _isLoggingIn = true;
        LoginButton.IsEnabled = false;

        try
        {
            ResultLabel.Text = "Logging in...";

            var email = (EmailEntry.Text ?? "").Trim();
            var password = PasswordEntry.Text ?? "";

            var ok = await _api.LoginAsync(email, password);
            if (!ok)
            {
                ResultLabel.Text = "Login failed. Check email/password.";
                return;
            }

            // Navigare la pagina de categorii (CRUD)
            await Navigation.PushAsync(new CategoriesPage(_api));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
            ResultLabel.Text = "Network error.";
        }
        finally
        {
            LoginButton.IsEnabled = true;
            _isLoggingIn = false;
        }
    }
}
