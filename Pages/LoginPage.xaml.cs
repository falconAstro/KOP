namespace TimeManagementApp.Pages;

public partial class LoginPage : ContentPage
{
	//Primitivny login
	public string Username { get; }
	public string Password { get; }
	public LoginPage()
	{
		InitializeComponent();

		Username = "Bob";
		Password = "1234";
        
	}

	//Stlacenie login tlacidla
	private void BtnLogIn_Clicked(object sender, EventArgs e)
	{
		if (Username == UsernameEntry.Text & Password == PasswordEntry.Text)
		{
			
            Shell.Current.GoToAsync($"//{nameof(PersonalTasks)}");
        }
		
	}

	//Stlacenie SignUp tlacidla
    private void BtnGoToSignUpPage_Clicked(object sender, EventArgs e)
    {	
		
		Shell.Current.GoToAsync(nameof(SignUpPage));
        
       
    }
}