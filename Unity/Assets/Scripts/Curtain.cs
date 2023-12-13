using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curtain : MonoBehaviour
{

    private float Alpha
    {
        get
        {
            return gameObject.GetComponent<Renderer>().material.color.a;
        }

        set
        {
            Color color = gameObject.GetComponent<Renderer>().material.color;
            gameObject.GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b, value);
        }
    }

    void Start()
    {
        //InvertFacesToInside();
        Alpha = 1f;
    }

    private void InvertFacesToInside()
    {
        MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }

    public void FadeTransparent(float duration)
    {
        StartCoroutine(FadeTransparentCoroutine(duration));
    }

    public void FadeBlack(float duration)
    {
        StartCoroutine(FadeBlackCoroutine(duration));
    }

    private IEnumerator FadeTransparentCoroutine(float duration)
    {
        Alpha = 1f;

        while (Alpha > 0f)
        {
            Alpha -= Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        Alpha = 0f;
    }

    private IEnumerator FadeBlackCoroutine(float duration)
    {
        Alpha = 0f;

        while (Alpha < 1f)
        {
            Alpha += Time.deltaTime / duration;
            yield return new WaitForEndOfFrame();
        }

        Alpha = 1f;
    }


}
