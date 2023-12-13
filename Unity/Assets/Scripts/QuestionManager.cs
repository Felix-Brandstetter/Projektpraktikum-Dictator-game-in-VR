using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;

public class QuestionManager : MonoBehaviour
{
    public GameObject bar1;
    public GameObject bar2;
    public TextMeshPro scoreText1;
    public TextMeshPro scoreText2;

    public bool voted = false;
    public int[] scores = { 0, 0 };
    public string GetRequestURI = "http://ukmb3zubvl8qcj4p.myfritz.net:5000/items/";
    //private string GetRequestURI = "http://localhost:5000/items/";

    private Vector3 baseBarSize = new Vector3(0.5f, 9, 1);
    private Vector3 basePositionBar1 = new Vector3(2.33f, 0.1f, 0);
    private Vector3 basePositionBar2 = new Vector3(1.53f, 0.1f, 0);
    private Vector3 barScaleOffset = new Vector3(0, 9, 0);

    void Start()
    {
        //Get Scores from Server
        StartCoroutine(GetRequest(GetRequestURI + transform.name + "_" + 1 + "/", 1));
        StartCoroutine(GetRequest(GetRequestURI + transform.name + "_" + 2 + "/", 2));
        //Set Bars with initial values
        setBars();
        setScoreText();
    }

    //manage if question was answered already
    public void setVoted(bool status)
    {
        voted = status;
    }

    //update scores after voting
    public void updateScore()
    {
        StartCoroutine(GetRequest(GetRequestURI + transform.name + "_" + 1 + "/", 1));
        StartCoroutine(GetRequest(GetRequestURI + transform.name + "_" + 2 + "/", 2));
    }

    //set Bar sizes according to votes
    public void setBars()
    {
        if (scores[0] == scores[1])
        {
            bar1.transform.localScale = baseBarSize;
            bar1.transform.localPosition = basePositionBar1;
            bar2.transform.localScale = baseBarSize;
            bar2.transform.localPosition = basePositionBar2;
        }
        else
        {
            if (scores[0] > scores[1])
            {
                //bar 1 is full size
                bar1.transform.localScale = baseBarSize;
                //bar 1 at basePosition
                bar1.transform.localPosition = basePositionBar1;
                //new Scale for bar 2
                bar2.transform.localScale = (float)scores[1] / (float)scores[0] * barScaleOffset + new Vector3(0.5f, 0, 1);
                //move bar 2 according to new size
                bar2.transform.localPosition = new Vector3(basePositionBar2[0], basePositionBar2[1], 0) + ((float)scores[1] / (float)scores[0] / 2 * 9 - 4.5f) * new Vector3(0, 0, 1);
            }
            else
            {
                //bar 2 is full size
                bar2.transform.localScale = baseBarSize;
                //bar 2 at basePosition
                bar2.transform.localPosition = basePositionBar2;
                //new Scale for bar 1
                bar1.transform.localScale = (float)scores[0] / (float)scores[1] * barScaleOffset + new Vector3(0.5f, 0, 1);
                //move bar 1 according to new size
                bar1.transform.localPosition = new Vector3(basePositionBar1[0], basePositionBar1[1], 0) + ((float)scores[0] / (float)scores[1] / 2 * 9 - 4.5f) * new Vector3(0, 0, 1);
            }
        }
    }

    //set text on score textfields
    public void setScoreText()
    {
        scoreText1.text = "" + scores[0];
        scoreText2.text = "" + scores[1];
    }

    IEnumerator GetRequest(string uri, int optionNr)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        try
        {
            //set new score according to answer
            scores[optionNr - 1] = Int32.Parse(uwr.downloadHandler.text);
            Debug.Log("GetRequest Score" + optionNr + " is: " + uwr.downloadHandler.text);
        }
        catch (Exception)
        {
            if (scores[optionNr - 1] == 0)
            {
                scores[optionNr - 1] = UnityEngine.Random.Range(4, 21);
            }
        }
        //set bars and text after request has gone through 
        setBars();
        setScoreText();
    }
}
