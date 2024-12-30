using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TimeManagementApp.Classes;
namespace TimeManagementApp.Pages;

public partial class PersonalTasks : ContentPage
{
    //Firebase clients
    private readonly FirebaseAuthClient _firebaseAuthClient;
    private readonly FirebaseClient _firebaseClient;
    public ObservableCollection<PersonalTask> PersonalTaskList { get; set; } = []; //Zoznam Taskov nacitavanych z DB
    public User User { get; set; } //Premenna (neskor) obsahujuca aktualne prihlaseneho usera

    //Inicializacia stranky
    public PersonalTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
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
        base.OnNavigatedTo(args);
        User = _firebaseAuthClient.User;
        await LoadTasksAsync();
    }

    //Funkcia nacitania taskov z databazy
    public async Task LoadTasksAsync()
    {
        PersonalTaskList.Clear();
        _firebaseClient.Child("PersonalTask").Child(User.Uid).AsObservable<PersonalTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.TaskId = item.Key;
                PersonalTaskList.Add(item.Object);
            }
        });
    }

    //Vytvorenie jednoducheho tasku v databaze
    private async void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("PersonalTask").Child(User.Uid).PostAsync(new PersonalTask{Task = EntryPersonalTask.Text});

        EntryPersonalTask.Text = string.Empty;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
    }
    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem)
        {
            var SwipeView = swipeItem.BindingContext as PersonalTask;
            if (SwipeView == null)
            {
                await DisplayAlert("Error", "Failed to identify the item to delete.", "OK");
            }
            else
            {
                bool isDeletionConfirmed = await DisplayAlert("Delete Task", $"Are you sure you want to delete the task \"{SwipeView.Task}\"?", "Yes", "No");
                if (isDeletionConfirmed)
                {
                    await _firebaseClient.Child("PersonalTask").Child(User.Uid).Child($"{SwipeView.TaskId}").DeleteAsync();
                    await LoadTasksAsync();
                }
            }
        }
    }
}
