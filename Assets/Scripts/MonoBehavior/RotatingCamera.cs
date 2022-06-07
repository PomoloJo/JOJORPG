using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于相机跟随和视角旋转, 挂载到 CameraPosition 下
public class RotatingCamera : MonoBehaviour
{
    // 一次旋转所需时间
    public float rotateTime = 0.2f;
    // 是否启动相机延迟跟随的效果
    public bool needToLate = false;
    // 相机延迟跟随的延迟时间
    public float cameraFollowTime = 1.0f;
    private bool isRotating = false;
    private Transform playerTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void LateUpdate() 
    {
        CameraFollow();
    }

    private void CameraFollow()
    {
        // 首先要判断一下角色还有没有引用，比如角色死亡时就没有引用了
        if(playerTransform != null)
        {
            // 正常运动时不需要启动相机延迟跟随
            if(needToLate == false)
            {
                transform.position = playerTransform.position;
            }
            // 相机延迟效果可以用于奔跑等快速移动时
            else
            {
                transform.position = Vector3.Lerp(transform.position, playerTransform.position, cameraFollowTime * Time.fixedDeltaTime);
            }
        }
    }

    private void Rotate()
    {
        if(Input.GetKeyDown(KeyCode.Q) && !isRotating)
        {
            StartCoroutine(RotateAround(-45, rotateTime));
        }
        else if(Input.GetKeyDown(KeyCode.E) && !isRotating)
        {
            StartCoroutine(RotateAround(45, rotateTime));
        }
    }

    IEnumerator RotateAround(float angle, float time)
    {
        float number = 60 * time;
        float nextAngle = angle / number;
        isRotating = true;
        for(int i=0; i<number; ++i)
        {
            transform.Rotate(new Vector3(0,0,nextAngle));
            yield return new WaitForFixedUpdate();
        }
        isRotating = false;
    }
}
