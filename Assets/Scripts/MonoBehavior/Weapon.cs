using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : Item
{
    // 用于暴露在 inspector 面板方便手动调整
    public int minAttack;
    public int maxAttack;
    public int minBasicElementPower;
    public int maxBasicElementPower;



    // 基础属性，在生成实例时产生随机值
    public int basicAttack;
    public int basicElementPower;

    
    [Serializable]
    public struct EntriesRangeStruct
    {
        // 用于暴露在 inspector 面板中修改, 一般情况下，第一个参数是下限，第二个参数是上限
        public string entriesId;
        public int inputValue1;
        public int inputValue2;


        // 词条被选中
        public bool isActive;
        public string outputDescription;
        // 用于给 装备字典使用的key，增加对应属性的数值
        public string outputKey;
        public int outputValue;
        
    }

    public EntriesRangeStruct[] EntriesRangeList;
    

}
