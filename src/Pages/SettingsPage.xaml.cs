using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using TimeManagementApp.Services;
using TimeManagementApp.Resources.Languages;

namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public SettingsPage(FirebaseService firebaseService)//Konstruktor stranky
    {
		InitializeComponent();
        _firebaseService = firebaseService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky 
    {
        try 
        {
            base.OnNavigatedTo(args);
            //Nacitanie logged usera a jeho udajov
            var LoggedUser = _firebaseService.AuthClient.User;
            UsernameLabel.Text = LoggedUser.Info.DisplayName;
            EmailLabel.Text = LoggedUser.Info.Email;
            IdLabel.Text = LoggedUser.Uid;
            await Toast.Make(AppResources.LoadedDataToast, ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException) 
        {
            await Toast.Make(AppResources.ErrorToastFirebaseAuth, ToastDuration.Short).Show();
        }
        catch (Exception) 
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Short).Show();
        }
    }
    private async Task SignOut()//Odhlasenie
    {
        try
        {
            bool isSignOutConfirmed = await DisplayAlert(AppResources.SettingsSignOut,AppResources.SignOutConfirmation,AppResources.Yes,AppResources.No);
            if (isSignOutConfirmed) 
            {
                _firebaseService.AuthClient.SignOut();
                await Toast.Make(AppResources.SignOutSuccess, ToastDuration.Long).Show();
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make(AppResources.ErrorToastFirebaseAuth, ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Long).Show();
        }
    }
    private async void BtnSignOut_Clicked (object sender, EventArgs e)
	{

        await SignOut();
    }
}