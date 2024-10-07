using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public bool isPaused = false; //variable that keeps track if the game is paused or not

    public GameObject pauseMenu;
    public GameObject pauseFirstButton;
    public AudioSource pauseSFX;
    public AudioSource selectSFX;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        pauseSFX = GetComponent<AudioSource>();
        selectSFX = transform.Find("Pause").GetComponent<AudioSource>();
    }
    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    public void PauseGame() //pauses game and shows Pause Panel
    {
        if (!isPaused)
        {
            GameObject.Find("Sound Manager").transform.Find("Music").GetComponent<AudioSource>().Pause();
            pauseSFX.Play();
            isPaused = true;
        }
        player.negateNextInput = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null); //deselects previous selection
        EventSystem.current.SetSelectedGameObject(pauseFirstButton); //selects resume button

    }
    public void ResumeGame() //Resumes game
    {
        pauseSFX.Play();
        isPaused = false;
        GameObject.Find("Sound Manager").transform.Find("Music").GetComponent<AudioSource>().UnPause();
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //deselects previous selection
    }

    public void RestartLevel()
    {
        pauseSFX.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }
    public void ExitGame() //Loads Title Screen
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start Screen");
    }
}
