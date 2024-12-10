using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class ShoppingLists : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public User User { get; set; }
    public RegisteredUser SelectedUser { get; set; }
    public ObservableCollection<ShoppingList> ShoppingListList { get; set; } = [];
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];
    public ShoppingLists(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
    {
        InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient;
    }
    //Nacitanie listov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        User = _firebaseAuthClient.User;

        var _RegisteredUserList = await GetRegisteredUsers();
        RegisteredUserList.Clear();
        RegisteredUserList = _RegisteredUserList.ToList();
        picker.ItemsSource = RegisteredUserList;

        await LoadShoppingListsAsync();
    }
    //Funkcia nacitania listov z databazy
    public async Task LoadShoppingListsAsync()
    {
        ShoppingListList.Clear();
        _firebaseClient.Child("ShoppingList").Child(User.Uid).AsObservable<ShoppingList>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.ListId = item.Key;
                ShoppingListList.Add(item.Object);
            }
        });
    }
    public async Task<List<RegisteredUser>> GetRegisteredUsers()
    {
        return (await _firebaseClient.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserID = item.Object.UserID,
            Email = item.Object.Email,
        }).ToList();
    }
    private async void BtnCreateShoppingList_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("ShoppingList").Child(SelectedUser.UserID).PostAsync(new ShoppingList { ShoppingItems = EntryShoppingItems.Text, Username = User.Uid });

        EntryShoppingItems.Text = string.Empty;
        picker.SelectedItem = null;

        await Shell.Current.DisplayAlert("", "Shopping list created", "OK");
    }
    void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            SelectedUser = (RegisteredUser)picker.ItemsSource[selectedIndex];
        }
    }
}