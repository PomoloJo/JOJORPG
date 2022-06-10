using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用于使所用物体始终朝向摄像机所在平面,挂载到 camera controller 下
public class FacingCamera : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        // Bug?? 考虑到以后可能多人游戏有多个 Player，现在这里可能只能旋转其中一个
        // 先更新人物朝向摄像机的角度
        if(player.transform.rotation != Camera.main.transform.rotation)
        {
            player.transform.rotation = Camera.main.transform.rotation;
        }
        // 更新所有环境静态资源(比如树)朝向摄像机的角度
        UpdateRotation(ResourceSpawner.envResources);
        // 更新所有可拾取物品朝向摄像机的角度
        UpdateRotation(ResourceSpawner.itemList);
        // 更新所有非玩家角色朝向摄像机的角度
        UpdateRotation(CharacterSpawner.Instance.characterList);
    }

    // 更新传入的 list 内所有物体朝向摄像机的旋转角度
    public void UpdateRotation(List<GameObject> rotationList)
    {
        foreach(GameObject item in rotationList)
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
