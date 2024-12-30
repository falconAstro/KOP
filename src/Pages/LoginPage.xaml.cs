using Firebase.Auth;
using Firebase.Database;

namespace TimeManagementApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public LoginPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
                AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()
            });
    }

	//Vymazanie Entry pri nacitani
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        EntryEMail.Text = string.Empty;
		EntryPassword.Text = string.Empty;	
    }
    //Stlacenie login tlacidla a prihlasenie
    private async void BtnLogIn_Clicked(object sender, EventArgs e)
	{
        try
        {
            //Prihlasenie
            var credentials = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
            await Shell.Current.DisplayAlert("", "Signed in successfully", "OK");
            //Otvorenie taskov
            await Shell.Current.GoToAsync($"//{nameof(PersonalTasks)}");
        }
        //Errory spojene s Firebase Auth systemom (nespravny email, atd)
        catch (FirebaseAuthException ex)
        {
            await Shell.Current.DisplayAlert("", "Firebase error", "OK");
        }
        //Ostatne errory
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", "Error", "OK");
        }
        
	}

	//Stlacenie SignUp tlacidla
    private void BtnGoToSignUpPage_Clicked(object sender, EventArgs e)
    {	
		
		Shell.Current.GoToAsync(nameof(SignUpPage));
        
       
    }
}