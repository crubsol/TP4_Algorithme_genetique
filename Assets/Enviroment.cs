using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enviroment : MonoBehaviour
{
    Material enviromentMaterial;
    void Start()
    {
        enviromentMaterial = gameObject.GetComponent<Renderer>().material;
        StartCoroutine("CycleColors");

    }
    IEnumerator CycleColors()
    {
        Vector3 previousColor = new Vector3(enviromentMaterial.color.r, enviromentMaterial.color.g, enviromentMaterial.color.b);

        Vector3 currentColor = previousColor;

        float colorTransitionTime = 4.0f;

        while (true)
        {
            Vector3 newColor = new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));

            //Debug.Log("I got a new color!");

            Vector3 deltaColor = (newColor - previousColor) * (1.0f / colorTransitionTime);

            while ((newColor - currentColor).magnitude > 0.1f) 
            {
                currentColor = currentColor + deltaColor * Time.deltaTime;
                enviromentMaterial.color = new Color(currentColor.x, currentColor.y, currentColor.z);
                yield return null;
            }

            previousColor = newColor;
        }
    }
}
