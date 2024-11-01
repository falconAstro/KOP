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
        this.firebaseClient.Child("PersonalTask").AsObservable<PersonalTask>().Subscribe((item) =>
        {
            if (item.Object != null)
            {
                PersonalTaskList.Add(item.Object);
            }
        });
    }

    //Vytvorenie kednoducheho tasku v databaze
    private void BtnCreatePersonalTask_Clicked(object sender, EventArgs e)
    {
        this.firebaseClient.Child("PersonalTask").PostAsync(new PersonalTask
        {
            Task = EntryPersonalTask.Text
        });

        EntryPersonalTask.Text = string.Empty;
    }
}
