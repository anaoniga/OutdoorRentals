using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile;

public partial class RentalsPage : ContentPage
{
    private readonly ApiService _api;

    public RentalsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var rentals = await _api.GetRentalsAsync();
        var customers = await _api.GetCustomersAsync();
        var map = customers.ToDictionary(c => c.Id, c => c.FullName);

        foreach (var r in rentals)
            r.CustomerName = map.TryGetValue(r.CustomerId, out var name) ? name : "-";

        RentalsList.ItemsSource = rentals;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RentalEditPage(_api));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var rental = (sender as Button)?.CommandParameter as RentalDto;
        if (rental == null) return;

        await Navigation.PushAsync(new RentalEditPage(_api, rental));
    }

    private async void OnItemsClicked(object sender, EventArgs e)
    {
        var rental = (sender as Button)?.CommandParameter as RentalDto;
        if (rental == null) return;

        await Navigation.PushAsync(new RentalItemsPage(_api, rental));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var rental = (sender as Button)?.CommandParameter as RentalDto;
        if (rental == null) return;

        var confirm = await DisplayAlert("Confirm", "Delete this rental (and its items)?", "Yes", "No");
        if (!confirm) return;

        var ok = await _api.DeleteRentalAsync(rental.Id);
        if (!ok)
        {
            await DisplayAlert("Error", "Delete failed.", "OK");
            return;
        }

        await LoadAsync();
    }
}
