using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using TimeManagementApp.Classes;
using TimeManagementApp.Services;
using TimeManagementApp.Resources.Languages;

namespace TimeManagementApp.Pages;

public partial class SignUpPage : ContentPage
{
    //Firebase clients
    private readonly FirebaseService _firebaseService;
    public SignUpPage(FirebaseService firebaseService)//Konstruktor stranky
    {
		InitializeComponent();
        _firebaseService = firebaseService;
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
                await Toast.Make(AppResources.SignUpToastNullUsername, ToastDuration.Short).Show();
                return;
            }
            //Porovnanie oboch password entry
            else if (EntryPassword.Text != EntryRepeatedPassword.Text)
            {
                await Toast.Make(AppResources.SignUpToastPasswordsMissmatch, ToastDuration.Short).Show();
                return;
            }
            //Vytvorenie pouzivatela
            var credentials = await _firebaseService.AuthClient.CreateUserWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text, displayName: EntryUsername.Text);
            //Log in z dovodu pridania noveho uctu do DB
            await _firebaseService.AuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
            var LoggedUser = _firebaseService.AuthClient.User;//Aktualne prihlaseny user
            //Vytvorenie objektu registered user v DB
            await _firebaseService.Client.Child("RegisteredUsers").PostAsync(new RegisteredUser { Username = EntryUsername.Text, UserId = LoggedUser.Uid, Email = EntryEMail.Text });
            _firebaseService.AuthClient.SignOut();//Log out
            await Toast.Make(AppResources.SignUpToastSuccess, ToastDuration.Short).Show();
            await Shell.Current.GoToAsync("..");//Navrat na Log In page
        }
        catch (FirebaseAuthException)//Errory spojene s Firebase Auth systemom (nespravny email, atd)
        {
            await Toast.Make(AppResources.ErrorToastFirebaseAuth, ToastDuration.Short).Show();
        }
        catch (FirebaseException)//Errory spojene s DB
        {
            await Toast.Make(AppResources.ErrorToastFirebase, ToastDuration.Short).Show();
        }
        catch (Exception)//Ostatne errory
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Short).Show();
        }
    }
    private async void BtnSignUp_Clicked(object sender, EventArgs e)//Stlacenie tlacidla registracie
    {
        await SignUp();
    }
}