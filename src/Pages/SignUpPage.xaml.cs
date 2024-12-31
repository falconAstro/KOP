using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SignUpPage : ContentPage
{
    private FirebaseAuthClient _firebaseAuthClient;
    private FirebaseClient _firebaseClient;

    private User User { get; set; }
    public SignUpPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
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
        EntryUsername.Text = string.Empty;
        EntryRepeatedPassword.Text = string.Empty;
    }
    private async void SignUp()
    {
        
        try
        {
            //Porovnanie oboch password entry
            if (EntryPassword.Text == EntryRepeatedPassword.Text)
            {
                //Vytvorenie pouzivatela
                var credentials = await _firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(email:EntryEMail.Text, password:EntryPassword.Text, displayName:EntryUsername.Text);

                //log in
                await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email: EntryEMail.Text, password: EntryPassword.Text);
                User = _firebaseAuthClient.User;

                //vytvorenie objektu registered user
                await _firebaseClient.Child("RegisteredUsers").PostAsync(new RegisteredUser{ Username=EntryUsername.Text, UserId = User.Uid, Email=EntryEMail.Text });
                //log out
                _firebaseAuthClient.SignOut();
                await Toast.Make("Signed up succesfully", ToastDuration.Long).Show();
                //Navrat na Log In page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Toast.Make("Passwords do not match!", ToastDuration.Long).Show();
            }
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
    //Vytvorenie usera a vymazanie Entry obsahu
    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        SignUp();
    }
}