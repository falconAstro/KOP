using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace TimeManagementApp.Pages;

public partial class ShoppingLists : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public User LoggedUser { get; set; }
    public RegisteredUser SelectedUser { get; set; }
    public List<string> TempShoppingItems { get; set; } = [];
    public ObservableCollection<ShoppingList> ShoppingListList { get; set; } = [];
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];
    public ShoppingLists(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
    {
        InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
            AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()
        });
    }
    //Nacitanie listov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        try
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
        catch (FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
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
        await Toast.Make("Loaded data successfully", ToastDuration.Long).Show();
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
    private void BtnAddShoppingItem_Clicked(object sender, EventArgs e)
    {
        TempShoppingItems.Add(EntryShoppingItem.Text);
        EntryShoppingItem.Text = string.Empty;
    }
    private async Task CreateShoppingListAsync()
    {
        try
        {
            bool isCreationConfirmed = await DisplayAlert("Create Shopping List", $"Are you sure you want to create this shopping list?", "Yes", "No");
            if (isCreationConfirmed)
            {
                await _firebaseClient.Child("ShoppingList").Child(SelectedUser.UserId).PostAsync(new ShoppingList { ShoppingItems = TempShoppingItems, Username = LoggedUser.Info.DisplayName, Date = DateTime.Now.ToString("ddd dd.MM.yyyy HH:mm") });
                await Toast.Make("Shopping list created", ToastDuration.Short).Show();
                picker.SelectedItem = null;
                TempShoppingItems.Clear();
            }
        }
        catch (FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
    }
    private async void BtnCreateShoppingList_Clicked(object sender, EventArgs e)
    {
        await CreateShoppingListAsync();
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
        try
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
                        await Toast.Make("Shopping list successfully deleted", ToastDuration.Short).Show();
                        await LoadShoppingListsAsync();
                    }
                }
            }
        }
        catch (FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
    }
}