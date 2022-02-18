using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Board board;
    public Text scoreText;
    public Button pauseButton;
    public Button quitButton;
    public Button resetButton;
    public Button resumeGame;
    

    private bool isGamePause;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject noMoreMoveMenu;
 
   
    void Update()
    {
        scoreText.text = board.playerScore.ToString();
    }

    public void PauseGame() 
    {
        pauseMenu.SetActive(true);
        mainMenu.SetActive(false);
        Time.timeScale = 0;
        isGamePause = true;
        SoundManager.instance.StopThemeMusic();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log(" Quit Game");
    }

    public void ResetGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        ResumeGame();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        mainMenu.SetActive(true);
        Time.timeScale = 1f;
        isGamePause = false;
        SoundManager.instance.PlayThemeMusic();
    }
    public void LoseGame()
    {
        mainMenu.SetActive(false);
        noMoreMoveMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePause = true;
    }
}
