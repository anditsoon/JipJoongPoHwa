using System.Collections;
using UnityEngine;

public class AllyCamera : MonoBehaviour
{
    private GameObject allyCamera;
    private Camera myCamera;

    // rect
    float currTime;
    float duration = 1f;

    void Start()
    {
        allyCamera = GameObject.Find("AllyCamera");
        myCamera = allyCamera.GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            allyCamera.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.F2))
        {
            allyCamera.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.F11))
        {
            SubCamera();
        }
        if(Input.GetKeyDown(KeyCode.F12))
        {
            FullCamera();
        }
    }

    void FullCamera()
    {
        StartCoroutine(ChangeCameraSize(0.02f, 0f, 0.6f, 0f, 0.36f, 1f, 0.37f, 1f));
    }

    void SubCamera()
    {
        StartCoroutine(ChangeCameraSize(0f, 0.02f, 0f, 0.6f, 1f, 0.36f, 1f, 0.37f));
    }

    static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);

    }

    private IEnumerator ChangeCameraSize(float a, float b, float c, float d, float e, float f, float g, float h)
    {
        while(currTime < duration)
        {
            currTime += Time.deltaTime;

            float p = currTime / duration;
            p = EaseOutBack(p);
            float x_location = Mathf.Lerp(a, b, p);
            float y_location = Mathf.Lerp(c, d, p);
            float width = Mathf.Lerp(e, f, p);
            float height = Mathf.Lerp(g, h, p);

            myCamera.rect = new Rect(x_location, y_location, width, height);
            
            yield return null;

        }
        currTime = 0;
        myCamera.rect = new Rect(b, d, f, h);
    }
}


