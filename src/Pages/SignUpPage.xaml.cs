using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SignUpPage : ContentPage
{
    //Firebase clients
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;

    //Konstruktor stranky
    public SignUpPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
            AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()//Refresh tokenu
        });
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky 
    {
        base.OnNavigatedTo(args);
        //Vymazanie Entry pri nacitani
        EntryUsername.Text = string.Empty;
        EntryEMail.Text = string.Empty;
        EntryPassword.Text = string.Empty;
        EntryRepeatedPassword.Text = string.Empty;
    }

    private async Task SignUp()//Registracia
    {
        try
        {
            if(EntryUsername.Text == string.Empty)//Overenie ci uzivatel zadal Username
            {
                await Toast.Make("Enter a username first!", ToastDuration.Short).Show();
                return;
            }
            //Porovnanie oboch password entry
            else if (EntryPassword.Text != EntryRepeatedPassword.Text)
            {
                await Toast.Make("Passwords do not match!", ToastDuration.Short).Show();
                return;
            }
            //Vytvorenie pouzivatela
            var credentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text, displayName: EntryUsername.Text);
            //Log in z dovodu pridania noveho uctu do DB
            await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
            var LoggedUser = _firebaseAuthClient.User;//Aktualne prihlaseny user
                                                      //Vytvorenie objektu registered user v DB
            await _firebaseClient.Child("RegisteredUsers").PostAsync(new RegisteredUser { Username = EntryUsername.Text, UserId = LoggedUser.Uid, Email = EntryEMail.Text });
            _firebaseAuthClient.SignOut();//Log out
            await Toast.Make("Signed up succesfully", ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");//Navrat na Log In page
        }
        catch (FirebaseAuthException)//Errory spojene s Firebase Auth systemom (nespravny email, atd)
        {
            await Toast.Make($"Firebase Auth Error", ToastDuration.Short).Show();
        }
        catch (FirebaseException)//Errory spojene s DB
        {
            await Toast.Make($"Firebase DB Error", ToastDuration.Short).Show();
        }
        catch (Exception)//Ostatne errory
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
    }

    private async void BtnSignUp_Clicked(object sender, EventArgs e)//Stlacenie tlacidla registracie
    {
        await SignUp();
    }
}