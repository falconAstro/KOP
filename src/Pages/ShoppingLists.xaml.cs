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
    public User LoggedUser { get; set; }
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
        LoggedUser = _firebaseAuthClient.User;
        //Nacitanie pouzivatelov do pickeru
        var _RegisteredUserList = LoadRegisteredUsersAsync();
        RegisteredUserList.Clear();
        RegisteredUserList = await _RegisteredUserList;
        RemoveLoggedUserFromList();
        picker.ItemsSource = RegisteredUserList;
        //Nacitanie taskov pri otvoreni stranky 
        await LoadShoppingListsAsync();
    }
    //Funkcia nacitania listov z databazy
    public async Task LoadShoppingListsAsync()
    {
        ShoppingListList.Clear();
        _firebaseClient.Child("ShoppingList").Child(LoggedUser.Uid).AsObservable<ShoppingList>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.ListId = item.Key;
                ShoppingListList.Add(item.Object);
            }
        });
    }
    public async Task<List<RegisteredUser>> LoadRegisteredUsersAsync()
    {
        return (await _firebaseClient.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserId = item.Object.UserId,
            Email = item.Object.Email,
        }).ToList();
    }
    private async void BtnCreateShoppingList_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("ShoppingList").Child(SelectedUser.UserId).PostAsync(new ShoppingList { ShoppingItems = EntryShoppingItems.Text, Username = LoggedUser.Uid });

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
    public void RemoveLoggedUserFromList()
    {
        foreach (RegisteredUser ListUser in RegisteredUserList)
        {
            if (ListUser.UserId == LoggedUser.Uid)
            {
                RegisteredUserList.Remove(ListUser);
                break;
            }
        }
    }
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem)
        {
            var SwipeView = swipeItem.BindingContext as ShoppingList;
            if (SwipeView == null)
            {
                await DisplayAlert("Error", "Failed to identify the item to delete.", "OK");
            }
            else
            {
                bool isDeletionConfirmed = await DisplayAlert("Delete Shopping List", $"Are you sure you want to delete this shopping list?", "Yes", "No");
                if (isDeletionConfirmed)
                {
                    await _firebaseClient.Child("ShoppingList").Child(LoggedUser.Uid).Child($"{SwipeView.ListId}").DeleteAsync();
                    await LoadShoppingListsAsync();
                }
            }
        }
    }
}