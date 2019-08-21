﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        StartCoroutine(sequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator sequence()
    {
        for(int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.5f);

            addCrimeText(i);
            crimeSubjectText.text = crimeSubejct;
            crimeDetailText.text = crimeDetail;
        }

        yield return new WaitForSeconds(0.5f);

        headText.text = "징역 ???년";

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
}
