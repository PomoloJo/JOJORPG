using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



// 专门管理人物身上装备与状态 UI 的内容，挂载到了 Player 下
public class StatusUIController : MonoBehaviour
{
    private static StatusUIController _instance;
    public static StatusUIController Instance
    {
        get{return _instance;}
    }

    private void Awake() 
    {
        _instance = this;
    }

    public GameObject playerUI;

    // key 装备部位名称 value item 实例
    public Dictionary<string, Item> equipmentDictionary;


    // 显示在 状态 UI 上的 信息
    private TextMeshProUGUI health;
    private TextMeshProUGUI defence;
    private TextMeshProUGUI moveSpeed;
    private TextMeshProUGUI luck;

    private TextMeshProUGUI attack;
    private TextMeshProUGUI penetration;
    private TextMeshProUGUI criticalRate;
    private TextMeshProUGUI criticalMultiple;

    private TextMeshProUGUI elementPower;
    private TextMeshProUGUI elementPenetration;
    private TextMeshProUGUI elementCriticalRate;
    private TextMeshProUGUI elementCriticalMultiple;

    private TextMeshProUGUI fireDamage;
    private TextMeshProUGUI waterDamage;
    private TextMeshProUGUI thunderDamage;
    private TextMeshProUGUI toxicDamage;

    private TextMeshProUGUI fireElementResistance;
    private TextMeshProUGUI waterElementResistance;
    private TextMeshProUGUI thunderElementResistance;
    private TextMeshProUGUI toxicElementResistance;


    // Start is called before the first frame update
    void Start()
    {
        equipmentDictionary = new Dictionary<string, Item>();
        equipmentDictionary.Add("WeaponSlot", null);
        equipmentDictionary.Add("OffhandWeaponSlot", null);

        Transform playerStatusUI = playerUI.transform.Find("PlayerStatusUI");

        health = playerStatusUI.GetChild(1).GetComponent<TextMeshProUGUI>();
        defence = playerStatusUI.GetChild(3).GetComponent<TextMeshProUGUI>();
        moveSpeed = playerStatusUI.GetChild(5).GetComponent<TextMeshProUGUI>();
        luck = playerStatusUI.GetChild(7).GetComponent<TextMeshProUGUI>();

        attack = playerStatusUI.GetChild(9).GetComponent<TextMeshProUGUI>();
        penetration = playerStatusUI.GetChild(11).GetComponent<TextMeshProUGUI>();
        criticalRate = playerStatusUI.GetChild(13).GetComponent<TextMeshProUGUI>();
        criticalMultiple = playerStatusUI.GetChild(15).GetComponent<TextMeshProUGUI>();

        elementPower = playerStatusUI.GetChild(17).GetComponent<TextMeshProUGUI>();
        elementPenetration = playerStatusUI.GetChild(19).GetComponent<TextMeshProUGUI>();
        elementCriticalRate = playerStatusUI.GetChild(21).GetComponent<TextMeshProUGUI>();
        elementCriticalMultiple = playerStatusUI.GetChild(23).GetComponent<TextMeshProUGUI>();

        fireDamage = playerStatusUI.GetChild(25).GetComponent<TextMeshProUGUI>();
        waterDamage = playerStatusUI.GetChild(27).GetComponent<TextMeshProUGUI>();
        thunderDamage = playerStatusUI.GetChild(29).GetComponent<TextMeshProUGUI>();
        toxicDamage = playerStatusUI.GetChild(31).GetComponent<TextMeshProUGUI>();

        fireElementResistance = playerStatusUI.GetChild(33).GetComponent<TextMeshProUGUI>();
        waterElementResistance = playerStatusUI.GetChild(35).GetComponent<TextMeshProUGUI>();
        thunderElementResistance = playerStatusUI.GetChild(37).GetComponent<TextMeshProUGUI>();
        toxicElementResistance = playerStatusUI.GetChild(39).GetComponent<TextMeshProUGUI>();
    }


    public void RefreshStatusUI()
    {
        Debug.Log("----------------Refresh Status UI-----------------");
        var tempStatusDictionary = Player.Instance.tempStatusDictionary;

        health.text = tempStatusDictionary["tempHealthMaxPoint"].ToString();
        defence.text = tempStatusDictionary["tempDefence"].ToString();
        moveSpeed.text = tempStatusDictionary["tempMoveSpeed"].ToString("f1");
        luck.text = tempStatusDictionary["tempLuck"].ToString();

        attack.text = tempStatusDictionary["tempAttack"].ToString();
        penetration.text = tempStatusDictionary["tempPenetration"].ToString();
        criticalRate.text = tempStatusDictionary["tempCriticalRate"].ToString() + "%";
        criticalMultiple.text = "x" + (tempStatusDictionary["tempCriticalMultiple"]/100.0f).ToString("f1");

        elementPower.text = tempStatusDictionary["tempElementPower"].ToString();
        elementPenetration.text = tempStatusDictionary["tempElementPenetration"].ToString();
        elementCriticalRate.text = tempStatusDictionary["tempElementCriticalRate"].ToString() + "%";
        elementCriticalMultiple.text = "x" + (tempStatusDictionary["tempElementCriticalMultiple"]/100.0f).ToString("f1");

        fireDamage.text = tempStatusDictionary["tempFireDamage"].ToString();
        waterDamage.text = tempStatusDictionary["tempWaterDamage"].ToString();
        thunderDamage.text = tempStatusDictionary["tempThunderDamage"].ToString();
        toxicDamage.text = tempStatusDictionary["tempToxicDamage"].ToString();

        fireElementResistance.text = tempStatusDictionary["tempFireElementResistance"].ToString();
        waterElementResistance.text = tempStatusDictionary["tempWaterElementResistance"].ToString();
        thunderElementResistance.text = tempStatusDictionary["tempThunderElementResistance"].ToString();
        toxicElementResistance.text = tempStatusDictionary["tempToxicElementResistance"].ToString();
    }
}
