using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SharedEvents : ContentPage
{
    private readonly FirebaseClient firebaseClient;

    public ObservableCollection<SharedEvent> SharedEventsList { get; set; } = [];

    public SharedEvents(FirebaseClient firebaseClient)
    {
        InitializeComponent();
        BindingContext = this;
        this.firebaseClient = firebaseClient;
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        SharedEventsList.Clear();
        await LoadEventsAsync();
    }
    public async Task LoadEventsAsync()
    {
        this.firebaseClient.Child("SharedEvent").AsObservable<SharedEvent>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                SharedEventsList.Add(item.Object);
            }
        });
    }
    private void BtnCreateSharedEvent_Clicked(object sender, EventArgs e)
    {
        this.firebaseClient.Child("SharedEvent").PostAsync(new SharedEvent
        {
            Event = EntrySharedEvent.Text,
            Date = EntryDate.Text

        });

        EntrySharedEvent.Text = string.Empty;
        EntryDate.Text = string.Empty;
    }
}