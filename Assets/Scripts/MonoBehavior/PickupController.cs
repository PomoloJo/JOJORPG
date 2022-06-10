using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// 用于人物的拾取控制，挂载到 Player 下
public class PickupController : MonoBehaviour
{
    private float moveSpeed;

    // 以后要改成 player 或 character 的属性
    public float pickupDistance = 7.0f;

    public bool isWalking = false;

    public GameObject pickupTarget;

    
    private Rigidbody2D playerRigibody;
    private Animator playerAnimator;

    private Inventory bag;
    public void SetBag(Inventory inventory)
    {
        bag = inventory;
    }


    // Start is called before the first frame update 
    void Start()
    {
        playerRigibody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        moveSpeed = GetComponent<Player>().moveSpeed;
        pickupTarget = null;
    }        

    // Update is called once per frame
    void Update()
    {   
        DetectKeyDown();
    }

    private void FixedUpdate() 
    {
        if(isWalking && pickupTarget != null)
        {
            WalkAndPickup(pickupTarget);
        }
    }


    // 检测输入，按下空格拾取, 按下 WASD 打断这一过程
    private void DetectKeyDown()
    {
        // 如果没有执行 WalkAndPickup 函数时按下空格，开始执行，如果正在执行时按下空格，不响应
        if( !isWalking && Input.GetKeyDown(KeyCode.Space))
        {
            isWalking = true;
            GetNearestGameObject(out pickupTarget);
        }
        // 如果正在执行时按下 WASD, 打断过程
        else if( isWalking && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)))
        {
            isWalking = false;
            pickupTarget = null;
        }
    }
    
    // 返回最近的可拾取物体
    private bool GetNearestGameObject(out GameObject targetObject)
    {
        float finalDistance = pickupDistance;
        targetObject = null;

        foreach(GameObject eachObject in ResourceSpawner.itemList) 
        {
            float distance = Vector3.Distance(transform.position, eachObject.transform.position);
            if(distance < pickupDistance && eachObject.CompareTag("Item"))
            {
                if(distance < finalDistance)
                {
                    finalDistance = distance;
                    targetObject = eachObject;
                }
            }
        }
        return finalDistance < pickupDistance ? true : false;
    }


    // 直线走过去并拾取最近物体
    public void WalkAndPickup(GameObject pickupTarget)
    {
        Vector3 positionVector = pickupTarget.transform.position - transform.position;
        // 模拟输入
        playerRigibody.velocity = positionVector.normalized * moveSpeed;
        // 这个停止的判断值可以再稍微设置大一点，避免因到达不了精确坐标出现的鬼畜现象
        if(Mathf.Abs(positionVector.x) < 0.3f && Mathf.Abs(positionVector.y) < 0.3f)
        {
            // 把 gameobject 里面的 item 放到背包中
            AddItemIntoBag(pickupTarget.GetComponent<Item>());

            // 从资源列表中移除该资源，并从内存中清除，为保险起见，再次将变量设置为 null
            ResourceSpawner.itemList.Remove(pickupTarget);
            Destroy(pickupTarget.gameObject);
            pickupTarget = null;
            isWalking = false;
            playerAnimator.SetBool("IsMoving", false);
        }
        else
        {
            playerAnimator.SetBool("IsMoving", true);
            Vector3 newVec = Quaternion.Euler(0,0,-transform.rotation.eulerAngles[2]) * playerRigibody.velocity;
            playerAnimator.SetFloat("InputX", newVec.x);
            playerAnimator.SetFloat("InputY", newVec.y);
        }
    }

    private void AddItemIntoBag(Item item)
    {
        bool needToAdd = false;
        if(item.dataType == Item.DataType.Shared)
        {
            foreach(Item bagItem in bag.ItemList)
            {
                if(bagItem.itemType == item.itemType)
                {
                    bagItem.quantity++;
                    // 增加背包 UI 中对应物品的数量，并更新背包 UI
                    var bagUI = GetComponent<BagUIController>();
                    bagUI.itemDictionary[bagItem][0] = Tuple.Create(bagUI.itemDictionary[bagItem][0].Item1, bagUI.itemDictionary[bagItem][0].Item2 + 1);
                    bagUI.RefreshBagItems();

                    // 这里返回！
                    return;
                }
            }
            needToAdd = true;
        }
        if(item.dataType == Item.DataType.Unique || needToAdd)
        {
            // 在偏远角落创建 item 的实例, 不要让它出现在画面中，用到的时候直接改变其位置即可
            int edgePosition = RandomMap.mapData.GetLength(0);
            Item newitem = Instantiate(item, new Vector3(-edgePosition,-edgePosition,0), Quaternion.identity);
            bag.AddItem(newitem);
            //Debug.Log("增加: " + newitem);

            // 更新背包 UI
            GetComponent<BagUIController>().RefreshBagItems();
        }
    }


}
