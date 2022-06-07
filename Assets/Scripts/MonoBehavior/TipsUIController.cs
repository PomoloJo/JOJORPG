using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// 挂载到 TipsUI 
public class TipsUIController : MonoBehaviour
{
    private static TipsUIController _instance;
    public static TipsUIController Instance
    {
        get{return _instance;}
    }


    private RectTransform childRectTransform;

    private void Awake() 
    {
        _instance = this;
        // 当前脚本挂载 TipsUI 下，获取当前第一个子物体的矩形 transform
        childRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
    }


    // Start is called before the first frame update
    void Start()
    {
        HideTips();
    }

    // Update is called once per frame
    void Update()
    {
        // 避免浮窗超出屏幕
        SetTipsPivot();
        // 跟随鼠标位置
        SetTipsPosition();
    }

    private void SetTipsPivot()
    {
        //设置浮窗动态中心
        int tempPivotX = (Input.mousePosition.x < Screen.width/2.0f) ? 0 : 1;
        int tempPivotY = (Input.mousePosition.y < Screen.height/2.0f) ? 0 : 1;
        if (childRectTransform.pivot.x != tempPivotX || childRectTransform.pivot.y != tempPivotY)
        {
            childRectTransform.pivot = new Vector2(tempPivotX, tempPivotY);
        }
    }

    private void SetTipsPosition()
    {
        childRectTransform.position = Input.mousePosition;
    }

    public bool GetTipsActive()
    {
        return childRectTransform.gameObject.activeSelf;
    }

    // 隐藏 TipsUI
    public void HideTips()
    {
        childRectTransform.gameObject.SetActive(false);
    }
    
    
    public void ShowTips(Item item)
    {
        // 0:name, 1:quality, 2:null , 4:description
        childRectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
        childRectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.itemName;
        childRectTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.quality.ToString();
        childRectTransform.GetChild(1).GetComponent<TextMeshProUGUI>().color = item.qualityColor;
        childRectTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = null;
        childRectTransform.GetChild(4).GetComponent<TextMeshProUGUI>().text = item.description;
        childRectTransform.gameObject.SetActive(true);
    }

    public void ShowTips(Weapon weapon)
    {
        // 0:Quality + Name, 1:null, 2:Basic Attribute, 4:description
        childRectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = weapon.quality.ToString() + " " + weapon.itemName;
        childRectTransform.GetChild(0).GetComponent<TextMeshProUGUI>().color = weapon.qualityColor;

        childRectTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = null;
        string basicAttribute = null;
        if(weapon.basicAttack != 0)
        {
            basicAttribute += "物理伤害: " + weapon.basicAttack + "\n";
        }
        if(weapon.basicElementPower != 0)
        {
            basicAttribute += "元素伤害: " + weapon.basicElementPower + "\n";
        }
        foreach(var entries in weapon.EntriesRangeList)
        {
            if(entries.isActive && entries.outputValue != 0)
            {
                basicAttribute += entries.outputDescription + "\n";
            }
        }
        childRectTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = basicAttribute;
        
        childRectTransform.GetChild(4).GetComponent<TextMeshProUGUI>().text = weapon.description;
        childRectTransform.gameObject.SetActive(true);
    }

}
