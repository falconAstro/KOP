using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;
using TimeManagementApp.Services;
using TimeManagementApp.Resources.Languages;

namespace TimeManagementApp.Pages;

public partial class PersonalTasks : ContentPage
{
    private readonly FirebaseService _firebaseService;
    public ObservableCollection<PersonalTask> PersonalTaskList { get; set; } = []; //Zoznam Taskov nacitavanych z DB
    public User LoggedUser { get; set; } //Premenna (neskor) obsahujuca aktualne prihlaseneho usera

    public PersonalTasks(FirebaseService firebaseService) //Konstruktor stranky
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
            await LoadPersonalTasksToCollection();
            await Toast.Make(AppResources.LoadedDataToast, ToastDuration.Short).Show();
        }
        catch (FirebaseAuthException)
        {
            await Toast.Make(AppResources.ErrorToastFirebaseAuth, ToastDuration.Short).Show();
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

    private async Task LoadPersonalTasksToCollection()//Nacitanie taskov z pola do observable collection
    {
        PersonalTaskList.Clear();
        var _loadedTasks = await LoadPersonalTasksAsync();
        foreach (PersonalTask task in _loadedTasks)
        {
            PersonalTaskList.Add(task);
        }
    }

    private async Task<List<PersonalTask>> LoadPersonalTasksAsync()//Nacitanie taskov z databazy do pola
    {
        return (await _firebaseService.Client.Child("PersonalTask").Child(LoggedUser.Uid).OnceAsync<PersonalTask>()).Select(item => new PersonalTask
        {
            Task = item.Object.Task,
            TaskId = item.Key
        }).ToList();
    }

    private async Task CreateTaskAsync()//Vytvorenie jednoducheho tasku v databaze
    {
        try
        {
            await _firebaseService.Client.Child("PersonalTask").Child(LoggedUser.Uid).PostAsync(new PersonalTask { Task = EntryPersonalTask.Text });
            EntryPersonalTask.Text = string.Empty;
            await Toast.Make(AppResources.TaskToast, ToastDuration.Long).Show();
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

    private async void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(EntryPersonalTask.Text))
        {
            await Toast.Make(AppResources.TaskToastNull, ToastDuration.Short).Show();
            return;
        }
        await CreateTaskAsync();
        await LoadPersonalTasksToCollection();
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
                    await Toast.Make(AppResources.ErrorToastDelete, ToastDuration.Short).Show();
                    return;
                }
                bool isDeletionConfirmed = await DisplayAlert(AppResources.TaskToastDeletion, $"{AppResources.TaskDeletionConfirmation}\"{SwipeView.Task}\"?", AppResources.Yes,AppResources.No);
                if (isDeletionConfirmed)
                    {
                     await _firebaseService.Client.Child("PersonalTask").Child(LoggedUser.Uid).Child($"{SwipeView.TaskId}").DeleteAsync();
                     await Toast.Make(AppResources.TaskToastDelete, ToastDuration.Short).Show();
                     await LoadPersonalTasksToCollection();
                    }
            }
        }
        catch(FirebaseException)
        {
            await Toast.Make(AppResources.ErrorToastFirebase, ToastDuration.Long).Show();
        }
        catch(Exception)
        {
            await Toast.Make(AppResources.ErrorToast, ToastDuration.Long).Show();
        }
    }
}
