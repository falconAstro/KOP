using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
    private FirebaseAuthClient _firebaseAuthClient;
    private FirebaseClient _firebaseClient;
    private User User { get; set; }
    private RegisteredUser PickedUser { get; set; }
    public SettingsPage(FirebaseAuthClient firebaseAuthClient,FirebaseClient firebaseClient)
	{
		InitializeComponent();
        
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
            AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()
        });
    }
    public async Task LoadRegisteredUsersAsync()
    {
        var LoadUsers = await _firebaseClient.Child("RegisteredUsers").OrderBy("UserID").EqualTo(User.Uid).OnceAsync<RegisteredUser>();
        var LoadedUser = LoadUsers.FirstOrDefault();
        PickedUser = LoadedUser.Object;
        UsernameLabel.Text = PickedUser.Username;
        EmailLabel.Text = PickedUser.Email;
        IdLabel.Text = PickedUser.UserId;
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        try 
        {
            base.OnNavigatedTo(args);
            User = _firebaseAuthClient.User;
            await LoadRegisteredUsersAsync();
            await Toast.Make("Loaded data successfully", ToastDuration.Long).Show();
        }
        catch (FirebaseException) 
        {
            await Toast.Make("Firebase error", ToastDuration.Short).Show();
        }
        catch (Exception) 
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
    }
    private async Task LogOut()
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
    private void BtnLogOut_Clicked (object sender, EventArgs e)
	{

        LogOut();
    }
}