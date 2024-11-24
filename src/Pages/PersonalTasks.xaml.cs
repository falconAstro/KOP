using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;

namespace TimeManagementApp.Pages;

public partial class PersonalTasks : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    public class PersonalTask
    {
        public string Username { get; set; }
        public required string Task { get; set; }
        public string TaskId { get; set; }
    }
    public ObservableCollection<PersonalTask> PersonalTaskList { get; set; } = [];

    public PersonalTasks(FirebaseClient firebaseClient)
	{
		InitializeComponent();
        BindingContext = this;
        _firebaseClient = firebaseClient;
    }

    //Nacitanie taskov pri otvoreni stranky 
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
       
        await LoadTasksAsync();
    }

    //Funkcia nacitania taskov z databazy
    public async Task LoadTasksAsync()
    {
        PersonalTaskList.Clear();
        _firebaseClient.Child("PersonalTask").AsObservable<PersonalTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.TaskId = item.Key;
                PersonalTaskList.Add(item.Object);
            }
        });
    }

    //Vytvorenie kednoducheho tasku v databaze
    private async void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        await _firebaseClient.Child("PersonalTask").PostAsync(new PersonalTask{Task = EntryPersonalTask.Text});

        EntryPersonalTask.Text = string.Empty;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
    }
    //Odstranenie tasku aktualne nefunkcne
    private async void DeletePersonalTask(string Id)
    {
        await _firebaseClient.Child($"PersonalTask/{Id}").DeleteAsync();
        await LoadTasksAsync();
    }
}
