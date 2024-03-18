// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Firebase.Auth;
// using Firebase.Extensions;
// using UnityEngine.UI;
// using TMPro;
// using Firebase;
// using Firebase.Firestore;
// using UnityEngine.UIElements;
// using System;

// public class AuthManager : MonoBehaviour
// {
//     public Text logText;
//     public TextMeshProUGUI username, email, password;
//     public UnityEngine.UI.Button signupButton; // Specify the namespace for Button

//     public static User CurrentUser { get; private set;}

//     Firebase.Auth.FirebaseAuth auth; // Declare the 'auth' variable
//     FirestoreManager firestoreManager; // Instance of the FirestoreManager
//     void InitializeFirebase() {
//         auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
//         auth.StateChanged += AuthStateChanged;
//         AuthStateChanged(this, null);
//     }

//     // Start is called before the first frame update
//     void Start()
//     {
//        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
//             var dependencyStatus = task.Result;
//             if (dependencyStatus == Firebase.DependencyStatus.Available)
//             {
//                 // Initialize the Firebase app
//                 InitializeFirebase();
//                 firestoreManager = new FirestoreManager();
//                 Debug.Log("Firebase app initialized");
//             }
//             else
//             {
//                 UnityEngine.Debug.LogError(System.String.Format(
//                   "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
//             }
//         });
//     }

//     public void loginRouting(){
//          Debug.Log("login clicked");
//          UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
//     }

//     public void signupRouting(){
//          Debug.Log("signup clicked");
//          UnityEngine.SceneManagement.SceneManager.LoadScene("signup");
//     }

//     public void OnClickSignup()
//     {     
//         Debug.Log("Clicked Signup");
        
    
//         if (isFilled())
//         {
//         FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email.text.Trim(), password.text.Trim())
//         .ContinueWithOnMainThread(task =>
//         {
            
//             // if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text) || string.IsNullOrEmpty(username.text))
//             // {
//             //     Debug.LogError("Email, password, or username is empty.");
//             //     logText.text = "Email, password, or username is empty.";
//             //     return;
//             // }
//             //     Debug.Log("Validation passed. Proceeding with signup.");
//             if (task.IsCanceled)
//             {
//                 Debug.Log("Signup cancelled");
//                 logText.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
//                 return;
//             }
//             if (task.IsFaulted)
//             {
//                 Debug.Log("Signup faulted");
//                 foreach (var exception in task.Exception.InnerExceptions)
//                 {
//                     FirebaseException firebaseEx = exception as FirebaseException;
//                     if (firebaseEx != null)
//                     {

//                         // Add more validation if needed, for example, password length
//                         // if (password.text.Length < 8)
//                         // {
//                         //     Debug.LogError("Password should be at least 8 characters long.");
//                         //     logText.text = "Password should be at least 8 characters long.";
//                         //     return;
//                         // }
//                         Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
//                         var errorCode=(AuthError)firebaseEx.ErrorCode;
//                         logText.text = GetErrorMessage(errorCode);
//                         //logText.text = firebaseEx.Message;
//                         // username.text = "";
//                         // email.text = "";
//                         // password.text = "";
//                          Start();
//                     }
//                 }
//                 return;
//             }

//             // Add success message if needed
//             Debug.Log("Signup successful");

//             Firebase.Auth.AuthResult authResult = task.Result;
//             Firebase.Auth.FirebaseUser newUser = authResult.User;

//             UserProfile userProfile = new UserProfile { DisplayName = username.text };
//             newUser.UpdateUserProfileAsync(userProfile).ContinueWithOnMainThread(updateProfileTask =>
//             {
//                 if (updateProfileTask.IsFaulted)
//                 {
//                     Debug.LogError("Failed to update user profile: " + updateProfileTask.Exception);
//                 }
//                 else
//                 {
//                     Debug.Log("User profile updated successfully.");
//                 }
                
//                 string username = newUser.DisplayName ?? "";
//                 CurrentUser = new User(newUser.UserId, username, newUser.Email);

//                 firestoreManager.StoreUserData(CurrentUser);

//                 Debug.Log($"User created successfully: {CurrentUser.Username}, userId: {CurrentUser.UserId}, userEmail: {CurrentUser.Email}");

//                 UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
//             });
//         });
//         }
//         else{
//             Debug.Log("Validation failed. Please fill all the fields.");
//             logText.text = "Validation failed. Please fill all the fields.";
//         }
//     }
    
//     public void OnClickLogin()
//     {
//         if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
//         {
//             Debug.LogError("Email and/or password is empty.");
//             logText.text = "Email and/or password is empty.";
//             return;
//         }

//         FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
//         {
//             if (task.IsCanceled)
//             {
//                 Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
//                 return;
//             }
//             if (task.IsFaulted)
//             {
//                 Debug.Log("Login faulted");
//                 foreach (var exception in task.Exception.InnerExceptions)
//                 {
//                     FirebaseException firebaseEx = exception as FirebaseException;
//                     if (firebaseEx != null)
//                     {
//                         Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
//                         var errorCode=(AuthError)firebaseEx.ErrorCode;
//                         logText.text = GetErrorMessage(errorCode);
//                        // Start();
//                     }
//                 }
//                 return;
//             }

//             Debug.Log("login successful");

