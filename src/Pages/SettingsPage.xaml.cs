using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public User User { get; set; }
    public RegisteredUser PickedUser { get; set; }
    public SettingsPage(FirebaseAuthClient firebaseAuthClient, FirebaseClient firebaseClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient;
    }
    public async Task LoadMyData()
    {
        _firebaseClient.Child("RegisteredUsers").AsObservable<RegisteredUser>().Subscribe((item) =>
        {
            if (item.Object != null && (item.Object.UserId == User.Uid))
            {
                PickedUser=item.Object;
            }
        });
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        User = _firebaseAuthClient.User;
        //await LoadMyData();
        //CollectionView.ItemsSource = (System.Collections.IEnumerable)PickedUser;
    }
    private void BtnLogOut_Clicked (object sender, EventArgs e)
	{
        try
        {
            _firebaseAuthClient.SignOut();
            Shell.Current.DisplayAlert("", "User successfully signed out", "OK");
            Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        catch (Exception ex)
        {
            Shell.Current.DisplayAlert("", "Error", "OK");
        }
    }
}