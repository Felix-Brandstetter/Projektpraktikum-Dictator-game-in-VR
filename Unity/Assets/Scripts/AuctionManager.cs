using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;



public class AuctionManager : MonoBehaviour
{
    [SerializeField]
    private string serverUri = "http://localhost:5000";

    public string ServerUri { get { return serverUri; } }

    [SerializeField]
    private GameObject[] pages; // Array with all the pages // needed

    private int currentPageId; // needed

    // Variables for dictator game
    public string id;
    public string sessionId = null;
    private int round = 1;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        currentPageId = 0;
        ActivatePage(currentPageId);

        //Register();

        StartCoroutine(SendHealth());
    }


    public IEnumerator SendHealth()
    {
        while(true)
        {
            if (id != null && id.Length > 0)
            {
                var uwr = UnityWebRequest.Get(serverUri + "/health/" + id);

                yield return uwr.SendWebRequest();

                switch (uwr.result)
                {
                    case UnityWebRequest.Result.Success:

                        // Check if new session was started
                        string currentSessionId = uwr.downloadHandler.text;

                        Debug.Log(currentSessionId);
                        Debug.Log(sessionId);

                        if (sessionId != null && sessionId.Length == 5 && currentSessionId.Length == 5 && !currentSessionId.Equals(sessionId))
                        {
                            Debug.Log("Restart");
                            round = 1;
                            ActivatePage(0);
                            id = null;
                        }
                        sessionId = currentSessionId;

                        break;

                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + uwr.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + uwr.error);
                        break;
                }

            }
            yield return new WaitForSeconds(1f);
        }
        
    }

    // Decide, which page is activated
    public void ActivatePage(int pageId)
    {
        pages[currentPageId].SetActive(false);
        pages[pageId].SetActive(true);
        currentPageId = pageId;

        // Send next page to server
        if (id != null)
        {
            StartCoroutine(SendCurrentPage(pages[pageId].name));
        }
    }

    private IEnumerator SendCurrentPage(string name)
    {
        var uwr = UnityWebRequest.Get(serverUri + "/health/" + id + "/page/" + name);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();
    }

    public void NextPage()
    {
        if (currentPageId + 1 < pages.Length) // is there another page?
        {
            ActivatePage(currentPageId + 1); // go to next page
        }
    }

    public void PreviousPage()
    {
        if (currentPageId - 1 > 0) // is there a previous page?
        {
            ActivatePage(currentPageId - 1); // go to previous page
        }
    }

    public void NextRound()
    {
        round = round +1;
    }


    // DICTATOR GAME Methods

    public void Register()
    {
        StartCoroutine(RegisterCoroutine());
    }

    private IEnumerator RegisterCoroutine()
    {
        var uwr = UnityWebRequest.Get(serverUri + "/register/" + SystemInfo.deviceName);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                //id = Int32.Parse(uwr.downloadHandler.text);
                id = uwr.downloadHandler.text;
                Debug.Log(id);
                break;
            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                Register();
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                Register();
                break;
        }        
    }

    public void AttentionCheck(string attention)
    {
        StartCoroutine(AttentionCheckCoroutine(attention));
    }
    private IEnumerator AttentionCheckCoroutine(string attention)
    {
        var uwr = UnityWebRequest.Get(serverUri + "/attention-check/" + id + "/"+ attention);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(uwr.downloadHandler.text);
                break;
            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                AttentionCheck(attention);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                AttentionCheck(attention);
                break;
        }        
    }

    public void SendDictatorDecision(string round, string amount)
    {
        StartCoroutine(SendDictatorDecisionCoroutine(round, amount));
    }

    private IEnumerator SendDictatorDecisionCoroutine(string round, string amount)
    {
        var uwr = UnityWebRequest.Get(serverUri + "/send-dictator-decision/" + id + "/"+ round +"/"+ amount);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log(uwr.downloadHandler.text);
                break;
            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                SendDictatorDecision(round, amount);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                SendDictatorDecision(round, amount);
                break;
        }        
    }

    public void CheckRoundEnd()
    {
        StartCoroutine( CheckRoundEndCoroutine());
    }

    private IEnumerator CheckRoundEndCoroutine()
    {
        var uwr = UnityWebRequest.Get(serverUri + "/check-round-end/" + round.ToString());
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                string text = uwr.downloadHandler.text;
                if (text == "True" )
                {
                    Debug.Log("Round Ended");
                    break;
                }
                else 
                {
                    Debug.Log("Round Not Ended");
                    yield return new WaitForSeconds(2);
                    CheckRoundEnd();
                    break;
                }

            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                CheckRoundEnd();
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                CheckRoundEnd();
                break;
        }        
    }
    
}