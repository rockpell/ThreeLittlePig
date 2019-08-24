using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialObject;

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
        tutorialObject[0].SetActive(true);
    }

    public void gameStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void gameExit()
    {
        Application.Quit(0);
    }

    public void backButton(int index)
    {
        tutorialObject[index].SetActive(false);
    }

    private void allHideTutorial(int index)
    {
        for (int i = 0; i < index + 1; i++)
        {
            tutorialObject[i].SetActive(false);
        }
    }

    public void nextButton(int index)
    {
        if(index == 5)
        {
            allHideTutorial(5);
        }
        else
        {
            tutorialObject[index + 1].SetActive(true);
        }
    }
}
