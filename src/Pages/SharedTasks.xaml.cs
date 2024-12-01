using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using static TimeManagementApp.Pages.SignUpPage;
namespace TimeManagementApp.Pages;

public partial class SharedTasks : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    private readonly FirebaseAuthClient _firebaseAuthClient;
    public class SharedTask
    {
        public string Username { get; set; }
        public required string Task { get; set; }
        public string TaskId { get; set; }
    }
    public User User { get; set; }
    public RegisteredUser SelectedUser {  get; set; }
    public ObservableCollection<SharedTask> SharedTaskList { get; set; } = [];
    public ObservableCollection<RegisteredUser> RegisteredUserList { get; set; } = [];
    public SharedTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        BindingContext = this;
        BindingContext = this;
        _firebaseClient = firebaseClient;
        _firebaseAuthClient = firebaseAuthClient;
    }
    //Nacitanie taskov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        User = _firebaseAuthClient.User;
        await LoadRegisteredUsers();
        await LoadSharedTasksAsync();
    }
    //Funkcia nacitania taskov z databazy
    public async Task LoadSharedTasksAsync()
    {
        SharedTaskList.Clear();
        _firebaseClient.Child("SharedTask").Child(User.Uid).AsObservable<SharedTask>().Subscribe((item) =>
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
        await _firebaseClient.Child("SharedTask").Child(SelectedUser.UserID).PostAsync(new SharedTask { Task = EntrySharedTask.Text,Username = User.Uid });

        EntrySharedTask.Text = string.Empty;
        picker.SelectedItem = null;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
    }
    private async Task LoadRegisteredUsers()
    {
        RegisteredUserList.Clear();
        _firebaseClient.Child("RegisteredUsers").AsObservable<RegisteredUser>().Subscribe((item) =>
        {
            if (item.Object != null && item.Object.UserID != User.Uid)
            {
                RegisteredUserList.Add(item.Object);
            }
        });
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
}