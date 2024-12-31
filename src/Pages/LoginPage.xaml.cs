using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;

namespace TimeManagementApp.Pages;

public partial class LoginPage : ContentPage
{
    private FirebaseAuthClient _firebaseAuthClient;
    private FirebaseClient _firebaseClient;
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
            await Toast.Make("Signed in succesfully", ToastDuration.Long).Show();
            //Otvorenie taskov
            await Shell.Current.GoToAsync($"//{nameof(PersonalTasks)}");
        }
        //Errory spojene s Firebase Auth systemom (nespravny email, atd)
        catch (FirebaseAuthException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Long).Show();
        }
        //Ostatne errory
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
        
	}

	//Stlacenie SignUp tlacidla
    private void BtnGoToSignUpPage_Clicked(object sender, EventArgs e)
    {	
		
		Shell.Current.GoToAsync(nameof(SignUpPage));
        
       
    }
}