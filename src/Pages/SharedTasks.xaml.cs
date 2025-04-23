using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Services;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class SharedTasks : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public List<RegisteredUser> RegisteredUserList { get; set; } = [];//Zoznam userov nacitavanych z DB (pre picker)
    public ObservableCollection<SharedTask> SharedTaskList { get; set; } = [];//Zoznam Taskov nacitavanych z DB
    public User LoggedUser { get; set; }//Aktualne prihlaseny user
    public RegisteredUser SelectedUser { get; set; }//User vybraty v pickeri

    public SharedTasks(FirebaseService firebaseService)//Konstruktor stranky
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
            await LoadSharedTasksToCollection();
            await Toast.Make("Loaded data successfully", ToastDuration.Long).Show();
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make("Firebase Auth Error", ToastDuration.Short).Show();
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

    private async Task LoadSharedTasksToCollection()//Nacitanie shared taskov z pola do observable collection
    {
        SharedTaskList.Clear();
        var _loadedTasks = await LoadSharedTasksAsync();
        foreach (SharedTask task in _loadedTasks)
        {
            SharedTaskList.Add(task);
        }
    }

    private async Task<List<SharedTask>> LoadSharedTasksAsync()//Nacitanie shared taskov z databazy do pola
    {
        return (await _firebaseService.Client.Child("SharedTask").Child(LoggedUser.Uid).OnceAsync<SharedTask>()).Select(item => new SharedTask
        {
            Task = item.Object.Task,
            TaskId = item.Key,
            Username = item.Object.Username,
        }).ToList();
    }

    private async Task<List<RegisteredUser>> LoadRegisteredUsersAsync()//Nacitanie userov z databazy
    {
        return (await _firebaseService.Client.Child("RegisteredUsers").OnceAsync<RegisteredUser>()).Select(item => new RegisteredUser
        {
            Username = item.Object.Username,
            UserId = item.Object.UserId,
            Email = item.Object.Email,
        }).ToList();
    }

    private async Task CreateTaskAsync()//Vytvorenie shared tasku
    {
        try
        {
            await _firebaseService.Client.Child("SharedTask").Child(SelectedUser.UserId).PostAsync(new SharedTask { Task = EntrySharedTask.Text, Username = LoggedUser.Info.DisplayName });
            EntrySharedTask.Text = string.Empty;
            picker.SelectedItem = null;
            await Toast.Make("Task created successfully", ToastDuration.Short).Show();
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

    private async void BtnCreateSharedTask_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(EntrySharedTask.Text) || SelectedUser == null)
        {
            await Toast.Make("Enter a task and pick a user first!", ToastDuration.Short).Show();
            return;
        }
        await CreateTaskAsync();
        await LoadSharedTasksToCollection();
    }

    void OnPickerSelectedIndexChanged(object sender, EventArgs e)//Vybratie Usera v pickeri
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            SelectedUser = (RegisteredUser)picker.ItemsSource[selectedIndex];
        }
    }

    private async Task RemoveLoggedUserFromList()//Odstranenie aktualne prihlaseneho usera zo zoznamu
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

    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)//Vymazavanie jednotlivych taskov
    {
        try
        {
            if (sender is SwipeItem swipeItem)
            {
                var SwipeView = swipeItem.BindingContext as SharedTask;
                if (SwipeView == null)
                {
                    await Toast.Make("Failed to identify item for deletion", ToastDuration.Short).Show();
                    return;
                }
                bool isDeletionConfirmed = await DisplayAlert("Delete Task", $"Are you sure you want to delete the task \"{SwipeView.Task}\"?", "Yes", "No");
                if (isDeletionConfirmed)
                {
                    await _firebaseService.Client.Child("SharedTask").Child(LoggedUser.Uid).Child($"{SwipeView.TaskId}").DeleteAsync();
                    await Toast.Make("Task successfully deleted", ToastDuration.Short).Show();
                    await LoadSharedTasksToCollection();
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