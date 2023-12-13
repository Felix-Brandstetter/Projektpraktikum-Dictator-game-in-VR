using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMovement : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 2f)]
    private float sensitivity = 1f;

    [SerializeField]
    private bool editorOnly = true;

    private bool active = false;

    private float prevV = 0f;
    private float prevH = 0f;

    public bool Active { get => active; set => active = value; }

    private void Start()
    {
        // Activate editor movement if this is not the first scene
        if (Time.time > 1f)
        {
            Camera.main.GetComponent<EditorMovement>().Active = true;
        }

        if (active == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!editorOnly || Application.isEditor)
        {
            if (active)
            {
                // Get Mouse Position
                float v = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * 400;
                float h = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * -400;

                // Apply smoothing
                prevV = (v + prevV) / 2;
                prevH = (h + prevH) / 2;

                // Restrict vertical restrictions
                if ((transform.rotation.eulerAngles.x + prevH) > 80 && (transform.rotation.eulerAngles.x + prevH) < 280)
                    prevH = 0;

                // Apply rotation
                transform.Rotate(transform.right, prevH, Space.World);
                transform.Rotate(Vector3.up, prevV, Space.World);

                // Unlock Cursor
                if (Input.GetKey(KeyCode.Escape))
                {
                    Cursor.lockState = CursorLockMode.None;
                    active = false;
                }
            }
            else
            {
                // Lock Cursor
                if (Input.GetButton("Fire1"))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    active = true;
                }
            }
        }
    }

}
