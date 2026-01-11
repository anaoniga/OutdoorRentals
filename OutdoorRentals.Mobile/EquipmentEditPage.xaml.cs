using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;
using System.Globalization;

namespace OutdoorRentals.Mobile;

public partial class EquipmentEditPage : ContentPage
{
    private readonly ApiService _api;
    private readonly EquipmentDto? _editing;

    public EquipmentEditPage(ApiService api, EquipmentDto? editing = null)
    {
        InitializeComponent();
        _api = api;
        _editing = editing;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var categories = await _api.GetCategoriesAsync();
        CategoryPicker.ItemsSource = categories;

        if (_editing != null)
        {
            Title = "Edit Equipment";

            NameEntry.Text = _editing.Name;
            DescriptionEntry.Text = _editing.Description ?? "";

            DailyRateEntry.Text = _editing.DailyRate.ToString(CultureInfo.InvariantCulture);
            StockTotalEntry.Text = _editing.StockTotal.ToString(CultureInfo.InvariantCulture);
            StockAvailableEntry.Text = _editing.StockAvailable.ToString(CultureInfo.InvariantCulture);

            var selected = categories.FirstOrDefault(c => c.Id == _editing.EquipmentCategoryId);
            if (selected != null) CategoryPicker.SelectedItem = selected;
        }
        else
        {
            Title = "Add Equipment";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = (NameEntry.Text ?? "").Trim();
        var desc = (DescriptionEntry.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Validation", "Name is required.", "OK");
            return;
        }

        if (CategoryPicker.SelectedItem is not EquipmentCategoryDto selectedCategory)
        {
            await DisplayAlert("Validation", "Please select a category.", "OK");
            return;
        }

        if (!decimal.TryParse((DailyRateEntry.Text ?? "").Trim(),
                NumberStyles.Number, CultureInfo.InvariantCulture, out var dailyRate) || dailyRate < 0)
        {
            await DisplayAlert("Validation", "Daily Rate must be a number >= 0 (use dot).", "OK");
            return;
        }

        if (!int.TryParse((StockTotalEntry.Text ?? "").Trim(), out var stockTotal) || stockTotal < 0)
        {
            await DisplayAlert("Validation", "Stock Total must be an integer >= 0.", "OK");
            return;
        }

        if (!int.TryParse((StockAvailableEntry.Text ?? "").Trim(), out var stockAvailable) || stockAvailable < 0)
        {
            await DisplayAlert("Validation", "Stock Available must be an integer >= 0.", "OK");
            return;
        }

        if (stockAvailable > stockTotal)
        {
            await DisplayAlert("Validation", "Stock Available cannot be greater than Stock Total.", "OK");
            return;
        }

        var dto = new EquipmentDto
        {
            Id = _editing?.Id ?? 0,
            Name = name,
            Description = string.IsNullOrWhiteSpace(desc) ? null : desc,
            DailyRate = dailyRate,
            StockTotal = stockTotal,
            StockAvailable = stockAvailable,
            EquipmentCategoryId = selectedCategory.Id
        };

        bool ok = _editing == null
            ? await _api.CreateEquipmentAsync(dto)
            : await _api.UpdateEquipmentAsync(dto);

        if (!ok)
        {
            await DisplayAlert("Error", "Save failed.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }
}
