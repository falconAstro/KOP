using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using TimeManagementApp.Classes;

namespace TimeManagementApp.Pages;

public partial class PersonalTasks : ContentPage
{
    private readonly FirebaseClient firebaseClient;

    public ObservableCollection<PersonalTask> PersonalTaskList { get; set; } = [];

    public PersonalTasks(FirebaseClient firebaseClient)
	{
		InitializeComponent();
        BindingContext = this;
        this.firebaseClient = firebaseClient;
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
        this.firebaseClient.Child("PersonalTask").AsObservable<PersonalTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                item.Object.Id = item.Key;
                PersonalTaskList.Add(item.Object);
            }
        });
    }

    //Vytvorenie kednoducheho tasku v databaze
    private async void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        await this.firebaseClient.Child("PersonalTask").PostAsync(new PersonalTask{Task = EntryPersonalTask.Text});

        EntryPersonalTask.Text = string.Empty;

        await Shell.Current.DisplayAlert("", "Task created", "OK");
    }
    private async void DeletePersonalTask(string Id)
    {
        await this.firebaseClient.Child($"PersonalTask/{Id}").DeleteAsync();
        await LoadTasksAsync();
    }
}
