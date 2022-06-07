using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 用于随机生成地图， 挂载到 Grid 下
// 单例类，构造函数如下所示, 到底写得对不对？？虽然看着不太像一般的单例类，但是确实运行成功了
public class RandomMap : MonoBehaviour
{
    [SerializeField] RuleTile waterRuleTile;

    [SerializeField] RuleTile groundRuleTile;   

    public Tilemap tilemapWater;

    public Tilemap tilemapGround;

    public const int WATER_UNIT = 0;
    public const int GROUND_UNIT = 1;
    public const float WATER_PERCENT = 0.45f;

    // 地图生成的行列大小
    public int mapDataRow = 101;
    public int mapDataColumn = 101;

    // 记录地图数据，是个由0和1组成的矩阵，暂时由 0 代表 water，1 代表 ground
    public static int[,] mapData;

    // 暂时在这里设置玩家的出生点，以后再想想怎么解耦合
    public Vector3 playerSpawnPosition;

    // 构造单例类, 虽然看着不太像一般的单例类，但是确实运行成功了
    private static RandomMap _instance;
    public static RandomMap Instance
    {
        get{return _instance;}
    }


    // Start is called before the first frame update
    private void Start()
    {
        if(!_instance)
        {
            _instance = this;
            mapData = new int[mapDataRow,mapDataColumn];
            GenerateFinalMapData(4);
            SetRuleTile();
            // 设置玩家出生点，以后再解耦合
            SetPlayerSpawnPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 以一定概率产生0和1，常量控制生成0的概率，也就是水体单元格，比如说设置成0.47时，产生的陆地会比较多
    private int GenerateUnitWithWaterPercnt()
    {
        // 产生 1~101 之间的随机数，左闭右开，实际上是 1~100
        int randInt = Random.Range(1,101);
        return randInt <= 100 * WATER_PERCENT ? WATER_UNIT : GROUND_UNIT;
    }

    private void FillMapDataWithZeroAndOne()
    {
       for(int i = 0; i < mapDataRow; ++i)
        {
            for(int j=0; j < mapDataColumn; ++j)
            {
                mapData[i,j] = GenerateUnitWithWaterPercnt();
            }
        }
    }


    private int CheckNeighbourSeaUnits(int x, int y)
    {
        int waterCount = 0;
        for(int i=-1; i<2; i++)
        {
            for(int j=-1; j<2; j++)
            {
                if(i == 0 && j == 0)
                    continue;
                int aroundX = x+i;
                int aroundY = y+j;
                if(aroundX < 0 || aroundX >= mapDataRow || aroundY < 0 || aroundY >= mapDataColumn)
                {
                    waterCount += 1;
                }
                else if(mapData[aroundX,aroundY] == WATER_UNIT)
                {
                    waterCount += 1;
                }
            }
        }
        return waterCount;
    }

    private void MergeMap()
    {
        for(int i=0; i<mapDataRow; i++)
        {
            for(int j=0; j<mapDataColumn; j++)
            {
                // 取得元素周围水体单元格的数量
                int waterCount = CheckNeighbourSeaUnits(i,j);
                if(mapData[i,j] == WATER_UNIT)
                {
                    // 如果当前单元格为海洋,且周围超过4格海洋，则保持为海洋单元格
                    mapData[i,j] = waterCount >= 4 ? WATER_UNIT : GROUND_UNIT;
                }
                else
                {
                    // 如果当前单元格为陆地,且周围超过5格海洋，则陆地变海洋
                    mapData[i,j] = waterCount >= 5 ? WATER_UNIT : GROUND_UNIT;
                }
            }
        }
    }

    private void GenerateFinalMapData(int mergeTimes)
    {
        FillMapDataWithZeroAndOne();
        while(mergeTimes > 0)
        {
            MergeMap();
            mergeTimes--;
        }
    }

    private void SetRuleTile()
    {
        int mapDataRowOffset = -mapDataRow/2;
        int colOffset = -mapDataColumn/2;
        for(int i = mapDataRowOffset; i < mapDataRow + mapDataRowOffset; ++i)
        {
            for(int j = colOffset; j < mapDataColumn + colOffset; ++j)
            {
                if(mapData[i-mapDataRowOffset, j-colOffset] == 0) 
                {
                    var positionNow = new Vector3Int(i,j,0);
                    tilemapWater.SetTile(positionNow, waterRuleTile);
                }
                else
                {
                    tilemapGround.SetTile(new Vector3Int(i,j,0), groundRuleTile);
                }
            }
        }
        tilemapWater.gameObject.AddComponent<TilemapCollider2D>();
        tilemapWater.gameObject.AddComponent<CompositeCollider2D>();
        tilemapWater.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        tilemapWater.gameObject.GetComponent<TilemapCollider2D>().usedByComposite = true;
    }

    // 设置玩家出生点，以后再解耦合
    private void SetPlayerSpawnPosition()
    {
        Vector3Int randomSpawnPosition = new Vector3Int(0,0,0);
        do
        {
            randomSpawnPosition.x = Random.Range(tilemapGround.origin.x, tilemapGround.size.x+tilemapGround.origin.x);
            randomSpawnPosition.y = Random.Range(tilemapGround.origin.y, tilemapGround.size.y+tilemapGround.origin.y);
        }while(tilemapGround.GetTile(randomSpawnPosition) == null);

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerTransform.position = randomSpawnPosition;
    }
}
