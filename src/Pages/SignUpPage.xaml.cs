using Firebase.Auth;
using Firebase.Database;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SignUpPage : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    public SignUpPage(FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
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
        catch (FirebaseAuthException e)
        {
            await Shell.Current.DisplayAlert("", "Firebase Error", "OK");
        }
        //Ostatne errory
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("", "Error", "OK");
        }
    }
    //Vytvorenie usera a vymazanie Entry obsahu
    private async void BtnSignUp_Clicked(object sender, EventArgs e)
    {
        SignUp();
        EntryEMail.Text = string.Empty;
        EntryPassword.Text = string.Empty;
        EntryUsername.Text = string.Empty;
        EntryRepeatedPassword.Text = string.Empty;
    }
}