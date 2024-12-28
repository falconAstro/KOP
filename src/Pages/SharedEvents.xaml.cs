using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SharedEvents : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    
    public ObservableCollection<SharedEvent> SharedEventsList { get; set; } = [];

    public SharedEvents(FirebaseClient firebaseClient)
    {
        InitializeComponent();
        BindingContext = this;
        _firebaseClient = firebaseClient;
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        SharedEventsList.Clear();
        await LoadEventsAsync();
    }
    public async Task LoadEventsAsync()
    {
        SharedEventsList.Clear();
        _firebaseClient.Child("SharedEvent").AsObservable<SharedEvent>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.EventId = item.Key;
                SharedEventsList.Add(item.Object);
            }
        });
    }
    private async void BtnCreateSharedEvent_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("SharedEvent").PostAsync(new SharedEvent
        {
            Event = EntrySharedEvent.Text,
            Date = EntryDate.Text

        });

        EntrySharedEvent.Text = string.Empty;
        EntryDate.Text = string.Empty;
        await Shell.Current.DisplayAlert("", "Event created", "OK");
    }
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
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
                    await LoadEventsAsync();
                }
            }
        }
    }
}