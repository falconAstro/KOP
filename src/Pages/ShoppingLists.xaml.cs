using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using TimeManagementApp.Services;

namespace TimeManagementApp.Pages;

public partial class ShoppingLists : ContentPage
{
    //Firebase clients
    private readonly FirebaseService _firebaseService;
    public User LoggedUser { get; set; }//Aktualne prihlaseny user
    public RegisteredUser SelectedUser { get; set; }//User vybraty v pickeri
    public List<string> TempShoppingItems { get; set; } = [];//Zoznam itemov pri vytvarani noveho Shopping listu
    public ObservableCollection<ShoppingList> ShoppingListList { get; set; } = [];//Zoznam Shopping listov nacitavanych z DB
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];//Zoznam userov nacitavanych z DB

    //Konstruktor stranky
    public ShoppingLists(FirebaseService firebaseService)
    {
        InitializeComponent();
        BindingContext = this;
        _firebaseService = firebaseService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky
    {
        try
        {
            base.OnNavigatedTo(args);
            LoggedUser = _firebaseService.AuthClient.User;//Aktualne prihlaseny user
            //Nacitanie pouzivatelov do zoznamu pre picker
            var _RegisteredUserList = LoadRegisteredUsersAsync();
            RegisteredUserList.Clear();
            RegisteredUserList = await _RegisteredUserList;
            RemoveLoggedUserFromList();//Odstranenie aktualne prihlaseneho usera zo zoznamu
            picker.ItemsSource = RegisteredUserList;
            await LoadShoppingListsToCollection();
            await Toast.Make("Loaded data successfully", ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make("Firebase Auth Error", ToastDuration.Short).Show();
        }
        catch (FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
    }

    private async Task LoadShoppingListsToCollection()//Nacitanie shopping listov z pola do observable collection
    {
        ShoppingListList.Clear();
        var _loadedLists = await LoadShoppingListsAsync();
        foreach (ShoppingList list in _loadedLists)
        {
            ShoppingListList.Add(list);
        }
    }

    private async Task<List<ShoppingList>> LoadShoppingListsAsync()//Nacitanie shopping listov z databazy do pola
    {
        return (await _firebaseService.Client.Child("ShoppingList").Child(LoggedUser.Uid).OnceAsync<ShoppingList>()).Select(item => new ShoppingList
        {
            ShoppingItems = item.Object.ShoppingItems,
            ListId = item.Key,
            Username = item.Object.Username,
            Date = item.Object.Date,
        }).ToList();
    }

    public async Task<List<RegisteredUser>> LoadRegisteredUsersAsync()//Nacitanie userov z databazy
    {
        return (await _firebaseService.Client.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserId = item.Object.UserId,
            Email = item.Object.Email,
        }).ToList();
    }

    private async Task AddItemToList()//Pridanie itemu do zoznamu
    {
        if (string.IsNullOrEmpty(EntryShoppingItem.Text))
        {
            await Toast.Make("Enter an item first!", ToastDuration.Short).Show();
            return;
        }
        TempShoppingItems.Add(EntryShoppingItem.Text);
        EntryShoppingItem.Text = string.Empty;
    }
    private async void BtnAddShoppingItem_Clicked(object sender, EventArgs e)
    {
        await AddItemToList();
    }
    private async Task CreateShoppingListAsync()//Vytvorenie Shopping listu
    {
        try
        {
            bool isCreationConfirmed = await DisplayAlert("Create Shopping List", $"Are you sure you want to create this shopping list?", "Yes", "No");
            if (isCreationConfirmed)
            {
                await _firebaseService.Client.Child("ShoppingList").Child(SelectedUser.UserId).PostAsync(new ShoppingList { ShoppingItems = TempShoppingItems, Username = LoggedUser.Info.DisplayName, Date = DateTime.Now.ToString("ddd dd.MM.yyyy HH:mm") });
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
        if (TempShoppingItems.Count==0 || SelectedUser == null)
        {
            await Toast.Make("Enter items and pick a user first!", ToastDuration.Short).Show();
            return;
        }
        await CreateShoppingListAsync();
        await LoadShoppingListsToCollection();
    }

    void OnPickerSelectedIndexChanged(object sender, EventArgs e)//Vybratie Shopping listu v pickeri
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            SelectedUser = (RegisteredUser)picker.ItemsSource[selectedIndex];
        }
    }
    public void RemoveLoggedUserFromList()//Odstranenie aktualne prihlaseneho usera zo zoznamu
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
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)//Vymazavanie jednotlivych Shopping listov
    {
        try
        {
            if (sender is SwipeItem swipeItem)
            {
                var SwipeView = swipeItem.BindingContext as ShoppingList;
                if (SwipeView == null)
                {
                    await DisplayAlert("Error", "Failed to identify the item to delete.", "OK");
                    return;
                }
                bool isDeletionConfirmed = await DisplayAlert("Delete Shopping List", $"Are you sure you want to delete this shopping list?", "Yes", "No");
                if (isDeletionConfirmed)
                {
                    await _firebaseService. Client.Child("ShoppingList").Child(LoggedUser.Uid).Child($"{SwipeView.ListId}").DeleteAsync();
                    await Toast.Make("Shopping list successfully deleted", ToastDuration.Short).Show();
                    await LoadShoppingListsToCollection();
                }
            }
        }
        catch (FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make("Error", ToastDuration.Short).Show();
        }
    }
}