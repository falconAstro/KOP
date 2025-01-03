using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
namespace TimeManagementApp.Pages;

public partial class PersonalTasks : ContentPage
{
    //Firebase clients
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public ObservableCollection<PersonalTask> PersonalTaskList { get; set; } = []; //Zoznam Taskov nacitavanych z DB
    public User LoggedUser { get; set; } //Premenna (neskor) obsahujuca aktualne prihlaseneho usera

    //Konstruktor stranky
    public PersonalTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
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
            LoggedUser = _firebaseAuthClient.User;//Aktualne prihlaseny user
            LoadTasksAsync();
            await Toast.Make("Loaded data successfully", ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make("Firebase Auth Error", ToastDuration.Short).Show();
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

    public void LoadTasksAsync()//Nacitanie taskov z databazy
    {
        PersonalTaskList.Clear();
        _firebaseClient.Child("PersonalTask").Child(LoggedUser.Uid).AsObservable<PersonalTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.TaskId = item.Key;
                PersonalTaskList.Add(item.Object);
            }
        });
    }
    private async Task CreateTaskAsync()//Vytvorenie jednoducheho tasku v databaze
    {
        try
        {
            await _firebaseClient.Child("PersonalTask").Child(LoggedUser.Uid).PostAsync(new PersonalTask { Task = EntryPersonalTask.Text });
            EntryPersonalTask.Text = string.Empty;
            await Toast.Make("Task successfully created", ToastDuration.Long).Show();
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

    private async void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        if(!string.IsNullOrEmpty(EntryPersonalTask.Text))
        {
            await CreateTaskAsync();
        }
        else
        {
            await Toast.Make("Enter a task first!", ToastDuration.Short).Show();
        }
    }
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)//Vymazavanie jednotlivych taskov
    {
        try
        {
            if (sender is SwipeItem swipeItem)
            {
                var SwipeView = swipeItem.BindingContext as PersonalTask;
                if (SwipeView == null)
                {
                    await Toast.Make("Failed to identify item for deletion", ToastDuration.Short).Show();
                }
                else
                {
                    bool isDeletionConfirmed = await DisplayAlert("Delete Task", $"Are you sure you want to delete the task \"{SwipeView.Task}\"?", "Yes", "No");
                    if (isDeletionConfirmed)
                    {
                        await _firebaseClient.Child("PersonalTask").Child(LoggedUser.Uid).Child($"{SwipeView.TaskId}").DeleteAsync();
                        await Toast.Make("Task successfully deleted", ToastDuration.Short).Show();
                        LoadTasksAsync();
                    }
                }
            }
        }
        catch(FirebaseException)
        {
            await Toast.Make("Firebase Error", ToastDuration.Long).Show();
        }
        catch(Exception)
        {
            await Toast.Make("Error", ToastDuration.Long).Show();
        }
    }
}
