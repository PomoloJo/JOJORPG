using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // 效果列表, 比如吸血等，存放对应函数
    public ArrayList effectList;

    // 可以显示出来的基础属性
    public int healthPoint;
    public int healthMaxPoint;

    public int attack;
    public int defence;
    public int penetration;
    public int criticalRate;
    public int criticalMultiple;
    
    public int elementPower;
    public int elementPenetration;
    public int elementCriticalRate;
    public int elementCriticalMultiple;

    public int fireDamage;
    public int waterDamage;
    public int thunderDamage;
    public int toxicDamage;

    public int fireElementResistance;
    public int waterElementResistance;
    public int thunderElementResistance;
    public int toxicElementResistance;

    public int moveSpeed;
    public int luck;

    // 不够位置显示的隐藏属性
    public int realDamage;



    // buff 状态
    public bool isFrenzy;


    // debuff 状态
    public bool isVertigo;
    public bool isFrozen;
    public bool isSluggish;
    public bool isWeakness;
    public bool isPoisoned;
}
