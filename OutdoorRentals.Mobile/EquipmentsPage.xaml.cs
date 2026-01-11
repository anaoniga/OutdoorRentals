using OutdoorRentals.Mobile.Models;
using OutdoorRentals.Mobile.Services;

namespace OutdoorRentals.Mobile;

public partial class EquipmentsPage : ContentPage
{
    private readonly ApiService _api;

    public EquipmentsPage(ApiService api)
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
        var equipments = await _api.GetEquipmentsAsync();

        // Ca s? afi??m numele categoriei, lu?m categoriile ?i facem map Id -> Name
        var categories = await _api.GetCategoriesAsync();
        var map = categories.ToDictionary(c => c.Id, c => c.Name);

        foreach (var e in equipments)
        {
            e.EquipmentCategoryName = map.TryGetValue(e.EquipmentCategoryId, out var n) ? n : "-";
        }

        EquipmentsList.ItemsSource = equipments;
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EquipmentEditPage(_api));
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        var eq = (sender as Button)?.CommandParameter as EquipmentDto;
        if (eq == null) return;

        await Navigation.PushAsync(new EquipmentEditPage(_api, eq));
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var eq = (sender as Button)?.CommandParameter as EquipmentDto;
        if (eq == null) return;

        var confirm = await DisplayAlert("Confirm", $"Delete '{eq.Name}'?", "Yes", "No");
        if (!confirm) return;

        var ok = await _api.DeleteEquipmentAsync(eq.Id);
        if (!ok)
        {
            await DisplayAlert("Error", "Delete failed.", "OK");
            return;
        }

        await LoadAsync();
    }
}
