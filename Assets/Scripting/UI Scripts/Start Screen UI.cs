using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartScreenUI : MonoBehaviour
{
    public GameObject FirstButton;
    public string mainGame;
    public GameObject credits;
    public GameObject creditsOpenButton;
    public GameObject creditsCloseButton;


    public void Start()
    {
        credits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null); //deselects previous selection
        EventSystem.current.SetSelectedGameObject(FirstButton); //selects resume button
    }

    public void CloseGame()
   {
        //close game
        Application.Quit();
   }

    public void StartGame()
    {
        //start game
        SceneManager.LoadScene(mainGame);
    }

    public void ShowCredits()
    {
        credits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(creditsOpenButton); //selects resume button
        //credits
    }

    public void CloseCredits()
    {
        credits.SetActive(false);
        EventSystem.current.SetSelectedGameObject(creditsCloseButton); //selects resume button
    }
}
