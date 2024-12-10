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
    public User User { get; set; } //Premenna (neskor) obsahujuca aktualne prihlaseneho usera

    //Inicializacia stranky
    public PersonalTasks(FirebaseClient firebaseClient, FirebaseAuthClient firebaseAuthClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseAuthClient = firebaseAuthClient;
        _firebaseClient = firebaseClient;
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
                item.Object.TaskID = item.Key;
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
    //Odstranenie tasku aktualne nefunkcne
    //private async void DeletePersonalTask(string TaskId)
    //{
    //    await _firebaseClient.Child("PersonalTask").Child(User.Uid).Child(TaskId).DeleteAsync();
    //    await LoadTasksAsync();
    //}

    //new Command DeleteTask(string TaskId,bool taskDeleted)
    //{
    //    _firebaseClient.Child("PersonalTask").Child(User.Uid).Child(TaskId).DeleteAsync();
    //    LoadTasksAsync();
        
    //}
}
