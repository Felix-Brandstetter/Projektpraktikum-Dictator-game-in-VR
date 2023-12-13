using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, 1, QueryTriggerInteraction.Collide) && hit.collider.isTrigger)
        {
            Debug.DrawLine(ray.origin, hit.point);
            InputHandler handler = hit.transform.gameObject.GetComponent<InputHandler>();
            if (handler != null)
            {
                handler.Trigger();
            }
        }
            
    }
}
