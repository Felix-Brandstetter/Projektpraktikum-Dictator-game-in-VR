using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DecisionHandler : MonoBehaviour
{
    [SerializeField]
    private string round;

    [SerializeField]
    private string amount;

    [SerializeField]
    private TMPro.TextMeshProUGUI amountsent_display;

    public void SendAmount() {
        GameObject.Find("Dictator Game").GetComponent<AuctionManager>().SendDictatorDecision(round, amount);
        amountsent_display.text = "Sie haben sich als Sender:in in dieser Runde entschieden " + amount + " GE an Ihre:n Mitspieler:in zu senden.";
    }

}
