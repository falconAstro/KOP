using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SharedTasks : ContentPage
{
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];
    public ObservableCollection<SharedTask> SharedTaskList { get; set; } = [];
    public User User { get; set; }
    public RegisteredUser SelectedUser { get; set; }
    public SharedTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient;
    }
    //Nacitanie taskov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        User = _firebaseAuthClient.User;

        var _RegisteredUserList = await GetRegisteredUsers();
        RegisteredUserList.Clear();
        RegisteredUserList = _RegisteredUserList.ToList();
        picker.ItemsSource = RegisteredUserList;

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
    public async Task<List<RegisteredUser>> GetRegisteredUsers()
    {
        return (await _firebaseClient.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserID = item.Object.UserID,
            Email = item.Object.Email,
        }).ToList();
    }
    private async void BtnCreateSharedTask_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("SharedTask").Child(SelectedUser.UserID).PostAsync(new SharedTask { Task = EntrySharedTask.Text,Username = User.Uid });

        EntrySharedTask.Text = string.Empty;
        picker.SelectedItem = null;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
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
    //public async void PrepareLists()
    //{
    //    var _RegisteredUserList = await GetAll();
    //    RegisteredUserList.Clear();
    //    RegisteredUserList = _RegisteredUserList.ToList();
    //    foreach (RegisteredUser i in RegisteredUserList)
    //    {
    //        if (i.UserID == User.Uid)
    //        {
    //            RegisteredUserList.Remove(i);
    //        }
    //    }
    //    picker.ItemsSource = RegisteredUserList;
    //}
}