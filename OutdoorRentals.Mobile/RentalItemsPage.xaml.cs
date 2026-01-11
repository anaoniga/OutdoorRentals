using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile;

public partial class RentalItemsPage : ContentPage
{
    private readonly ApiService _api;
    private readonly RentalDto _rental;

    public RentalItemsPage(ApiService api, RentalDto rental)
    {
        InitializeComponent();
        _api = api;
        _rental = rental;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private int GetDays()
    {
        var days = (_rental.EndDate.Date - _rental.StartDate.Date).Days + 1;
        return Math.Max(1, days);
    }

    private async Task LoadAsync()
    {
        var items = await _api.GetRentalItemsAsync(_rental.Id);
        var equipments = await _api.GetEquipmentsAsync();
        var map = equipments.ToDictionary(e => e.Id, e => e.Name);

        foreach (var it in items)
            it.EquipmentName = map.TryGetValue(it.EquipmentId, out var n) ? n : "-";

        ItemsList.ItemsSource = items;

        var days = GetDays();
        HeaderLabel.Text = $"Rental #{_rental.Id} • Days: {days}";
        var total = items.Sum(i => i.Quantity * i.DailyRate * days);
        TotalLabel.Text = $"Total: {total}";
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RentalItemEditPage(_api, _rental));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var item = (sender as Button)?.CommandParameter as RentalItemDto;
        if (item == null) return;

        await Navigation.PushAsync(new RentalItemEditPage(_api, _rental, item));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var item = (sender as Button)?.CommandParameter as RentalItemDto;
        if (item == null) return;

        var confirm = await DisplayAlert("Confirm", "Delete this item?", "Yes", "No");
        if (!confirm) return;

        var ok = await _api.DeleteRentalItemAsync(item.Id);
        if (!ok)
        {
            await DisplayAlert("Error", "Delete failed.", "OK");
            return;
        }

        await LoadAsync();
    }
}
