using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class VoteButton : MonoBehaviour
{
    public GameObject question;

    private string PostRequestURI;
    //private string PostRequestURI = "http://localhost:5000/items/";

    private bool voted;

    public int optionNr;

    void Start()
    {
        PostRequestURI = question.GetComponent<QuestionManager>().GetRequestURI;
        voted = question.GetComponent<QuestionManager>().voted;
    }

    protected void FixedUpdate()
    {
        voted = question.GetComponent<QuestionManager>().voted;
        if (voted)
        {
            gameObject.GetComponent<InputHandler>().enabled = false;
        }
    }

    public void Click()
    {
        StartCoroutine(PostRequest(PostRequestURI + question.name + "_" + optionNr + "/increase/"));
        voted = true;
        question.GetComponent<QuestionManager>().setVoted(voted);

    }

    IEnumerator PostRequest(string url)
    {
        var uwr = new UnityWebRequest(url, "POST");
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            question.GetComponent<QuestionManager>().scores[optionNr - 1]++;
        }
        else
        {
            Debug.Log("Received from PostRequest: " + uwr.downloadHandler.text);
        }
        question.GetComponent<QuestionManager>().updateScore();
    }
}
