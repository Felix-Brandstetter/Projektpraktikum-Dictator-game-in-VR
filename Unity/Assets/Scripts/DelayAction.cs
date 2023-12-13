using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayAction : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 15f)]
    private float delay;

    public void DisableObject(GameObject go)
    {
        StartCoroutine(DisableObjectAsync(go));
    }

    private IEnumerator DisableObjectAsync(GameObject go)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
    }

    public void EnableObject(GameObject go)
    {
        StartCoroutine(EnableObjectAsync(go));
    }

    private IEnumerator EnableObjectAsync(GameObject go)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(true);
    }
}
