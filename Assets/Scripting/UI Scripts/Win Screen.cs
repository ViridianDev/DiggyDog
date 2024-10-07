using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class WinScreen : MonoBehaviour
{
    public GameObject winScreenUI;
    public GameObject firstButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null); //deselects previous selection
        EventSystem.current.SetSelectedGameObject(firstButton); //selects resume button
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Start Screen");
    }
}