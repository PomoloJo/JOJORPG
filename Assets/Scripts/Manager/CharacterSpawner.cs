using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 把地图传递给该脚本，用于规定在哪个地方生成多少个 character
public class CharacterSpawner : MonoBehaviour
{
    private static CharacterSpawner _instance;
    public static CharacterSpawner Instance
    {
        get{return _instance;}
    }

    private void Awake() 
    {
        if(!_instance)
        {
            _instance = this;   
        }
    }

    private void Start() 
    {
        characterList = new List<GameObject>();
        GenerateCharactor(new Vector3(0,0,0), 0);
    }


    // 暴露在 inspector 面板，手动添加可以生成的 prefab
    public List<GameObject> characterPrefab = new List<GameObject>();

    // 全局静态变量，存放生成的所有资源， 类型为 gameobject
    public List<GameObject> characterList;

    // 角色生成，需要指明生成的位置，生成的 prefab id，生成的数量
    public void GenerateCharactor(Vector3 position, int prefabId, int num = 1)
    {
        GameObject prefab = characterPrefab[prefabId];
        GameObject newCharacter = Instantiate(prefab, position, Quaternion.identity);
        characterList.Add(newCharacter);
    }
    

}
