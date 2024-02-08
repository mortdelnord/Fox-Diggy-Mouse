using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.AI;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI pauseScoreText;
    public TextMeshProUGUI gameTimerText;

    public Button pauseButton;
    public Button backButton;
    public Button restartButton;
    public Button mainMenuButton;
    public GameObject pauseMenu;
    public GameObject scoreWord;
    public GameObject pauseButtonContainer;
    public GameObject scoreContainer;
    public GameObject timeContainer;

    public RawImage blackscreen;


    [Header ("GameManaging")]

    public int scoreNum = 0;
    public float preySpeedIncrease = 5f;
    public float preySpeed = 1f;

    public float range;

    private float gameTimer = 10f;
    public float maxTime;

    public bool isStarted = false;
    public bool stopTimer;


    public List<GameObject> preyList; 

    private void Start()
    {
        isStarted = false;
        Time.timeScale = 1f;
        gameTimer = maxTime;
        scoreText.text = scoreNum.ToString();
        pauseButton.onClick.AddListener(PauseGame);
        backButton.onClick.AddListener(UnPause);
        restartButton.onClick.AddListener(Restart);
        mainMenuButton.onClick.AddListener(MainMenu);
        SpawnPrey();
    }

    private void Update()
    {

        if (isStarted) // game has started count down the timer
        {
            if (!stopTimer)
            {
                gameTimer -= Time.deltaTime;
            }
        }else // fade the screeen from black
        {
            blackscreen.color = new Color(blackscreen.color.r, blackscreen.color.g, blackscreen.color.b, blackscreen.color.a - 0.01f);
            if (blackscreen.color.a <= 0f)
            {
                isStarted = true;
            }
        }

        int timerInt = (int)gameTimer; // get the timer number as an int
        gameTimerText.text = timerInt.ToString(); // update the timer text to int version of timer
        if (gameTimer <= 0f) // if can timer runs out, pause the game 
        {
            PauseGame();
            backButton.gameObject.SetActive(false);
        }
    }

    public void UpdateScore(int newPoints) // updates the score
    {
        gameTimer = maxTime + 1;
        scoreNum += newPoints;
        scoreText.text = scoreNum.ToString();
        pauseScoreText.text = scoreText.text;
        pauseScoreText.text = scoreText.text;
    }


    public void SpawnPrey()
    {
        Vector3 point;
       // Debug.Log("method spawnprey");
        if (RandomPoint(transform.position, range, out point)) // if a random point is on the navmesh of the prey
        {
            //Debug.Log("found point on mesh");
            Vector3 spawnPoint = new Vector3(point.x, -.4f, point.z); // get a spawn point
            GameObject prey = Instantiate(NewPrey(), spawnPoint, transform.rotation); // instatiate a new random prey
            if (scoreNum % 5 == 0)
            {
                //Debug.Log("increase speed");
                preySpeed += preySpeedIncrease;
            }
            prey.GetComponent<NavMeshAgent>().speed = preySpeed;
            //Debug.Log("Spawned new prey");
        }


    }


    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {

            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.GetAreaFromName("PreyArea")))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }


    private GameObject NewPrey() // returns a game object prey that is random
    {
        int randInt = Random.Range(0, preyList.Count);
        //Debug.Log(randInt);
        GameObject newPrey = preyList[randInt];
        //Debug.Log(newPrey);
        return newPrey;
    }

    private void PauseGame()
    {
        scoreContainer.SetActive(false);
        //pauseButton.gameObject.SetActive(false);
        pauseButtonContainer.SetActive(false);
        timeContainer.SetActive(false);
        //scoreWord.SetActive(false);       
        Time.timeScale = 0f;
        pauseButton.enabled = false;

        pauseMenu.SetActive(true);
    }
    private void UnPause()
    {
        scoreContainer.SetActive(true);
        //pauseButton.gameObject.SetActive(true);
        pauseButtonContainer.SetActive(true);
        pauseButton.GetComponent<Button>().enabled = true;
        timeContainer.SetActive(true);
        scoreWord.SetActive(true);
        
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }



}
