using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Java.Lang;
using Java.Util;
using Taxi.EventListerners;
using Xamarin.Essentials;

namespace Taxi.Activities
{
    [Activity(Label = "@string/app_name", Theme= "@style/taxiTheme", MainLauncher=false)]
    public class RegistrationActivity : AppCompatActivity
    {
        Button registerButton;
        TextInputLayout passwordText;
        TextInputLayout fullNameText;
        TextInputLayout emailText;
        TextInputLayout phoneText;
        CoordinatorLayout rootView;
        TextView clickToLoginText;
        TaskCompletionListener TaskCompletionListener = new TaskCompletionListener();

        FirebaseDatabase database;
        FirebaseAuth mAuth;

        string fullname, phone, email, password;
        ISharedPreferences preferences = Application.Context.GetSharedPreferences("userinfo", FileCreationMode.Private);
        ISharedPreferencesEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.register);
            InitializeFirebase();
            mAuth = FirebaseAuth.Instance;
            ConnectControl();
        }

        void ConnectControl() 
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            clickToLoginText = (TextView)FindViewById(Resource.Id.clickToLogin);

            clickToLoginText.Click += ClickToLoginText_Click;
            registerButton.Click += RegisterButton_Click;
        }

        private void ClickToLoginText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(loginActivity));
            Finish();
        }
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            fullname = fullNameText.EditText.Text;
            email = emailText.EditText.Text;
            phone = phoneText.EditText.Text;
            password = passwordText.EditText.Text;

            if(fullname.Length < 3)
            {
                Snackbar.Make(rootView, "Please enter a valid name", Snackbar.LengthShort).Show();
                return;
            }
            else if(phone.Length < 10)
            {
                Snackbar.Make(rootView, "Please enter a valid phone number", Snackbar.LengthShort).Show();
                return;
            }
            else if(!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please enter a valid email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a password upto 8 characters", Snackbar.LengthShort).Show();
                return;
            }
            RegisterUser(fullname, phone, password, email);
        }

        void InitializeFirebase()
        {
            var app = FirebaseApp.InitializeApp(this);

            if (app == null)
            {
                var options = new FirebaseOptions.Builder()

                .SetApplicationId("taxi-9a19d")
                .SetApiKey("AIzaSyDZ6afI8brW-dYiyECqFDLTjP20YI8Ass0")
                .SetDatabaseUrl("https://taxi-9a19d.firebaseio.com")
                .SetStorageBucket("taxi-9a19d.appspot.com")
                .Build();

                app = FirebaseApp.InitializeApp(this, options);
                database = FirebaseDatabase.GetInstance(app);

            }
            else
            {
                database = FirebaseDatabase.GetInstance(app);
            }

        }

        void RegisterUser(string name, string phone, string password, string email) 
        {
            TaskCompletionListener.Success += TaskCompletionListener_Success;
            TaskCompletionListener.Failure += TaskCompletionListener_Failure;
            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, TaskCompletionListener)
                .AddOnFailureListener(this, TaskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "User Registration failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "User Registration was Successful", Snackbar.LengthShort).Show();

            HashMap userMap = new HashMap();
            userMap.Put("email", email);
            userMap.Put("phone", phone);
            userMap.Put("fullname", fullname);

            DatabaseReference userReference = database.GetReference("users/" + mAuth.CurrentUser.Uid);
            userReference.SetValue(userMap);
        }

        void SaveToSharedReference() 
        {
            editor = preferences.Edit();
            editor.PutString("email", email);
            editor.PutString("fullname", fullname);
            editor.PutString("phone", phone);

            editor.Apply();
        }

        void RetriveData()
        {
            string email = preferences.GetString("email", "");
        }
    }
}