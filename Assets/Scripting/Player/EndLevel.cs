using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndLevel : MonoBehaviour
{
    private PlayerController player;
    public bool canPlay;
    public bool isRestarting = false;

    [SerializeField] private string currentLevel;
    [SerializeField] private string nextLevel;
    [SerializeField] private TextMeshProUGUI levelName;

    private void Awake()
    {
        canPlay = false;
        isRestarting = false;
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }

    public void BeginCurrentLevel()
    {
        canPlay = true;
        player.keys = 0;
        player.health = 3;
    }

    public void GoToNextLevel()
    {
        if (!isRestarting)
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            SceneManager.LoadScene(currentLevel);
        }
    }

    public void Start()
    {
        transform.GetComponent<Animation>().clip = transform.GetComponent<Animation>().GetClip("IrisOut");
        transform.GetComponent<Animation>().Play();
        currentLevel = SceneManager.GetActiveScene().name;

        switch (currentLevel)
        {
            case "Level0":
                levelName.text = "Tutorial Tunnel";
                nextLevel = "Level1";
                break;

            case "Level1":
                levelName.text = "Quiet Quarry";
                nextLevel = "Level2";
                break;

            case "Level2":
                levelName.text = "Glacial Grotto";
                nextLevel = "Level3";
                break;

            case "Level3":
                levelName.text = "Caldera Chasm";
                nextLevel = "Win Screen";
                break;
        }
    }
}
