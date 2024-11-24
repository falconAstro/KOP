using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace TimeManagementApp.Pages;

public partial class SharedTasks : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    public class SharedTask
    {
        public string Username { get; set; }
        public string ReceiverUsername { get; set; }
        public required string Task { get; set; }
        public string TaskId { get; set; }
    }
    public ObservableCollection<SharedTask> SharedTaskList { get; set; } = [];
    public SharedTasks(FirebaseClient firebaseClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseClient = firebaseClient;
    }
    //Nacitanie taskov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await LoadSharedTasksAsync();
    }
    //Funkcia nacitania taskov z databazy
    public async Task LoadSharedTasksAsync()
    {
        SharedTaskList.Clear();
        _firebaseClient.Child("SharedTask").AsObservable<SharedTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.TaskId = item.Key;
                SharedTaskList.Add(item.Object);
            }
        });
    }
    private async void BtnCreateSharedTask_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("SharedTask").PostAsync(new SharedTask { Task = EntrySharedTask.Text,ReceiverUsername = EntryReceiverUsername.Text });

        EntrySharedTask.Text = string.Empty;
        EntryReceiverUsername.Text= string.Empty;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
    }
}