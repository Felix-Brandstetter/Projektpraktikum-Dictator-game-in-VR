using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartphoneOrientation : MonoBehaviour
{
    [SerializeField]
    private ScreenOrientation screenOrientation;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = screenOrientation;
    }
}
