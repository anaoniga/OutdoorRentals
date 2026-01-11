using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile.Pages;

public partial class CategoriesPage : ContentPage
{
    private readonly ApiService _api;

    public CategoriesPage(ApiService api)
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
        var items = await _api.GetCategoriesAsync();
        CategoriesList.ItemsSource = items;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CategoryEditPage(_api));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var cat = (sender as Button)?.CommandParameter as EquipmentCategoryDto;
        if (cat == null) return;

        await Navigation.PushAsync(new CategoryEditPage(_api, cat));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var cat = (sender as Button)?.CommandParameter as EquipmentCategoryDto;
        if (cat == null) return;

        var confirm = await DisplayAlert("Confirm", $"Delete '{cat.Name}'?", "Yes", "No");
        if (!confirm) return;

        var ok = await _api.DeleteCategoryAsync(cat.Id);
        if (!ok)
        {
            await DisplayAlert("Error", "Delete failed.", "OK");
            return;
        }

        await LoadAsync();
    }

    private async void OnGoEquipmentsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EquipmentsPage(_api));
    }

    private async void OnGoRentalsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RentalsPage(_api));
    }





}
