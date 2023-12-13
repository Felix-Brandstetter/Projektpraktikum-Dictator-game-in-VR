using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CheckRoundEnd : MonoBehaviour
{
    [SerializeField]
    private string round;

    [SerializeField]
    private string serverUri;

    // Start is called before the first frame update
    void Start()
    {
        serverUri = GameObject.Find("Dictator Game").GetComponent<AuctionManager>().ServerUri;

        //NextButton.gameObject.SetActive(false); Fehler weil Gameobjekt inactive
        //Check();
    }
    void OnEnable()
    {
        Check();
        Debug.Log("Check Roundend");

    }

    public void Check()
    {
        StartCoroutine(CheckCoroutine());
    }

    private IEnumerator CheckCoroutine()
    {
        var uwr = UnityWebRequest.Get(serverUri + "/check-round-end/" + round);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                string text = uwr.downloadHandler.text;
                if (text == "True" )
                {
                    GameObject.Find("Dictator Game").GetComponent<AuctionManager>().NextPage();
                    Debug.Log("Round Ended");
                    break;
                }
                else 
                {
                    Debug.Log("Round Not Ended");
                    yield return new WaitForSeconds(2);
                    Check();
                    break;
                }

            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                Check();
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                Check();
                break;
        }        
    }

}
