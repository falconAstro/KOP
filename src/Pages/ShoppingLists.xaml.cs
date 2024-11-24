using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace TimeManagementApp.Pages;

public partial class ShoppingLists : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    public class ShoppingList
    {
        public string Username { get; set; }
        public string ReceiverUsername { get; set; }
        public string ShoppingItems { get; set; }
        public string ListId { get; set; }
    }
    public ObservableCollection<ShoppingList> ShoppingListList { get; set; } = [];
    public ShoppingLists(FirebaseClient firebaseClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseClient = firebaseClient;
    }
    //Nacitanie listov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await LoadShoppingListsAsync();
    }
    //Funkcia nacitania listov z databazy
    public async Task LoadShoppingListsAsync()
    {
        ShoppingListList.Clear();
        _firebaseClient.Child("ShoppingList").AsObservable<ShoppingList>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.ListId = item.Key;
                ShoppingListList.Add(item.Object);
            }
        });
    }
    private async void BtnCreateShoppingList_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("ShoppingList").PostAsync(new ShoppingList { ShoppingItems = EntryShoppingItems.Text, ReceiverUsername = EntryReceiverUsername.Text });

        EntryShoppingItems.Text = string.Empty;
        EntryReceiverUsername.Text = string.Empty;

        await Shell.Current.DisplayAlert("", "Shopping list created", "OK");
    }
}