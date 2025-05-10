using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using TimeManagementApp.Services;
using TimeManagementApp.Resources.Languages;

namespace TimeManagementApp.Pages;

public partial class ShoppingLists : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public User LoggedUser { get; set; }//Aktualne prihlaseny user
    public RegisteredUser SelectedUser { get; set; }//User vybraty v pickeri
    public List<string> TempShoppingItems { get; set; } = [];//Zoznam itemov pri vytvarani noveho Shopping listu
    public ObservableCollection<ShoppingList> ShoppingListList { get; set; } = [];//Zoznam Shopping listov nacitavanych z DB
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];//Zoznam userov nacitavanych z DB

    public ShoppingLists(FirebaseService firebaseService)//Konstruktor stranky
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
            await RemoveLoggedUserFromList();//Odstranenie aktualne prihlaseneho usera zo zoznamu
            picker.ItemsSource = RegisteredUserList;
            await LoadShoppingListsToCollection();
            await Toast.Make(AppResources.LoadedDataToast, ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make(AppResources.ErrorToastFirebaseAuth, ToastDuration.Short).Show();
        }
        catch (FirebaseException)
        {
            await Toast.Make(AppResources.ErrorToastFirebase, ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Short).Show();
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
            await Toast.Make(AppResources.ShoppingListToastNull, ToastDuration.Short).Show();
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
            bool isCreationConfirmed = await DisplayAlert(AppResources.ShoppingListLabel1, $"{AppResources.ShoppingListCreationConfirm}",AppResources.Yes,AppResources.No);
            if (isCreationConfirmed)
            {
                await _firebaseService.Client.Child("ShoppingList").Child(SelectedUser.UserId).PostAsync(new ShoppingList { ShoppingItems = TempShoppingItems, Username = LoggedUser.Info.DisplayName, Date = DateTime.Now.ToString("ddd dd.MM.yyyy HH:mm") });
                await Toast.Make(AppResources.ShoppingListCreationToast, ToastDuration.Short).Show();
                picker.SelectedItem = null;
                TempShoppingItems.Clear();
            }
        }
        catch (FirebaseException)
        {
            await Toast.Make(AppResources.ErrorToastFirebase, ToastDuration.Long).Show();
        }
        catch (Exception)
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Long).Show();
        }
    }

    private async void BtnCreateShoppingList_Clicked(object sender, EventArgs e)
    {
        if (TempShoppingItems.Count==0 || SelectedUser == null)
        {
            await Toast.Make(AppResources.ShoppingListToastNull2, ToastDuration.Short).Show();
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
    public async Task RemoveLoggedUserFromList()//Odstranenie aktualne prihlaseneho usera zo zoznamu
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
                    await DisplayAlert(AppResources.ErrorToast,AppResources.ErrorToastDelete, "OK");
                    return;
                }
                bool isDeletionConfirmed = await DisplayAlert(AppResources.ShoppingListDelete,AppResources.ShoppingListConfirmationDeletion,AppResources.Yes,AppResources.No);
                if (isDeletionConfirmed)
                {
                    await _firebaseService. Client.Child("ShoppingList").Child(LoggedUser.Uid).Child($"{SwipeView.ListId}").DeleteAsync();
                    await Toast.Make(AppResources.ShoppingListDeletioniToast, ToastDuration.Short).Show();
                    await LoadShoppingListsToCollection();
                }
            }
        }
        catch (FirebaseException)
        {
            await Toast.Make(AppResources.ErrorToastFirebase, ToastDuration.Short).Show();
        }
        catch (Exception)
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Short).Show();
        }
    }
}