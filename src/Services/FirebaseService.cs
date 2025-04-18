using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;

namespace TimeManagementApp.Services
{
    public class FirebaseService
    {
        private FirebaseClient _firebaseClient;
        private FirebaseAuthClient _firebaseAuthClient;
        public FirebaseService()
        {
            InitializeFirebaseClient();
            InitializeFirebaseAuthClient();
        }
        private void InitializeFirebaseAuthClient()
        {
            _firebaseAuthClient = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = SecureStorage.GetAsync("fireBaseAuthApiKey").Result ?? string.Empty,
                AuthDomain = "timemanagement-4d83d.web.app",
                Providers = [new EmailProvider()]
            });

        }
        private void InitializeFirebaseClient()
        {
            _firebaseClient = new FirebaseClient("https://timemanagement-4d83d-default-rtdb.firebaseio.com/",
            new FirebaseOptions()
            {
                AuthTokenAsyncFactory = () => _firebaseAuthClient.User.GetIdTokenAsync() //Refresh tokenu
            });
        }
        public FirebaseClient Client => _firebaseClient;
        public FirebaseAuthClient AuthClient => _firebaseAuthClient;
    }
}
