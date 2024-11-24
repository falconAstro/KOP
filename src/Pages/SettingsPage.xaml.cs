using Firebase.Auth;

namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    public SettingsPage(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
    }
	private void BtnLogOut_Clicked (object sender, EventArgs e)
	{
        try
        {
            _firebaseAuthClient.SignOut();
            Shell.Current.DisplayAlert("", "User successfully signed out", "OK");
            Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        catch (Exception ex)
        {
            Shell.Current.DisplayAlert("", "Error", "OK");
        }
    }
}