//             Firebase.Auth.AuthResult authResult = task.Result;
//             Firebase.Auth.FirebaseUser newUser = authResult.User;

//             string username = newUser.DisplayName ?? "";
//             Debug.LogFormat("User signed in successfully: {0} ({1})", username, newUser.UserId);
//             CurrentUser = new User(newUser.UserId, username, newUser.Email);

//             UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
//         });
//     }

    
//     void AuthStateChanged(object sender, System.EventArgs eventArgs) {
//         Firebase.Auth.FirebaseUser user = auth.CurrentUser;
//         if (auth.CurrentUser != user) {
//             bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
//                     && auth.CurrentUser.IsValid();
//             if (!signedIn && user != null) {
//                 Debug.Log("Signed out " + user.UserId);
//             }
//             user = auth.CurrentUser;
//             if (signedIn) {
//                 Debug.Log("Signed in " + user.UserId);
//                 username.SetText(user.DisplayName ?? ""); 
//                 string emailAddress = user.Email ?? ""; 
//             }
//         }
//     }

//     private static string GetErrorMessage(AuthError errorCode)
//     {
//         var message = "";
//         switch (errorCode)
//         {
//             case AuthError.AccountExistsWithDifferentCredentials:
//                 message = "Account doesn't exist with the specified credentials";
//                 break;
//             case AuthError.MissingPassword:
//                 message = "Password is missing";
//                 break;
//             case AuthError.WeakPassword:
//                 message = "The password is weak";
//                 break;
//             case AuthError.WrongPassword:
//                 message = "Incorrect password";
//                 break;
//             case AuthError.EmailAlreadyInUse:
//                 message = "The email address is already in use by another user";
//                 break;
//             case AuthError.InvalidEmail:
//                 message = "The email address is invalid";
//                 break;
//             case AuthError.MissingEmail:
//                 message = "Email is missing";
//                 break;
//             default:
//                 message = "Some error occurred";
//                 break;
//         }
//         return message; 
//     }

//     public bool isFilled()
//     {
//         if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text) || string.IsNullOrEmpty(username.text))
//         {
//             return false;
//         }
//         return true;
//     } 
// }

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

    public void loginRouting(){
         Debug.Log("login clicked");
         UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }

    public void signupRouting(){
         Debug.Log("signup clicked");
         UnityEngine.SceneManagement.SceneManager.LoadScene("signup");
    }

    public void OnClickSignup()
    {     
        Debug.Log("Clicked Signup");
        
    
        if (isFilled())
        {
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
                            var errorCode=(AuthError)firebaseEx.ErrorCode;
                            logText.text = GetErrorMessage(errorCode);
                            ClearFields(); // Clear the input fields
                        }
                    }
                    return;
                }

                // Add success message if needed
                Debug.Log("Signup successful");

                Firebase.Auth.AuthResult authResult = task.Result;
                Firebase.Auth.FirebaseUser newUser = authResult.User;

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

                    firestoreManager.StoreUserData(CurrentUser);

                    Debug.Log($"User created successfully: {CurrentUser.Username}, userId: {CurrentUser.UserId}, userEmail: {CurrentUser.Email}");

                    UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
                });
            });
        }
        else{
            Debug.Log("Validation failed. Please fill all the fields.");
            logText.text = "Validation failed. Please fill all the fields.";
        }
    }
    
    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            Debug.LogError("Email and/or password is empty.");
            logText.text = "Email and/or password is empty.";
            return;
        }

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
                foreach (var exception in task.Exception.InnerExceptions)
                {
                    FirebaseException firebaseEx = exception as FirebaseException;
                    if (firebaseEx != null)
                    {
                        Debug.LogError($"Error code: {firebaseEx.ErrorCode}, Message: {firebaseEx.Message}");
                        var errorCode=(AuthError)firebaseEx.ErrorCode;
                        logText.text = GetErrorMessage(errorCode);
                        ClearFields(); // Clear the input fields
                    }
                }
                return;
            }

            Debug.Log("login successful");

            Firebase.Auth.AuthResult authResult = task.Result;
            Firebase.Auth.FirebaseUser newUser = authResult.User;

            string username = newUser.DisplayName ?? "";
            Debug.LogFormat("User signed in successfully: {0} ({1})", username, newUser.UserId);
            CurrentUser = new User(newUser.UserId, username, newUser.Email);

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
                username.SetText(user.DisplayName ?? ""); 
                string emailAddress = user.Email ?? ""; 
            }
        }
    }

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "Account doesn't exist with the specified credentials";
                break;
            case AuthError.MissingPassword:
                message = "Password is missing";
                break;
            case AuthError.WeakPassword:
                message = "The password is weak";
                break;
            case AuthError.WrongPassword:
                message = "Incorrect password";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "The email address is already in use by another user";
                break;
            case AuthError.InvalidEmail:
                message = "The email address is invalid";
                break;
            case AuthError.MissingEmail:
                message = "Email is missing";
                break;
            default:
                message = "Some error occurred";
                break;
        }
        return message; 
    }

    public bool isFilled()
    {
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text) || string.IsNullOrEmpty(username.text))
        {
            return false;
        }
        return true;
    } 

    private void ClearFields()
    {
        email.text = "";
        password.text = "";
        username.text = "";
    }
}
