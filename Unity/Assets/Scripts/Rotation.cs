using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationOffset = 0.5f;

    // Update is called once per frame
    void FixedUpdate () 
     {
         if (Input.GetKey("right"))
         {
            transform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y+rotationOffset,transform.localEulerAngles.z));
         }
         if (Input.GetKey("left"))
         {
            transform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x,transform.localEulerAngles.y-rotationOffset,transform.localEulerAngles.z));
         }
         if (Input.GetKey("down"))
         {
            transform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x+rotationOffset,transform.localEulerAngles.y,transform.localEulerAngles.z));
         }
         if (Input.GetKey("up"))
         {
            transform.rotation = Quaternion.Euler(new Vector3(transform.localEulerAngles.x-rotationOffset,transform.localEulerAngles.y,transform.localEulerAngles.z));
         }
     }
 
}
