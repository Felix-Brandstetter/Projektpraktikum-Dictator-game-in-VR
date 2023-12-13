using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Text;
using TMPro;
using Newtonsoft.Json;



public class ResultDisplay : MonoBehaviour
{
    [SerializeField]
    private string serverUri;
    [SerializeField]
    private TMPro.TextMeshProUGUI round1kept;
    [SerializeField]
    private TMPro.TextMeshProUGUI round1received;
    [SerializeField]
    private TMPro.TextMeshProUGUI round2kept;
    [SerializeField]
    private TMPro.TextMeshProUGUI round2received;
    [SerializeField]
    private TMPro.TextMeshProUGUI round3kept;
    [SerializeField]
    private TMPro.TextMeshProUGUI round3received;


    [SerializeField] Image Round1_image_left;
    [SerializeField] Image Round1_image_right;
    [SerializeField] Image Round2_image_left;
    [SerializeField] Image Round2_image_right;
    [SerializeField] Image Round3_image_left;
    [SerializeField] Image Round3_image_right;

    [SerializeField]
    private TMPro.TextMeshProUGUI Text_Erinnerung;

    private float endamount =0;
    
    
    public class Result
    {
        public string id { get; set; }
        public string round1Sent { get; set; }
        public string round1Received { get; set; }
        public string round2Sent { get; set; }
        public string round2Received { get; set; }
        public string round3Sent { get; set; }
        public string round3Received { get; set; }
        public bool round1MyDecision { get; set; }
        public bool round2MyDecision { get; set; }
        public bool round3MyDecision { get; set; }
    }


    // Start is called before the first frame update
    void Start()
    {
        serverUri = GameObject.Find("Dictator Game").GetComponent<AuctionManager>().ServerUri;

        GetResults();
    }
        
   public void GetResults()
   {
        string id = GameObject.Find("Dictator Game").GetComponent<AuctionManager>().id;
        StartCoroutine(GetResultsCoroutine(id));
    }

    private IEnumerator GetResultsCoroutine(string id)
    {
        var uwr = UnityWebRequest.Get(serverUri + "/results/" + id);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        switch (uwr.result)
        {
            case UnityWebRequest.Result.Success:
                string text = uwr.downloadHandler.text;
                var result = JsonConvert.DeserializeObject<Result>(text);
                Debug.Log("sucess");

                round1kept.text = (10  -System.Convert.ToInt32(result.round1Sent)).ToString();
                round1received.text = result.round1Received;
                if(result.round1MyDecision == true){
                    endamount = endamount + (10  -System.Convert.ToInt32(result.round1Sent));
                    Round1_image_left.gameObject.SetActive(true);
                    Round1_image_right.gameObject.SetActive(false);
                }
                if(result.round1MyDecision == false){
                    endamount = endamount + (System.Convert.ToInt32(result.round1Received));
                    Round1_image_left.gameObject.SetActive(false);
                    Round1_image_right.gameObject.SetActive(true);
                }

                round2kept.text = (10  -System.Convert.ToInt32(result.round2Sent)).ToString();
                round2received.text = result.round2Received;
                if(result.round2MyDecision == true){
                    endamount = endamount + (10  -System.Convert.ToInt32(result.round2Sent));
                    Round2_image_left.gameObject.SetActive(true);
                    Round2_image_right.gameObject.SetActive(false);
                }
                if(result.round2MyDecision == false){
                    endamount = endamount + (System.Convert.ToInt32(result.round2Received));
                    Round2_image_left.gameObject.SetActive(false);
                    Round2_image_right.gameObject.SetActive(true);
                }

                round3kept.text = (10  -System.Convert.ToInt32(result.round3Sent)).ToString();
                round3received.text = result.round3Received;
                if(result.round3MyDecision == true){
                    endamount = endamount + (10  -System.Convert.ToInt32(result.round3Sent));
                    Round3_image_left.gameObject.SetActive(true);
                    Round3_image_right.gameObject.SetActive(false);
                }
                if(result.round3MyDecision == false){
                    endamount = endamount + (System.Convert.ToInt32(result.round3Received));
                    Round3_image_left.gameObject.SetActive(false);
                    Round3_image_right.gameObject.SetActive(true);
                }
                Text_Erinnerung.text = "<b>Erinnerung:</b> Eine GE entspricht 0,10€. Somit erhalten Sie neben Ihrer fixen Auszahlung von 6,50€ zusätzlich noch " + (endamount/10).ToString() +"€.";

       
                break;
            
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                GetResults();
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("HTTP Error: " + uwr.error);
                yield return new WaitForSeconds(1f);
                GetResults();
                break;
        }        
    }
    
}
