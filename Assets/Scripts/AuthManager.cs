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
using System.Threading.Tasks;


public class AuthManager : MonoBehaviour
{
    public Text logText;
    public TextMeshProUGUI username, email, password, forgetEmail, newPassword;
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
                //Debug.Log("User created successfully: " + CurrentUser.Username + " userId: " + CurrentUser.UserId + " userEmail: " + CurrentUser.Email);
                //CurrentUser.Score = 0; // Set the initial score if needed

                // Store the user data in Firestore
                 firestoreManager.StoreUserData(CurrentUser);

                Debug.Log($"User created successfully: {CurrentUser.Username}, userId: {CurrentUser.UserId}, userEmail: {CurrentUser.Email}");


                // Load the home page scene
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomePage");
            
            });
        });
    }
    


    public void OnClickLogin(){
    
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            logText.text = "Fields are empty";
            return;
        }
        else
        {
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
            });
        }

        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            logText.text = "Fields are empty";
            return;
        }else{
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
                         logText.text = firebaseEx.Message;
                    }
                }
               return;
            }
            else{
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
        }
        });
    }
    }

    public void OnClickLogout(){
        Debug.Log("Logout button clicked");
        auth.SignOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
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

    public void OnClickForgetPassword()
    {
        string email = forgetEmail.text;
        Debug.Log(email); // Get the email entered by the user

        var db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("User_Details").Document(email);
       // StartCoroutine(FetchUserData(docRef));
    
       
//    private IEnumerator FetchUserData(DocumentReference docRef)
// {
//     Task<DocumentSnapshot> fetchTask = docRef.GetSnapshotAsync();

//     while (!fetchTask.IsCompleted)
//     {
//         yield return null; // Wait until the task is completed
//     }

//     if (fetchTask.IsFaulted)
//     {
//         Debug.LogError($"Failed to retrieve user data: {fetchTask.Exception}");
//         yield break;
//     }

//     DocumentSnapshot snapshot = fetchTask.Result;

//     if (snapshot.Exists)
//     {
//         // Get the email field from the snapshot
//         string userEmail = snapshot.GetValue<string>("email");
//         Debug.Log($"User data retrieved successfully: Email = {userEmail}");
//         // Call the method to send the password reset email
//         SendPasswordResetEmail(userEmail);
//     }
//     else
//     {
//         Debug.Log("No such user.");
//         logText.text = "Email is not registered with an account.";
//         forgetEmail.text = "";
//     }
// }


        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to retrieve user data: {task.Exception}");
                return;
            }
            else if (task.Result.Exists)
            {
                Dictionary<string, object> userData = task.Result.ToDictionary();
                //string email = userData["email"].ToString();
                string userEmail = userData["email"].ToString();

                Debug.Log($"User data retrieved successfully: Email = {userEmail}");
                SendPasswordResetEmail(userEmail);
                
                
            }
            else
            {
                Debug.Log("No such user.");
                logText.text="Email is not registered with an account.";
                forgetEmail.text = "";
                
            }
        });
        
        }


    // var db = FirebaseFirestore.DefaultInstance;
    // var docRef = db.Collection("YOUR_COLLECTION_NAME").Document(email);

    // docRef.GetSnapshotAsync().ContinueWith(task =>
    // {
    //     if (task.IsCompleted)
    //     {
    //         DocumentSnapshot snapshot = task.Result;
    //         if (snapshot.Exists)
    //         {
    //             Debug.LogFormat("Document data for {0} document:", snapshot.Id);
    //             Dictionary<string, object> documentData = snapshot.ToDictionary();
    //             // Email exists in the Firestore collection
    //             // You can now proceed with your logic here
    //             SendPasswordResetEmail(email);

    //         }
    //         else
    //         {
    //             logText.text = "Email is not registered with an account.";
    //             Debug.LogFormat("Document {0} does not exist!", snapshot.Id);
    //             // Email does not exist in the Firestore collection
    //             // You can now proceed with your logic here
                
    //             forgetEmail.text = "";
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Error checking email: " + task.Exception);
    //     }
    // });

    
//}

        // // Check if the email belongs to an authenticated user

        //  FirebaseAuth.DefaultInstance.FetchSignInMethodsForEmailAsync(forgetEmail.text).ContinueWith(task =>
        // {
        //     if (task.IsFaulted)
        //     {
        //         Debug.LogError("Error checking email: " + task.Exception);
        //         return;
        //     }

        //     if (task.Result.SignInMethods.Contains("password"))
        //     {
        //         // Email belongs to an authenticated user
        //         SendPasswordResetEmail(email);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("Email does not belong to an authenticated user.");
        //         logText.Text = "Email is not registered with an account.";
        //         // Provide feedback to the user indicating that the email does not belong to an authenticated user
        //     }
        // });
    

    public void SendPasswordResetEmail(string email)
    {
         FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Error sending password reset email: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
            // Redirect the user to the reset password page within the app
            UnityEngine.SceneManagement.SceneManager.LoadScene("forgotpw2");
            //SceneManager.LoadScene("resetpw");
        });
    }

    public void UpdatePassword()
    {
        //string newPassword = newPassword.text;
        FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.UpdatePasswordAsync(newPassword.text).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdatePasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Error updating password: " + task.Exception);
                    return;
                }

                Debug.Log("Password updated successfully.");
                // Redirect the user to the login page
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            });
        }
    }


}