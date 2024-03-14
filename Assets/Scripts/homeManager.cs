using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomePageManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;

void Update()
{
    if (AuthManager.CurrentUser != null)
    {
        usernameText.text = AuthManager.CurrentUser.Username + "!";
    }
}

    public void gameRouting(){
         Debug.Log("game clicked");
         UnityEngine.SceneManagement.SceneManager.LoadScene("gamePage");

    }

}