using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntriesPool
{
    // 传入参数为：在什么范围抽，需要在这个范围抽多少个词条
    public static void GenerateEntries(ref Weapon.EntriesRangeStruct[] EntriesRangeList, int entriesNumber)
    {
        // 不重复等概率抽取词条
        for(int i = EntriesRangeList.Length - 1; i >= 0 && entriesNumber > 0; i--)
        {
            // 概率是：剩余取数长度/总数组剩余的长度
            if (UnityEngine.Random.Range(0, i + 1) < entriesNumber)
            {
                // 生成词条到原结构体的最后两个参数中
                string id = EntriesRangeList[i].entriesId;
                int parma1 = EntriesRangeList[i].inputValue1;
                int parma2 = EntriesRangeList[i].inputValue2;
                var method = entriesDictionary[id];
                var resultTuple = method(parma1, parma2);
                EntriesRangeList[i].outputDescription = resultTuple.Item1;
                EntriesRangeList[i].outputKey = resultTuple.Item2;
                EntriesRangeList[i].outputValue = resultTuple.Item3;
                // 激活词条
                EntriesRangeList[i].isActive = true;
                entriesNumber--;
            }
        }
    }


    // 返回值：描述，key，value
    public delegate Tuple<string, string, int> Method(int param1, int param2 = -1);

    public static Method AddFireDamage = 
    (int param1, int param2) =>
    {
        int outputValue = UnityEngine.Random.Range(param1, param2);
        string description = "火焰伤害+" + outputValue;
        string outputKey = "tempFireDamage";
        var resultTuple = new Tuple<string, string, int>(description, outputKey, outputValue);
        return resultTuple;
    };

    public static Method AddWaterDamage = 
    (int param1, int param2) =>
    {
        int outputValue = UnityEngine.Random.Range(param1, param2);
        string description = "水流伤害+" + outputValue;
        string outputKey = "tempWaterDamage";
        var resultTuple = new Tuple<string, string, int>(description, outputKey, outputValue);
        return resultTuple;
    };

    public static Method AddThunderDamage = 
    (int param1, int param2) =>
    {
        int outputValue = UnityEngine.Random.Range(param1, param2);
        string description = "雷电伤害+" + outputValue;
        string outputKey = "tempThunderDamage";
        var resultTuple = new Tuple<string, string, int>(description, outputKey, outputValue);
        return resultTuple;
    };

    public static Method AddToxicDamage = 
    (int param1, int param2) =>
    {
        int outputValue = UnityEngine.Random.Range(param1, param2);
        string description = "毒素伤害+" + outputValue;
        string outputKey = "tempToxicDamage";
        var resultTuple = new Tuple<string, string, int>(description, outputKey, outputValue);
        return resultTuple;
    };


    // 第一个值是 词条id, 第二个是 Method委托类型 的 委托对象
    public static Dictionary<string, Method> entriesDictionary = new Dictionary<string, Method>
    {
        {"000001", AddFireDamage},
        {"000002", AddWaterDamage},
        {"000003", AddThunderDamage},
        {"000004", AddToxicDamage},
    };


}
