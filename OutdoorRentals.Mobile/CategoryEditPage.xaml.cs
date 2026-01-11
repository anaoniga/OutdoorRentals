using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile.Pages;

public partial class CategoryEditPage : ContentPage
{
    private readonly ApiService _api;
    private readonly EquipmentCategoryDto? _editing;

    public CategoryEditPage(ApiService api, EquipmentCategoryDto? editing = null)
    {
        InitializeComponent();
        _api = api;
        _editing = editing;

        if (_editing != null)
        {
            Title = "Edit Category";
            NameEntry.Text = _editing.Name;
        }
        else
        {
            Title = "Add Category";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = (NameEntry.Text ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Validation", "Name is required.", "OK");
            return;
        }

        if (name.Length < 2)
        {
            await DisplayAlert("Validation", "Name must be at least 2 characters.", "OK");
            return;
        }

        bool ok = _editing == null
            ? await _api.CreateCategoryAsync(name)
            : await _api.UpdateCategoryAsync(_editing.Id, name);

        if (!ok)
        {
            await DisplayAlert("Error", "Save failed.", "OK");
            return;
        }

        await Navigation.PopAsync();
    }
}
