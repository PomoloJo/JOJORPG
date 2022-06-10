using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ZoomHandler : MonoBehaviour
{
    public float scrollSpeed = 10.0f;
    public float viewRecoverTime = 0.2f;


    private bool isRecovering = false; 


    // zoomDelta 用于记录与原位置的偏离程度
    private float zoomDelta;
    private float scrollWheelSize;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;


    // Start is called before the first frame update
    void Start()
    {
        zoomDelta = 0.0f;
        originalCameraPosition = Camera.main.transform.localPosition;
        originalCameraRotation = Camera.main.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        RecoverDetect();
    }

    private void LateUpdate() 
    {
        ViewZoom();
    }

    private void ViewZoom()
    {
        if(!isRecovering)
        {
            scrollWheelSize = Input.GetAxis("Mouse ScrollWheel");
            if(Mathf.Approximately(scrollWheelSize, 0))
            {
                return;
            }

            // zoomDelta >= 0 鸟瞰角度
            if(zoomDelta >= 0)
            {
                if(scrollWheelSize > 0 && zoomDelta < 20)
                {
                    zoomDelta += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                }
                else if(scrollWheelSize < 0)
                {
                    zoomDelta += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                }
                //print("鸟瞰角度: " + zoomDelta);
                Camera.main.transform.localRotation = Quaternion.Euler(originalCameraRotation.eulerAngles + new Vector3(zoomDelta,0,0));
                Camera.main.transform.localPosition = originalCameraPosition + new Vector3(0, zoomDelta * 0.2f, zoomDelta * -0.1f);
            }

            // zoomDelta < 0 平视角度
            else
            {
                if(scrollWheelSize < 0 && zoomDelta > -40)
                {
                    zoomDelta += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                }
                else if(scrollWheelSize > 0)
                {
                    zoomDelta += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                }
                //print("平视角度: " + zoomDelta);
                Camera.main.transform.localRotation = Quaternion.Euler(originalCameraRotation.eulerAngles + new Vector3(zoomDelta,0,0));
                Camera.main.transform.localPosition = originalCameraPosition + new Vector3(0, 0, zoomDelta * zoomDelta * -0.002f + zoomDelta * -0.25f);
            }
            
            //print(Camera.main.transform.eulerAngles.x);  
        }
    }


    private void RecoverDetect()
    {
        if(Input.GetKeyDown(KeyCode.Mouse2) && !isRecovering)
        {
            StartCoroutine(RecoverOriginalView(viewRecoverTime));
            zoomDelta = 0.0f;
        }
    }

    IEnumerator RecoverOriginalView(float time)
    {
        isRecovering = true;
        float number = 60 * time;
        float nextAngle = zoomDelta / number;
        float nextPositionStep = zoomDelta / number;
        float nextPositionStep2 = zoomDelta * zoomDelta / number;
        for(int i=0; i<number; ++i)
        {
            if(nextAngle >= 0)
            {
                Camera.main.transform.localPosition -= new Vector3(0, nextPositionStep * 0.2f, nextPositionStep * -0.1f);
            }
            else
            {
                Camera.main.transform.localPosition -= new Vector3(0, 0, nextPositionStep2 * -0.002f + nextPositionStep * -0.25f);
            }
            Camera.main.transform.Rotate(new Vector3(-nextAngle,0,0));
            yield return new WaitForFixedUpdate();
        }
        isRecovering = false;
    }

}
