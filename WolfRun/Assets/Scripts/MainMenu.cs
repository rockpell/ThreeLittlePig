using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject tutorialObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showTutorial()
    {
        tutorialObject.SetActive(true);
    }

    public void gameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void gameExit()
    {
        Application.Quit(0);
    }
}
