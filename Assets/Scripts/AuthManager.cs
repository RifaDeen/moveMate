using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Firestore;
using UnityEngine.UIElements;
using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;


public class AuthManager : MonoBehaviour
{
    public Text logText;
    public TextMeshProUGUI username, email, password;
    public UnityEngine.UI.Button signupButton; // Specify the namespace for Button

    public static User CurrentUser { get; private set;}

    Firebase.Auth.FirebaseAuth auth; // Declare the 'auth' variable
    FirestoreManager firestoreManager; // Instance of the FirestoreManager
    void InitializeFirebase() {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }


    // Start is called before the first frame update
    void Start()
    {
       Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Initialize the Firebase app
                InitializeFirebase();
                firestoreManager = new FirestoreManager();
                Debug.Log("Firebase app initialized");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

    }

    // public void loginRouting(){
    //      Debug.Log("login clicked");
    //      UnityEngine.SceneManagement.SceneManager.LoadScene("Login");

    // }

    // public void signupRouting(){
    //      Debug.Log("signup clicked");
    //      UnityEngine.SceneManagement.SceneManager.LoadScene("signup");
    // }


    public void OnClickSignup()
    {     
        Debug.Log("Clicked Signup");

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email.text.Trim(), password.text.Trim())
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("Signup cancelled");
                logText.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Signup faulted");
                foreach (var exception in task.Exception.InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
                        logText.text = "An error occurred, please try again";
                    }
                } 
            }

            // Firebase user has been created.
            Firebase.Auth.AuthResult authResult = task.Result;
            Firebase.Auth.FirebaseUser newUser = authResult.User;
            Debug.Log("Signup successful");

            // Set additional user information (username) in the user profile
            UserProfile userProfile = new UserProfile { DisplayName = username.text };
            newUser.UpdateUserProfileAsync(userProfile).ContinueWithOnMainThread(updateProfileTask =>
            {
                if (updateProfileTask.IsFaulted)
                {
                    Debug.LogError("Failed to update user profile: " + updateProfileTask.Exception);
                }
                else
                {
                    Debug.Log("User profile updated successfully.");
                }
                string username = newUser.DisplayName ?? "";
                
                CurrentUser = new User(newUser.UserId, username, newUser.Email);
            
                // Store the user data in Firestore
                firestoreManager.StoreUserData(CurrentUser);
                Debug.Log($"User created successfully: {CurrentUser.Username}, userId: {CurrentUser.UserId}, userEmail: {CurrentUser.Email}");


                // Load the home page scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
            
            });
        });
    }
    


    public void OnClickLogin(){

        Debug.Log("Clicked Login");
    
        FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.Log("Login faulted");
                string mail = email.text;
                foreach (var exception in task.Exception.InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
                        logText.text = "An error occurred, please try again";
                    }
                }
            }
        
    
            Debug.Log("login successful");
            // Firebase user has been created.
            Firebase.Auth.AuthResult authResult = task.Result;
            Firebase.Auth.FirebaseUser newUser = authResult.User;

            // Access additional user information (username) from the user profile
            string username = newUser.DisplayName ?? "";
            Debug.LogFormat("User signed in successfully: {0} ({1})", username, newUser.UserId);
            CurrentUser = new User(newUser.UserId, username, newUser.Email);

            // Load the home page scene or perform any other post-login actions
            UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
    
        });
    }
    
    void AuthStateChanged(object sender, System.EventArgs eventArgs) {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                    && auth.CurrentUser.IsValid();
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
                username.SetText(user.DisplayName ?? ""); // Assign the user.DisplayName value to the TextMeshProUGUI component
                string emailAddress = user.Email ?? ""; // Declare and assign the email address
            
            }
        }
    }

    //Forgot password codes

     private void showNotificationMessage(string title, string message)
    {
        Debug.Log($"{title}: {message}");
    }

    private string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Email is missing.";
            case AuthError.MissingPassword:
                return "Password is missing.";
            default:
                return "An unknown error occurred.";
        }
    }
    void forgetPasswordSubmit(string forgetPasswordEmail){
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                {
                    Firebase.FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        // Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
                        // logText.text = firebaseEx.Message;
                        var errorCode=(AuthError)firebaseEx.ErrorCode;
                        showNotificationMessage("Error",GetErrorMessage(errorCode));
                    }
                }
            }
            Debug.Log("Password reset email sent successfully.");
            showNotificationMessage("Success","Password reset email sent successfully.");
        });
    }
    public void OnClickForgetPassword()
    {
        // Get the user's email address from the email input field.
        string forgetPasswordEmail = email.text;
        // Call the forgetPasswordSubmit method with the user's email address.
        forgetPasswordSubmit(forgetPasswordEmail);
    }

}
     
     



