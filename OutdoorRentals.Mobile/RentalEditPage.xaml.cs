using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile;

public partial class RentalEditPage : ContentPage
{
    private readonly ApiService _api;
    private readonly RentalDto? _editing;

    public RentalEditPage(ApiService api, RentalDto? editing = null)
    {
        InitializeComponent();
        _api = api;
        _editing = editing;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var customers = await _api.GetCustomersAsync();
        CustomerPicker.ItemsSource = customers;

        if (_editing != null)
        {
            Title = "Edit Rental";
            StartDatePicker.Date = _editing.StartDate.Date;
            EndDatePicker.Date = _editing.EndDate.Date;

            var selected = customers.FirstOrDefault(c => c.Id == _editing.CustomerId);
            if (selected != null) CustomerPicker.SelectedItem = selected;
        }
        else
        {
            Title = "Add Rental";
            StartDatePicker.Date = DateTime.Today;
            EndDatePicker.Date = DateTime.Today;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (CustomerPicker.SelectedItem is not CustomerDto customer)
        {
            await DisplayAlert("Validation", "Please select a customer.", "OK");
            return;
        }

        var start = StartDatePicker.Date;
        var end = EndDatePicker.Date;

        if (end < start)
        {
            await DisplayAlert("Validation", "End date must be >= start date.", "OK");
            return;
        }

        var dto = new RentalDto
        {
            Id = _editing?.Id ?? 0,
            CustomerId = customer.Id,
            StartDate = start,
            EndDate = end
        };

        bool ok = _editing == null
            ? await _api.CreateRentalAsync(dto)
            : await _api.UpdateRentalAsync(dto);

        if (!ok)
        {
            await DisplayAlert("Error", "Save failed.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }
}
