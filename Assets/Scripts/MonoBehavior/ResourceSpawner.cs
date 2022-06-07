using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 用于在地图上随机生成资源 Object, 当前脚本挂载与 Grid/TilemapGround 下
public class ResourceSpawner : MonoBehaviour
{
    // 暴露在 inspector 面板，用于拖拽预制体进去
    public List<GameObject> envPrefab = new List<GameObject> ();
    public List<GameObject> itemPrefab = new List<GameObject> ();

    // 因为生成资源的函数放在了 update 中，确保这些资源只在最初生成一次
    private int initOnce;

    // 全局静态变量，存放生成的所有资源， 类型为 gameobject
    public static List<GameObject> envResources;
    public static List<GameObject> itemList;

    // Start is called before the first frame update
    void Start()
    {
        envResources = new List<GameObject>();
        itemList = new List<GameObject>();
        initOnce = 1;
    }

    // Update is called once per frame
    void Update()
    {
        InitResourcesOnce();
    }

    // 在传入的地图中随机产生一个有效的位置
    private void SelectRandomPosition(Tilemap tilemap, out int randomPositonX, out int randomPositonY)
    {
        do
        {
            randomPositonX = Random.Range(tilemap.origin.x, tilemap.size.x+tilemap.origin.x);
            randomPositonY = Random.Range(tilemap.origin.y, tilemap.size.y+tilemap.origin.y);
        }while(tilemap.GetTile(new Vector3Int(randomPositonX, randomPositonY, 0)) == null);
    }

    private void InitResourcesOnce()
    {
        if(initOnce > 0)
        {
            Tilemap tilemapGround = RandomMap.Instance.tilemapGround;
            tilemapGround = GetComponent<Tilemap>();

            int randomPositonX;
            int randomPositonY;

            // 随机产生50棵树，可能有 Bug 万一位置重复了会不会叠放到一起？
            for(int i=0; i<50; i++)
            {
                SelectRandomPosition(tilemapGround, out randomPositonX, out randomPositonY);
                GameObject resource = envPrefab[Random.Range(0,2)];
                GameObject newObject = Instantiate(resource, new Vector3(randomPositonX, randomPositonY, 0), Quaternion.identity);
                envResources.Add(newObject);
            }

            // 随机产生100个item
            // 暂时在这个地方刷出装备
            for(int i=0; i<100; i++)
            {
                SelectRandomPosition(tilemapGround, out randomPositonX, out randomPositonY);
                GameObject item = itemPrefab[Random.Range(0,itemPrefab.Count)];
                var newObject = Instantiate(item, new Vector3(randomPositonX, randomPositonY, 0), Quaternion.identity);
                if(newObject.GetComponent<Item>().tagType == Item.TagType.装备)
                {
                    var weapon =  newObject.GetComponent<Weapon>();
                    weapon.quality = (Item.Quality)Random.Range(0, ItemUniversalMethod.GetEnumElementLength<Item.Quality>());
                    weapon.basicAttack = Random.Range(weapon.minAttack, weapon.maxAttack);
                    EntriesPool.GenerateEntries(ref weapon.EntriesRangeList, (int)weapon.quality);
                }
                newObject.GetComponent<Item>().qualityColor =  ItemUniversalMethod.GetQualityColor(newObject.GetComponent<Item>().quality);
                itemList.Add(newObject);
            }
        }
        initOnce--;
    }
}
