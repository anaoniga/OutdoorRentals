using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;
using System.Globalization;

namespace OutdoorRentals.Mobile;

public partial class RentalItemEditPage : ContentPage
{
    private readonly ApiService _api;
    private readonly RentalDto _rental;
    private readonly RentalItemDto? _editing;

    public RentalItemEditPage(ApiService api, RentalDto rental, RentalItemDto? editing = null)
    {
        InitializeComponent();
        _api = api;
        _rental = rental;
        _editing = editing;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var equipments = await _api.GetEquipmentsAsync();
        EquipmentPicker.ItemsSource = equipments;

        if (_editing != null)
        {
            Title = "Edit Item";
            QtyEntry.Text = _editing.Quantity.ToString(CultureInfo.InvariantCulture);
            RateEntry.Text = _editing.DailyRate.ToString(CultureInfo.InvariantCulture);

            var selected = equipments.FirstOrDefault(e => e.Id == _editing.EquipmentId);
            if (selected != null) EquipmentPicker.SelectedItem = selected;
        }
        else
        {
            Title = "Add Item";
            QtyEntry.Text = "1";
        }
    }

    private void OnEquipmentChanged(object sender, EventArgs e)
    {
        // auto-complete rate/day from equipment when adding new item
        if (_editing != null) return;

        if (EquipmentPicker.SelectedItem is EquipmentDto eq)
            RateEntry.Text = eq.DailyRate.ToString(CultureInfo.InvariantCulture);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (EquipmentPicker.SelectedItem is not EquipmentDto eq)
        {
            await DisplayAlert("Validation", "Please select equipment.", "OK");
            return;
        }

        if (!int.TryParse((QtyEntry.Text ?? "").Trim(), out var qty) || qty <= 0)
        {
            await DisplayAlert("Validation", "Quantity must be > 0.", "OK");
            return;
        }

        if (!decimal.TryParse((RateEntry.Text ?? "").Trim(),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var rate) || rate < 0)
        {
            await DisplayAlert("Validation", "Rate must be a number >= 0 (use dot).", "OK");
            return;
        }

        var dto = new RentalItemDto
        {
            Id = _editing?.Id ?? 0,
            RentalId = _rental.Id,
            EquipmentId = eq.Id,
            Quantity = qty,
            DailyRate = rate
        };

        bool ok = _editing == null
            ? await _api.CreateRentalItemAsync(dto)
            : await _api.UpdateRentalItemAsync(dto);

        if (!ok)
        {
            await DisplayAlert("Error", "Save failed.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }
}
