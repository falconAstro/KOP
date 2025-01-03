using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;

namespace TimeManagementApp.Pages;

public partial class LoginPage : ContentPage
{
    //Firebase clients
    private FirebaseAuthClient _firebaseAuthClient;
    private FirebaseClient _firebaseClient;

    //Konstruktor stranky
    public LoginPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
                AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync() //Refresh tokenu
            });
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky 
    {
        base.OnNavigatedTo(args);
        //Vymazanie Entry pri nacitani
        EntryEMail.Text = string.Empty;
		EntryPassword.Text = string.Empty;	
    }

    private async void BtnSignIn_Clicked(object sender, EventArgs e)//Stlacenie login tlacidla
    {
        try
        {
            //Prihlasenie
            await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
            await Toast.Make("Signed in succesfully", ToastDuration.Short).Show();
            //Otvorenie taskov
            await Shell.Current.GoToAsync($"//{nameof(PersonalTasks)}");
        }
        catch (FirebaseAuthException)//Errory spojene s Firebase Auth systemom (nespravny email, atd)
        {
            await Toast.Make("Firebase Auth Error", ToastDuration.Short).Show();
        }
        catch (Exception)//Ostatne errory
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
	}

    private void BtnGoToSignUpPage_Clicked(object sender, EventArgs e)//Stlacenie SignUp tlacidla
    {	
		Shell.Current.GoToAsync(nameof(SignUpPage));
    }
}