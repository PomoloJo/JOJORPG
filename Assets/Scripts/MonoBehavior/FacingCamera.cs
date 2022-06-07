using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于使所用物体始终朝向摄像机所在平面,挂载到 camera controller 下
public class FacingCamera : MonoBehaviour
{
    private GameObject player;


    private GameObject treant;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        treant = GameObject.Find("Treant1");
    }

    void Update()
    {
        // Bug?? 考虑到以后可能多人游戏有多个 Player，现在这里可能只能旋转其中一个
        // 先更新人物朝向摄像机的角度
        if(player.transform.rotation != Camera.main.transform.rotation)
        {
            player.transform.rotation = Camera.main.transform.rotation;

            treant.transform.rotation = Camera.main.transform.rotation;
        }
        // 更新所有资源朝向摄像机的角度
        foreach(GameObject resource in ResourceSpawner.envResources)
        {
            // 按理说不应该由这个脚本来控制物体的删除，所以这里改成检查物体是否为空了
            //GameObject resource = (GameObject)resource;
            if(resource == null)
            {
                Debug.Log("检测到 gameobject 为空,不应该出现这种情况");
            }
            else if(resource.transform.rotation != Camera.main.transform.rotation)
            {
                resource.transform.rotation = Camera.main.transform.rotation;
            }
        }
        foreach(GameObject item in ResourceSpawner.itemList)
        {
            // 按理说不应该由这个脚本来控制物体的删除，所以这里改成检查物体是否为空了
            if(item == null)
            {
                Debug.Log("检测到 item 为空,不应该出现这种情况");
            }
            else if(item.transform.rotation != Camera.main.transform.rotation)
            {
                item.transform.rotation = Camera.main.transform.rotation;
            }
        }
    }

}
