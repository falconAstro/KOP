using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace TimeManagementApp.Pages;

public partial class SignUpPage : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;

    public User User { get; set; }
    public class RegisteredUser
    {
        public string Username { get; set; }
        public string UserID { get; set; }
        public string Email { get; set; }
        public RegisteredUser(string _Username, string _UserID, string _Email)
        {
            Username = _Username;
            UserID = _UserID;
            Email = _Email;

        }
    }
    public SignUpPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
	{
		InitializeComponent();
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient;
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
                await _firebaseClient.Child("RegisteredUsers").PostAsync(new RegisteredUser(EntryUsername.Text, User.Uid, EntryEMail.Text));
                //log out
                _firebaseAuthClient.SignOut();
                await Shell.Current.DisplayAlert("", "Signed up successfully", "OK");
                //Navrat na Log In page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("", "Passwords do not match!", "OK");
            }
        }
        //Errory spojene s Firebase Auth systemom (nespravny email, atd)
        catch (FirebaseAuthException ex)
        {
            await Shell.Current.DisplayAlert("", "Firebase Error", "OK");
        }
        //Ostatne errory
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", "Error", "OK");
        }
    }
    //Vytvorenie usera a vymazanie Entry obsahu
    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        SignUp();
    }
}