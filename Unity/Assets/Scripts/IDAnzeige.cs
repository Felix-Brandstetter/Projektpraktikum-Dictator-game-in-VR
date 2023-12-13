using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDAnzeige : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI IDText;
    
    // Start is called before the first frame update
    void Start()
    {
        string id = GameObject.Find("Dictator Game").GetComponent<AuctionManager>().id;
        IDText.text = "Ihre ID lautet: " + id;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
