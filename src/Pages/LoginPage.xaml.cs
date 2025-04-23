using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using TimeManagementApp.Services;

namespace TimeManagementApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public LoginPage(FirebaseService firebaseService)//Konstruktor stranky
    {
		InitializeComponent();
        _firebaseService = firebaseService;
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
            await _firebaseService.AuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
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