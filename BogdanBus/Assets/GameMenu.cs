using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (GameEnd.Singletone.IsGameEnd && Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene("Level1");
        }
    }
}
