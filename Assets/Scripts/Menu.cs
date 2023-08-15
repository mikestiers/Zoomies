using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject carStats;
    public GameObject carSelectionMenu;
    public GameObject carSelector;
    public GameObject startMenu;
    public Button startButton;
    public Button quitButton;
    public Button raceButton;

    void Start()
    {
        startButton.onClick.AddListener(CarSelection);
        quitButton.onClick.AddListener(Quit);
        raceButton.onClick.AddListener(StartRace);
    }

    public void CarSelection()
    {
        startMenu.SetActive(false);
        carSelectionMenu.SetActive(true);
        carStats.SetActive(true);
        carSelector.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void StartRace()
    {
        SceneManager.LoadScene("Race");
    }    
}
