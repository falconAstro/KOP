using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
    public User LoggedUser { get; set; }
    public RegisteredUser SelectedUser { get; set; }
    public SharedTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
        new FirebaseOptions()
        {
            AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync()
        });
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        try
        {
            base.OnNavigatedTo(args);
            LoggedUser = _firebaseAuthClient.User;
            //Nacitanie pouzivatelov do pickeru
            var _RegisteredUserList = LoadRegisteredUsersAsync();
            RegisteredUserList.Clear();
            RegisteredUserList = await _RegisteredUserList;
            RemoveLoggedUserFromList();
            picker.ItemsSource = RegisteredUserList;
            //Nacitanie taskov pri otvoreni stranky 
            await LoadSharedTasksAsync();
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
    //Funkcia nacitania taskov z databazy
    public async Task LoadSharedTasksAsync()
    {
        SharedTaskList.Clear();
        _firebaseClient.Child("SharedTask").Child(LoggedUser.Uid).AsObservable<SharedTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.TaskId = item.Key;
                SharedTaskList.Add(item.Object);
            }
        });
        await Toast.Make("Loaded data successfully", ToastDuration.Long).Show();
    }
    public async Task<List<RegisteredUser>> LoadRegisteredUsersAsync()
    {
        return (await _firebaseClient.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserId = item.Object.UserId,
            Email = item.Object.Email,
        }).ToList();
    }
    private async Task CreateTaskAsync()
    {
        try
        {
            await _firebaseClient.Child("SharedTask").Child(SelectedUser.UserId).PostAsync(new SharedTask { Task = EntrySharedTask.Text, Username = LoggedUser.Info.DisplayName });

            EntrySharedTask.Text = string.Empty;
            picker.SelectedItem = null;

            await Toast.Make("Task created successfully", ToastDuration.Short).Show();
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
    private async void BtnCreateSharedTask_Clicked(object sender, EventArgs e)
    {
        await CreateTaskAsync();
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
    private void RemoveLoggedUserFromList()
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
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        try
        {
            if (sender is SwipeItem swipeItem)
            {
                var SwipeView = swipeItem.BindingContext as SharedTask;
                if (SwipeView == null)
                {
                    await DisplayAlert("Error", "Failed to identify the item to delete.", "OK");
                }
                else
                {
                    bool isDeletionConfirmed = await DisplayAlert("Delete Task", $"Are you sure you want to delete the task \"{SwipeView.Task}\"?", "Yes", "No");
                    if (isDeletionConfirmed)
                    {
                        await _firebaseClient.Child("SharedTask").Child(LoggedUser.Uid).Child($"{SwipeView.TaskId}").DeleteAsync();
                        await Toast.Make("Task successfully deleted", ToastDuration.Short).Show();
                        await LoadSharedTasksAsync();
                    }
                }
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
}