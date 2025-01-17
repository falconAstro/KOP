using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SharedEvents : ContentPage
{
    //Firebase clients
    private readonly FirebaseClient _firebaseClient;
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private DateTime DateNow;//Aktualny datum a cas
    private List<SharedEvent> TempSharedEventsList { get; set; } = [];//Zoznam Shared eventov
    public ObservableCollection<SharedEvent> SharedEventsList { get; set; } = [];//Zoznam zoradenych Shared eventov (pre zobrazenie v UI)

    //Konstruktor stranky
    public SharedEvents(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
    {
        InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
            AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()//Refresh tokenu
        });
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)//Vykona sa pri nacitani stranky
    {
        try
        {
            base.OnNavigatedTo(args);
            DateNow = DateTime.Now;//Aktualny datum a cas
            SharedEventsList.Clear();
            await LoadEventsAsync();
            await Toast.Make("Loaded data successfully", ToastDuration.Short).Show();
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
    public async Task LoadEventsAsync()//Nacitanie Shared eventov z databazy a ich zoradenie podla datumu
    {
        EventDatePicker.Date = DateNow;
        EventDatePicker.MinimumDate = DateNow;
        SharedEventsList.Clear();
        TempSharedEventsList.Clear();
        var LoadedEvents = await _firebaseClient.Child("SharedEvent").OnceAsync<SharedEvent>();
        foreach(var _event in LoadedEvents)
        {
            var SharedEvent = _event.Object;
            SharedEvent.EventId = _event.Key;
            TempSharedEventsList.Add(SharedEvent);
        }
        var SortedEvents = TempSharedEventsList.OrderBy(SharedEvent => SharedEvent.Date);
        foreach (var _event in SortedEvents)
        {
            SharedEventsList.Add(_event);
        }
    }

    private async Task CreateEventAsync()//Vytvorenie Shared eventu
    {
        try
        {
            await _firebaseClient.Child("SharedEvent").PostAsync(new SharedEvent
            {
                Event = EntrySharedEvent.Text,
                Date = EventDatePicker.Date
            });
            EntrySharedEvent.Text = string.Empty;
            await Toast.Make("Event created successfully", ToastDuration.Short).Show();
            await LoadEventsAsync();
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

    private async void BtnCreateSharedEvent_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(EntrySharedEvent.Text))
        {
            await CreateEventAsync();
        }
        else
        {
            await Toast.Make("Enter an event and pick a date first!", ToastDuration.Short).Show();
        }
    }

    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)//Vymazavanie jednotlivych Shared eventov
    {
        try
        {
            if (sender is SwipeItem swipeItem)
            {
                var SwipeView = swipeItem.BindingContext as SharedEvent;
                if (SwipeView == null)
                {
                    await DisplayAlert("Error", "Failed to identify the item to delete.", "OK");
                }
                else
                {
                    bool isDeletionConfirmed = await DisplayAlert("Delete Event", $"Are you sure you want to delete the event \"{SwipeView.Event}\"?", "Yes", "No");
                    if (isDeletionConfirmed)
                    {
                        await _firebaseClient.Child("SharedEvent").Child($"{SwipeView.EventId}").DeleteAsync();
                        await Toast.Make("Event deleted successfully", ToastDuration.Short).Show();
                        await LoadEventsAsync();
                    }
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