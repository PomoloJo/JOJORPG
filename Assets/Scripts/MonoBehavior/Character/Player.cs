using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Character
{
    // 构造单例类？
    private static Player _instance;
    public static Player Instance
    {
        get{return _instance;}
    }


    private Inventory _bag;

    public Inventory Bag
    {
        get{return _bag;}
    }

    private void Awake() 
    {
        if(!_instance)
        {
            _instance = this;   
            _bag = new Inventory();
        }
    }

    public Dictionary<string, int> tempStatusDictionary;

    // Start is called before the first frame update
    void Start()
    {
        // 把背包传入给 BagUIController， PickupController
        GetComponent<BagUIController>().SetBag(_bag);
        GetComponent<PickupController>().SetBag(_bag);

        // 经过装备或技能加成以后的临时属性, 用于 UI 显示
        tempStatusDictionary = new Dictionary<string, int>
        {
            {"tempHealthMaxPoint", healthMaxPoint},
            {"tempDefence", defence},
            {"tempMoveSpeed", moveSpeed},
            {"tempLuck", luck},

            {"tempAttack", attack},
            {"tempPenetration", penetration},
            {"tempCriticalRate", criticalRate},
            {"tempCriticalMultiple", criticalMultiple},

            {"tempElementPower", elementPower},
            {"tempElementPenetration", elementPenetration},
            {"tempElementCriticalRate", elementCriticalRate},
            {"tempElementCriticalMultiple", elementCriticalMultiple},

            {"tempFireElementResistance", fireElementResistance},
            {"tempWaterElementResistance", waterElementResistance},
            {"tempThunderElementResistance", thunderElementResistance},
            {"tempToxicElementResistance", toxicElementResistance},

            // 不在面板显示的隐藏属性
            {"tempRealDamage", 0},
            
            {"tempFireDamage", fireDamage},
            {"tempWaterDamage", waterDamage},
            {"tempThunderDamage", thunderDamage},
            {"tempToxicDamage", toxicDamage},
        };
    }


    // 经过装备或技能加成以后的临时属性, 用于 UI 显示，不要在面板进行操作
    // public int tempHealthPoint;
    // public int tempHealthMaxPoint;

    // public int tempAttack;
    // public int tempDefence;
    // public int tempPenetration;
    // public int tempCriticalRate;
    // public int tempCriticalMultiple;

    
    // public int tempElementPower;
    // public int tempElementPenetration;
    // public int tempElementCriticalRate;
    // public int tempElementCriticalMultiple;

    // public int tempFireElementResistance;
    // public int tempWaterElementResistance;
    // public int tempThunderElementResistance;
    // public int tempToxicElementResistance;


    // public float tempMoveSpeed;
    // public int tempLuck;


    // // 不够位置显示的隐藏属性
    // public int tempRealDamage;

    // public int tempFireDamage;
    // public int tempWaterDamage;
    // public int tempThunderDamage;
    // public int tempToxicDamage;

    // 刷新人物临时状态
    public void RefreshStatus()
    {
        Debug.Log("----------------Refresh Status-----------------");
        var equipmentDictionary = StatusUIController.Instance.equipmentDictionary;

        int weaponAttack = 0;
        int weaponElementPower = 0;

        tempStatusDictionary["tempFireDamage"] = fireDamage;
        tempStatusDictionary["tempWaterDamage"] = waterDamage;
        tempStatusDictionary["tempThunderDamage"] = thunderDamage;
        tempStatusDictionary["tempToxicDamage"] = toxicDamage;

        if(equipmentDictionary["WeaponSlot"])
        {
            Weapon weapon = equipmentDictionary["WeaponSlot"].GetComponent<Weapon>();
            weaponAttack = weapon.basicAttack;
            weaponElementPower = weapon.basicElementPower;
            foreach(var entries in weapon.EntriesRangeList)
            {
                if(entries.isActive)
                {
                    tempStatusDictionary[entries.outputKey] += entries.outputValue;
                }
            }
        }

        tempStatusDictionary["tempHealthMaxPoint"] = healthMaxPoint;
        tempStatusDictionary["tempDefence"] = defence;
        tempStatusDictionary["tempMoveSpeed"] = moveSpeed;
        tempStatusDictionary["tempLuck"] = luck;

        tempStatusDictionary["tempAttack"] = attack + weaponAttack;
        tempStatusDictionary["tempPenetration"] = penetration;
        tempStatusDictionary["tempCriticalRate"] = criticalRate;
        tempStatusDictionary["tempCriticalMultiple"] = criticalMultiple;

        tempStatusDictionary["tempElementPower"] = elementPower + weaponElementPower;
        tempStatusDictionary["tempElementPenetration"] = elementPenetration;
        tempStatusDictionary["tempElementCriticalRate"] = elementCriticalRate;
        tempStatusDictionary["tempElementCriticalMultiple"] = elementCriticalMultiple;

        tempStatusDictionary["tempFireElementResistance"] = fireElementResistance;
        tempStatusDictionary["tempWaterElementResistance"] = waterElementResistance;
        tempStatusDictionary["tempThunderElementResistance"] = thunderElementResistance;
        tempStatusDictionary["tempToxicElementResistance"] = toxicElementResistance;


        StatusUIController.Instance.RefreshStatusUI();
    }

}
