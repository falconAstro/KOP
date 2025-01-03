using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;

namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
    //Firebase client
    private readonly FirebaseAuthClient _firebaseAuthClient;

    //Konstruktor stranky
    public SettingsPage(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky 
    {
        try 
        {
            base.OnNavigatedTo(args);
            //Nacitanie logged usera a jeho udajov
            var LoggedUser = _firebaseAuthClient.User;
            UsernameLabel.Text = LoggedUser.Info.DisplayName;
            EmailLabel.Text = LoggedUser.Info.Email;
            IdLabel.Text = LoggedUser.Uid;
            await Toast.Make("Loaded data successfully", ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException) 
        {
            await Toast.Make("Firebase Auth error", ToastDuration.Short).Show();
        }
        catch (Exception) 
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
    }
    private async Task SignOut()//Odhlasenie
    {
        try
        {
            bool isSignOutConfirmed = await DisplayAlert("Sign out", $"Are you sure you want to sign out?", "Yes", "No");
            if (isSignOutConfirmed) 
            {
                _firebaseAuthClient.SignOut();
                await Toast.Make("User signed out succesfully", ToastDuration.Long).Show();
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make("Firebase Auth Error", ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
    }
    private async void BtnSignOut_Clicked (object sender, EventArgs e)
	{

        await SignOut();
    }
}