using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTestScript : MonoBehaviour
{
    


    public Button startButton;
    public Button creditButton;
    public Button backButton;
    public GameObject credits;



    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        creditButton.onClick.AddListener(Credits);
        backButton.onClick.AddListener(Back);
    }





    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Credits()
    {
        credits.SetActive(true);
    }
    public void Back()
    {
        credits.SetActive(false);
    }
}
