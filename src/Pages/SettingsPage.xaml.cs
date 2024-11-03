namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
	private void BtnLogOut_Clicked (object sender, EventArgs e)
	{
        Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}