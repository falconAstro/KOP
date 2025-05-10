using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
using TimeManagementApp.Services;
using TimeManagementApp.Resources.Languages;

namespace TimeManagementApp.Pages;

public partial class SharedEvents : ContentPage
{
    private readonly FirebaseService _firebaseService;
    private DateTime DateNow;//Aktualny datum a cas
    private List<SharedEvent> TempSharedEventsList { get; set; } = [];//Zoznam Shared eventov
    public ObservableCollection<SharedEvent> SharedEventsList { get; set; } = [];//Zoznam zoradenych Shared eventov (pre zobrazenie v UI)

    public SharedEvents(FirebaseService firebaseService)//Konstruktor stranky
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
            DateNow = DateTime.Now;//Aktualny datum a cas
            SharedEventsList.Clear();
            await LoadEventsAsync();
            await Toast.Make(AppResources.LoadedDataToast, ToastDuration.Short).Show();
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
    public async Task LoadEventsAsync()//Nacitanie Shared eventov z databazy a ich zoradenie podla datumu
    {
        EventDatePicker.Date = DateNow;
        EventDatePicker.MinimumDate = DateNow;
        SharedEventsList.Clear();
        TempSharedEventsList.Clear();
        var LoadedEvents = await _firebaseService.Client.Child("SharedEvent").OnceAsync<SharedEvent>();
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
            await _firebaseService.Client.Child("SharedEvent").PostAsync(new SharedEvent
            {
                Event = EntrySharedEvent.Text,
                Date = EventDatePicker.Date
            });
            EntrySharedEvent.Text = string.Empty;
            await Toast.Make(AppResources.SharedEventConfirmation, ToastDuration.Short).Show();
            await LoadEventsAsync();
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

    private async void BtnCreateSharedEvent_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntrySharedEvent.Text))
        {
            await Toast.Make(AppResources.SharedEventToastNull, ToastDuration.Short).Show();
            return;
        }
        await CreateEventAsync();
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
                    await DisplayAlert(AppResources.ErrorToast, AppResources.ErrorToastDelete, "OK");
                    return;
                }
                bool isDeletionConfirmed = await DisplayAlert(AppResources.SharedEventDeletion, $"{AppResources.SharedEventDeletionConfirmation} \"{SwipeView.Event}\"?",AppResources.Yes, AppResources.No);
                if (isDeletionConfirmed)
                {
                    await _firebaseService.Client.Child("SharedEvent").Child($"{SwipeView.EventId}").DeleteAsync();
                    await Toast.Make(AppResources.SharedEventDeletionToast, ToastDuration.Short).Show();
                    await LoadEventsAsync();
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