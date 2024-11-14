using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResultPopCount : MonoBehaviour
{

    public TextMeshProUGUI CornCount;
    private int score;

    public GameObject Good, Soso, Bad;
    public GameObject star, star2, star3;
    // Start is called before the first frame update
    void Start()
    {
        score = Gauge3.getCornCount();
        CornCount.text = score.ToString();
        Good.SetActive(false);
        Soso.SetActive(false);
        Bad.SetActive(false);
        star.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);
        Result();
    }

    // Update is called once per frame
    private void Result()
    {
        if (score <4 && score >= 0)
        {
            Good.SetActive(true);
            star.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        else if (score < 7 && score >= 4)
        {
            Soso.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
        }
        else if (score < 10 && score >= 7)
        {
            Bad.SetActive(true);
            star3.SetActive(true);
        }
        else
        {
            
        }
    }
}
