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
using Taxi.EventListerners;
using Xamarin.Essentials;

namespace Taxi.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/taxiTheme", MainLauncher =false)]
    public class loginActivity : AppCompatActivity
    {
        TextInputLayout emailText;
        TextInputLayout passwordtext;
        Button loginButton;
        TextView clickToRegisterText;
        CoordinatorLayout rootView;
        FirebaseAuth mAuth;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.login);

            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordtext = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            clickToRegisterText = (TextView)FindViewById(Resource.Id.clickToRegisterText);

            clickToRegisterText.Click += ClickToRegisterText_Click;
            loginButton.Click += LoginButton_click;
            InitializeFirebase();
        }

        private void ClickToRegisterText_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
            Finish();
        }

        private void LoginButton_click(object sender, EventArgs e)
        {
            string email, password;

            email = emailText.EditText.Text;
            password = passwordtext.EditText.Text;

            if (!email.Contains("@")) 
            {
                Snackbar.Make(rootView, "Please enter a valid email", Snackbar.LengthShort).Show();
                return;
            }
            else if(password.Length < 8) 
            {
                Snackbar.Make(rootView, "Please provide a valid password", Snackbar.LengthShort).Show();
                return;
            }

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(taskCompletionListener)
                .AddOnFailureListener(taskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "Login Failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
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
                mAuth = FirebaseAuth.Instance;

            }
            else
            {
                mAuth = FirebaseAuth.Instance;
            }

        }
    }
}