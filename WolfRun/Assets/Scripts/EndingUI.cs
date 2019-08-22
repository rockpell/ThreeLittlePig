using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingUI : MonoBehaviour
{
    [SerializeField] private Text headText;
    [SerializeField] private Text crimeSubjectText;
    [SerializeField] private Text crimeDetailText;

    [SerializeField] private GameObject[] buttons;

    private string crimeSubejct;
    private string crimeDetail;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        crimeSubjectText.text = "";
        crimeDetailText.text = "";
        headText.text = "";

        buttons[0].SetActive(false);
        buttons[1].SetActive(false);

        //StartCoroutine(sequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void settingGameEnding(GameEnding gameEnding)
    {
        switch (gameEnding)
        {
            case GameEnding.THEFT:
                StartCoroutine(theftSequence());
                break;
            case GameEnding.ARREST:
                StartCoroutine(arrestSequence());
                break;
            case GameEnding.MEAT:
                StartCoroutine(meatSequence());
                break;
        }
    }

    private IEnumerator arrestSequence()
    {
        for(int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.5f);

            addCrimeText(i);
            crimeSubjectText.text = crimeSubejct;
            crimeDetailText.text = crimeDetail;
        }

        yield return new WaitForSeconds(0.5f);

        headText.text = "징역 " + (gameManager.Score / 100) + "년";

        yield return new WaitForSeconds(1.0f);

        buttons[0].SetActive(true);
        buttons[1].SetActive(true);
    }

    private void addCrimeText(int index)
    {
        switch (index)
        {
            case 0:
                crimeSubejct = "방화\n";
                crimeDetail = gameManager.FireCount + "회\n";
                break;
            case 1:
                crimeSubejct += "절도\n";
                crimeDetail += gameManager.DressUpCount + "회\n";
                break;
            case 2:
                crimeSubejct += "특수폭행\n";
                crimeDetail += gameManager.DestroyCount + "회\n";
                break;
            case 3:
                crimeSubejct += "불법건축\n";
                crimeDetail += gameManager.ConstructionCount + "회\n";
                break;
        }
    }

    private IEnumerator theftSequence()
    {
        yield return new WaitForSeconds(0.5f);

        headText.text = "절도죄로 체포되었습니다";

        yield return new WaitForSeconds(1.0f);

        buttons[0].SetActive(true);
        buttons[1].SetActive(true);
    }

    private IEnumerator meatSequence()
    {
        yield return new WaitForSeconds(0.5f);

        headText.text = "오늘 저녁은 돼지고기다!";

        yield return new WaitForSeconds(1.0f);

        buttons[0].SetActive(true);
        buttons[1].SetActive(true);
    }

    public void restartButton()
    {
        SceneManager.LoadScene("SampleScene");
        //StartCoroutine(LoadYourAsyncScene());
    }

    public void endButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator LoadYourAsyncScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);
    }
}
