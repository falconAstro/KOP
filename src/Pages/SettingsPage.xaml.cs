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
        _firebaseAuthClient.SignOut();
        Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}