using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelPlattformGenerator : MonoBehaviour
{
    public NavMeshSurface navMeshSurface;

    public GameObject[] mountainTiles;
    public GameObject[] islandTiles;
    public GameObject[] pondTiles;
    public GameObject[] hillTiles;
    public GameObject[] waterTiles;
    public GameObject[] cloudTiles;
    public GameObject[] grasTiles;
    public GameObject[] specialgrasTiles;
    public GameObject[] floatingTiles;
    public GameObject[] pathTiles;
    public GameObject[] itemProps;
    public GameObject[] enemyProps;
    public GameObject[] dungeonTiles;
    public GameObject[] weatherLights;
    public GameObject directionalLight;
    public GameObject rainFX;

    public int minTiles = 7;
    public int maxTiles = 25;



    public enum TileTyp { Void, Water, Island, Pond, Gras, Hill, Mountain, Path, Special};
    //public TileTyp[,] tileTypMap;
    public float[,] tileHeightMap;

    public Tile[,] tileMap;

    public MapGenerator mapGenerator;
    public bool genMap = true;
    //public bool genWater = false;
    //public bool genFloat = false;
    //public bool delTiles = false;
    public bool genProps = false;
    public bool findStart = false;
    public bool genNav = false;
    //public bool delWater = false;
    //public bool genPath = false;
    public bool delSepIsl = false;
    public bool genChests = false;
    public bool genDungeon = false;

    //Testing *******************
    public bool deltTest = false;
    public int Xp, Yp;

    public float landHeightRegion = 1f;
    public float voidHeightRegion = 0.5f;
    public float waterHeightRegion = 0.55f;
    public float islandHeightRegion = 0.575f;
    public float pondHeightRegion = 0.6f;
    public float hillHeightRegion = 0.9f;
    public float mountainHeightRegion = 0.925f;

    public GameObject detailProp;
    public GameObject startProp;
    public GameObject placeHolderProp;
    public GameObject levelParent;
    public GameObject enemiesParent;

    private Tile startingTile;
    private Tile finishTile;

    List<Tile> pathTilesList = new List<Tile>();
    List<Tile> islandTilesList = new List<Tile>();

    
    public void Update()
    {
        if (genMap)
        {
            genMap = false;
            int seed = Random.Range(-100000, 100000);
            //Debug.Log("Seed: " + seed);
            tileHeightMap = mapGenerator.GetNoiseMap(seed);
            tileMap = new Tile[tileHeightMap.GetLength(0), tileHeightMap.GetLength(1)];
            CreateGround(tileHeightMap);
            DeleteSoloTiles();
            CreateWaterTiles();
            DeleteWaterSoloTiles();
            DeleteSoloTiles();
            CreatePath2();
            CreateWaterTiles();
            DeleteWaterSoloTiles();
            if (FindStartPos(TileTyp.Mountain)) { }
            else if (FindStartPos(TileTyp.Hill)) { }
            else FindStartPos(TileTyp.Gras);
            DeleteSeperatedIslands();
            DeleteSoloTiles();
            CreateWaterTiles();
            DeleteWaterSoloTiles();
            DeleteSoloTiles();
            PlaceDungeon();
            CreateFloatingPlattforms();
            PlaceItems();
            levelParent.transform.localScale *= 5;          

            StartCoroutine(GenNavMeshLate());
            SetPlayer();
            GenWeather();
        }
        
        if (genProps)
        {
            CreateProps();
            genProps = false;
        }
        if (findStart)
        {
            FindStartPos();
            findStart = false;
        }
        if (genNav)
        {
            navMeshSurface.BuildNavMesh();
            genNav = false;
        }
        if (delSepIsl)
        {
            DeleteSeperatedIslands();
            delSepIsl = false;
        }
        if (genChests)
        {
            PlaceItems();
            genChests = false;
        }
        if (genDungeon)
        {
            PlaceDungeon();
            genDungeon = false;
        }        
    }

    IEnumerator GenNavMeshLate()
    {
        yield return new WaitForSeconds(0.25f);
        navMeshSurface.BuildNavMesh();
        PlaceEnemies();
    }

    private void GenWeather()
    {
        rainFX.SetActive(false);
        int w = Random.Range(0, weatherLights.Length);
        GameObject weather = weatherLights[w];
        Instantiate(weather, new Vector3(0, 3, 0), weather.transform.rotation);
        Destroy(directionalLight);
        if (w == 2) rainFX.SetActive(true);
        else if(w == 1 & Random.value > 0.5f) rainFX.SetActive(true);
    }

    private void SetPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = startingTile.tile.transform.GetChild(0).position;
    }

    public void CreateGround(float[,] noiseMap)
    {
        float offset = 0;
        for (int i = 0; i < tileHeightMap.GetLength(0); i++)
        {
            if (i % 2 == 0) offset = 0;
            else offset = 2.5f;

            for (int j = 0; j < tileHeightMap.GetLength(1); j++)
            {
                if (tileHeightMap[i, j] > voidHeightRegion)
                {
                    if (tileHeightMap[i, j] > mountainHeightRegion)
                    {
                        tileMap[i, j].tile = Instantiate(mountainTiles[Random.Range(0, mountainTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Mountain;
                    }
                    else if (tileHeightMap[i, j] > hillHeightRegion)
                    {
                        tileMap[i, j].tile = Instantiate(hillTiles[Random.Range(0, hillTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Hill;
                    }
                    else if (tileHeightMap[i, j] > pondHeightRegion)
                    {
                        if(Random.value < 0.2f) tileMap[i, j].tile = Instantiate(specialgrasTiles[Random.Range(0, specialgrasTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        else tileMap[i, j].tile = Instantiate(grasTiles[Random.Range(0, grasTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Gras;
                    }
                    else if (tileHeightMap[i, j] > islandHeightRegion)
                    {
                        tileMap[i, j].tile = Instantiate(pondTiles[Random.Range(0, pondTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Pond;
                    }
                    else if (tileHeightMap[i, j] > waterHeightRegion)
                    {
                        tileMap[i, j].tile = Instantiate(islandTiles[Random.Range(0, islandTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Island;
                    }
                    else
                    {
                        tileMap[i, j].tile = Instantiate(waterTiles[Random.Range(0, waterTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Water;
                    }
                    tileMap[i, j].posX = i;
                    tileMap[i, j].posY = j;
                }
                else
                {
                    tileMap[i, j].typ = TileTyp.Void;
                    tileMap[i, j].posX = i;
                    tileMap[i, j].posY = j;
                }
            }
        }
    }

    public void DeleteSoloTiles()
    {
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j].typ == TileTyp.Void) continue;
                int tileCounter = 0;    
                if (i % 2 == 0)
                {
                    if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
                    {
                        if (tileMap[i + 1, j - 1].typ != TileTyp.Void) tileCounter++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        if (tileMap[i - 1, j - 1].typ != TileTyp.Void) tileCounter++;
                    }
                }
                else
                {
                    if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i + 1, j + 1].typ == TileTyp.Void) tileCounter++;
                    }
                    if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i - 1, j + 1].typ != TileTyp.Void) tileCounter++;
                    }
                }

                if (i - 1 >= 0)
                {
                    if (tileMap[i - 1, j].typ != TileTyp.Void) tileCounter++;
                }
                if (i + 1 < tileMap.GetLength(0))
                {
                    if (tileMap[i + 1, j].typ != TileTyp.Void) tileCounter++;
                }
                if (j + 1 < tileMap.GetLength(1))
                {
                    if (tileMap[i, j + 1].typ != TileTyp.Void) tileCounter++;
                }
                if (j - 1 >= 0)
                {
                    if (tileMap[i, j - 1].typ != TileTyp.Void) tileCounter++;
                }

                if (tileCounter <= 1)
                {
                    Destroy(tileMap[i, j].tile);
                    tileMap[i, j].typ = TileTyp.Void;
                    tileMap[i, j].posX = i;
                    tileMap[i, j].posY = j;
                }
                
            }
        }
    }

    public void DeleteWaterSoloTiles()
    {
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j].typ != TileTyp.Water) continue;
                int tileCounter = 0;
                if (i % 2 == 0)
                {
                    if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
                    {
                        if (tileMap[i + 1, j - 1].typ == TileTyp.Water) tileCounter++;
                    }
                    if (i - 1 >= 0 && j - 1 >= 0)
                    {
                        if (tileMap[i - 1, j - 1].typ == TileTyp.Water) tileCounter++;
                    }
                }
                else
                {
                    if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i + 1, j + 1].typ == TileTyp.Water) tileCounter++;
                    }
                    if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i - 1, j + 1].typ == TileTyp.Water) tileCounter++;
                    }
                }

                if (i - 1 >= 0)
                {
                    if (tileMap[i - 1, j].typ == TileTyp.Water) tileCounter++;
                }
                if (i + 1 < tileMap.GetLength(0))
                {
                    if (tileMap[i + 1, j].typ == TileTyp.Water) tileCounter++;
                }
                if (j + 1 < tileMap.GetLength(1))
                {
                    if (tileMap[i, j + 1].typ == TileTyp.Water) tileCounter++;
                }
                if (j - 1 >= 0)
                {
                    if (tileMap[i, j - 1].typ == TileTyp.Water) tileCounter++;
                }

                if (tileCounter <= 0)
                {
                    Destroy(tileMap[i, j].tile);
                    tileMap[i, j].typ = TileTyp.Void;
                    tileMap[i, j].posX = i;
                    tileMap[i, j].posY = j;
                }

            }
        }
    }

    public void CreateWaterTiles()
    {
        float offset = 0f;
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            if (i % 2 == 0) offset = 0f;
            else offset = 2.5f;
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j].typ == TileTyp.Void)
                {
                    int tileCounter = GetNeighborsNumber(i, j);
                    if (tileCounter >= 2)
                    {
                        tileMap[i, j].tile = Instantiate(waterTiles[Random.Range(0, waterTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                        tileMap[i, j].typ = TileTyp.Water;
                        tileMap[i, j].posX = i;
                        tileMap[i, j].posY = j;
                    }
                }
            }
        }
    }

    public void CreateFloatingPlattforms()
    {
        float offset = 0f;
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            if (i % 2 == 0) offset = 0f;
            else offset = 2.5f;
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if(tileMap[i,j].typ != TileTyp.Void && tileMap[i, j].typ != TileTyp.Water) Instantiate(floatingTiles[0], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                else if(tileMap[i, j].typ == TileTyp.Water) Instantiate(floatingTiles[1], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
            }
        }
    }

    public void CreateProps()
    {
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if(tileMap[i,j].typ == TileTyp.Gras && !(i == startingTile.posX && j == startingTile.posY))
                {
                    Vector3 pos = tileMap[i, j].tile.transform.position;
                    Vector2 rndPos = Random.insideUnitCircle * 1.5f;
                    Vector3 placePos = pos + new Vector3(rndPos.x,0,rndPos.y);
                    Instantiate(detailProp, placePos, Quaternion.Euler(0, Random.Range(-180, 180), 0), levelParent.transform);
                }
                if(tileMap[i, j].typ != TileTyp.Void && tileMap[i, j].typ != TileTyp.Water && Random.Range(0f, 1f) > .8f)
                {
                    Vector3 pos = tileMap[i, j].tile.transform.position + new Vector3(0, Random.Range(-0.5f, 0.5f), 0);
                    float rnd = Random.value;
                    if (rnd >= 0.95f) Instantiate(cloudTiles[0], pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                    else if (rnd >= 0.87f) Instantiate(cloudTiles[Random.Range(11, 15)], pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                    else if (rnd >= 0.70f) Instantiate(cloudTiles[Random.Range(5, 11)], pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                    else Instantiate(cloudTiles[Random.Range(1, 5)], pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                }
            }
        }
    }


    //Not Workling =(
    public void CreatePath() {
        int i = startingTile.posX;
        int j = startingTile.posY;

        
        //Create List with all Possible PathTiles
        Tile pathTile = GetNextPathTile(i,j);

        while(pathTile.tile != null)
        {
            pathTile = GetNextPathTile(pathTile.posX, pathTile.posY);
        }

        foreach (Tile item in pathTilesList)
        {
            tileMap[item.posX, item.posY].distance = 100000;
        }

        Debug.Log("Wege: " + pathTilesList.Count);

        //Check if PathLength >= 3 so Path will be long enough
        if (pathTilesList.Count >= 3)
        {
            //Djisktra
            Tile startPath = pathTilesList[0];
            Tile finishPath = pathTilesList[pathTilesList.Count - 1];

            Debug.Log("Start x: " + startPath.posX + " y: " + startPath.posY);
            Debug.Log("Finish x: " + finishPath.posX + " y: " + finishPath.posY);

            List<Tile> openList = new List<Tile>();
            List<Tile> closedList = new List<Tile>();

            startPath.distance = 0;
            openList.Add(startPath);

            Tile t = openList[0];

            while (openList.Count > 0)
            {
                t = openList[0];
                List<Tile> neighborTiles = GetNeighbors(t.posX, t.posY);

                for (int n = 0; n < neighborTiles.Count; n++)
                {
                    if (neighborTiles[n].typ == TileTyp.Gras && !closedList.Contains(neighborTiles[n]) && !openList.Contains(neighborTiles[n]))
                    {
                        tileMap[neighborTiles[n].posX, neighborTiles[n].posY].distance = t.distance + 1;
                        openList.Add(neighborTiles[n]);
                    }else if (closedList.Contains(neighborTiles[n]))
                    {
                        if(tileMap[neighborTiles[n].posX, neighborTiles[n].posY].distance > t.distance + 1)
                        {
                            tileMap[neighborTiles[n].posX, neighborTiles[n].posY].distance = t.distance + 1;
                        }
                    }
                }
                openList.Remove(t);
                closedList.Add(t);
                if (closedList.Contains(finishPath)) break;

                //kleinersterDistance aus OpenList

                
            }

            //Place Path Backwards
            if (closedList.Contains(finishPath))
            {
                Debug.Log("Ziel in Closed");
                Tile actualTile = finishPath;
                Vector3 pos = finishPath.tile.transform.position;
                Destroy(tileMap[actualTile.posX, actualTile.posY].tile);
                tileMap[finishPath.posX, finishPath.posY].tile = Instantiate(pathTiles[1], pos, Quaternion.identity, levelParent.transform);
                tileMap[finishPath.posX, finishPath.posY].typ = TileTyp.Path;
                closedList.Remove(finishPath);

                Debug.Log("Start2 x: " + startPath.posX + " y: " + startPath.posY);
                Debug.Log("Actual x: " + actualTile.posX + " y: " + actualTile.posY);

                int x = actualTile.posX;
                int y = actualTile.posY;
                int sx = startPath.posX;
                int sy = startPath.posY;
                while ((x != sx) && (y != sy))
                {
                    Debug.Log("MacheWeeeg");
                    if (closedList.Count <= 0)
                    {
                        Debug.Log("ClosedListEmpty -.-");
                        break;
                    }
                    actualTile = GetMinDistanceTile(actualTile, closedList);
                    if (actualTile.tile == null)
                    {
                        Debug.Log("PathTile is dump -.-");
                        break;
                    }

                    x = actualTile.posX;
                    y = actualTile.posY;

                    pos = actualTile.tile.transform.position;
                    Destroy(tileMap[actualTile.posX, actualTile.posY].tile);
                    tileMap[actualTile.posX, actualTile.posY].tile = Instantiate(pathTiles[2], pos, Quaternion.identity, levelParent.transform);
                    tileMap[actualTile.posX, actualTile.posY].typ = TileTyp.Path;
                    closedList.Remove(actualTile);
                    Debug.Log("Actual x: " + actualTile.posX + " y: " + actualTile.posY);
                }

                Debug.Log("Start x: " + startPath.posX + " y: " + startPath.posY);
                Debug.Log("ActualFin x: " + actualTile.posX + " y: " + actualTile.posY);

                pos = startPath.tile.transform.position;
                Destroy(tileMap[startPath.posX, startPath.posY].tile);
                tileMap[startPath.posX, startPath.posY].tile = Instantiate(pathTiles[0], pos, Quaternion.identity, levelParent.transform);
                tileMap[startPath.posX, startPath.posY].typ = TileTyp.Path;

            }
            else
            {
                Debug.Log("No Path Possible");
            }

        }
    #region
    /*
    //int pathLength = Random.Range(3, pathTilesList.Count);
    //Debug.Log("PathLength: " + pathLength);

    //Vector3 pos = pathTilesList[0].tile.transform.position;
    //Destroy(pathTilesList[0].tile);
    //tileMap[pathTilesList[0].posX, pathTilesList[0].posY].tile = Instantiate(pathTiles[0], pos, Quaternion.identity);
    //tileMap[pathTilesList[0].posX, pathTilesList[0].posY].typ = TileTyp.Path;




    //for (int k = 1; k < pathLength; k++)
    //{
    //    pos = pathTilesList[k].tile.transform.position;
    //    Destroy(tileMap[pathTilesList[k].posX, pathTilesList[k].posY].tile);
    //    tileMap[pathTilesList[k].posX, pathTilesList[k].posY].tile = Instantiate(pathTiles[0], pos, Quaternion.identity);
    //    tileMap[pathTilesList[k].posX, pathTilesList[k].posY].typ = TileTyp.Path;                
    //}

    //for (int k = 1; k < pathLength; k++)
    //{
    //    List<Tile> neighbors = new List<Tile>();
    //    neighbors = GetNeighbors(pathTilesList[k].posX, pathTilesList[k].posY);
    //    int pathsAround = 0;
    //    foreach (Tile item in neighbors)
    //    {
    //        if (item.typ == TileTyp.Path) pathsAround++;
    //    }
    //    Debug.Log(pathsAround);

    //    GameObject path = null;
    //    switch (pathsAround)
    //    {                    
    //        case 1: path = pathTiles[0];
    //            break;
    //        case 2:
    //            path = pathTiles[2];
    //            break;
    //        case 3:
    //            path = pathTiles[6];
    //            break;
    //        case 4:
    //            path = pathTiles[9];
    //            break;
    //        case 5:
    //            path = pathTiles[10];
    //            break;
    //        case 6:
    //            path = pathTiles[11];
    //            break;
    //        default:
    //            break;
    //    }
    //    if(path != null)
    //    {
    //        pos = pathTilesList[k].tile.transform.position;
    //        //Destroy(pathTilesList[k].tile);
    //        Destroy(tileMap[pathTilesList[k].posX, pathTilesList[k].posY].tile);
    //        tileMap[pathTilesList[k].posX, pathTilesList[k].posY].tile = Instantiate(path, pos, Quaternion.identity);
    //        tileMap[pathTilesList[k].posX, pathTilesList[k].posY].typ = TileTyp.Path;
    //    }                
    //} 

    */
    #endregion

    }
    
    public void CreatePath2()
    {

        foreach (Tile tile in tileMap)
        {
            if(tile.typ == TileTyp.Gras) pathTilesList.Add(tileMap[tile.posX, tile.posY]);
        }        

        Tile startpathTile = pathTilesList[Random.Range(0, pathTilesList.Count)];
        Tile endpathTile = pathTilesList[Random.Range(0, pathTilesList.Count)];

        if (startpathTile.posX == endpathTile.posX && startpathTile.posY == endpathTile.posY)
        {
            //Debug.Log("Same TileSelected");
            CreatePath2();
            return;
        }

        int x = startpathTile.posX;
        int y = startpathTile.posY;

        int ex = endpathTile.posX;
        int ey = endpathTile.posY;

        Vector3 pos = startpathTile.tile.transform.position;
        Destroy(tileMap[startpathTile.posX, startpathTile.posY].tile);
        tileMap[startpathTile.posX, startpathTile.posY].tile = Instantiate(pathTiles[Random.Range(20, 24)], pos, Quaternion.identity, levelParent.transform);
        tileMap[startpathTile.posX, startpathTile.posY].typ = TileTyp.Path;

        
        if (x > ex) {
            if (x % 2 == 0)
            {
                tileMap[x, y].tile.transform.Rotate(new Vector3(0, 180, 0));
                x--;
            }
            else
            {
                tileMap[x, y].tile.transform.Rotate(new Vector3(0, 120, 0));
                x--;
            }
        }else if(x < ex)
        {
            if (x % 2 == 0)
            {
                tileMap[x, y].tile.transform.Rotate(new Vector3(0, -60, 0));
                x++;
            }
            else
            {
                tileMap[x, y].tile.transform.Rotate(new Vector3(0, 0, 0));
                x++;
            }
        }else if(y < ey)
        {
            tileMap[x, y].tile.transform.Rotate(new Vector3(0, -120, 0));
            y++;
        }else if (y > ey)
        {
            tileMap[x, y].tile.transform.Rotate(new Vector3(0, 60, 0));
            y--;
        }

        bool firstChange = true;        
        while (true)
        {
            if(tileMap[x,y].tile == null)
            {
                float offset;
                if (x % 2 == 0) offset = 0f;
                else offset = 2.5f;
                tileMap[x, y].tile = Instantiate(grasTiles[Random.Range(0, grasTiles.Length)], new Vector3(x * 4.33f, 0, (y * 5f) + offset), Quaternion.identity, levelParent.transform);
                tileMap[x, y].typ = TileTyp.Gras;
            }
            if (x > endpathTile.posX)
            {
                if(x % 2 == 0)
                {
                    pos = tileMap[x,y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0,-60,0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    x--;
                }
                else
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, 120, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    x--;
                }

            }else if (x < endpathTile.posX)
            {
                if (x % 2 == 0)
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, -60, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    x++;
                }
                else
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, 120, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    x++;
                }
            }
            else
            {
                if (firstChange)
                {
                    firstChange = false;
                    if(startpathTile.posX != endpathTile.posX && startpathTile.posY != endpathTile.posY)
                    {
                        int xStart = startpathTile.posX;
                        int xEnd = endpathTile.posX;
                        int yStart = startpathTile.posY;
                        int yEnd = endpathTile.posY;                    

                        if (xStart > xEnd)
                        {
                            if(yStart > yEnd)
                            {
                                pos = tileMap[x, y].tile.transform.position;
                                Destroy(tileMap[x, y].tile);
                                if (x % 2 == 0) tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, 60, 0), levelParent.transform);
                                else tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(12, 16)], pos, Quaternion.Euler(0, 60, 0), levelParent.transform);
                                tileMap[x, y].typ = TileTyp.Path;
                                y--;
                            }
                            else
                            {
                                pos = tileMap[x, y].tile.transform.position;
                                Destroy(tileMap[x, y].tile);
                                if (x % 2 == 0) tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(12, 16)], pos, Quaternion.Euler(0, -60, 0), levelParent.transform);
                                else tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, 0, 0), levelParent.transform);
                                tileMap[x, y].typ = TileTyp.Path;
                                y++;
                            }
                        }
                        else
                        {
                            if (yStart > yEnd)
                            {
                                pos = tileMap[x, y].tile.transform.position;
                                Destroy(tileMap[x, y].tile);
                                if (x % 2 == 0) tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, -180, 0), levelParent.transform);
                                else tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(12, 16)], pos, Quaternion.Euler(0, -240, 0), levelParent.transform);
                                tileMap[x, y].typ = TileTyp.Path;
                                y--;
                            }
                            else
                            {
                                pos = tileMap[x, y].tile.transform.position;
                                Destroy(tileMap[x, y].tile);
                                if (x % 2 == 0) tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(12, 16)], pos, Quaternion.Euler(0, -120, 0), levelParent.transform);
                                else tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(8, 12)], pos, Quaternion.Euler(0, -120, 0), levelParent.transform);
                                tileMap[x, y].typ = TileTyp.Path;
                                y++;
                            }
                        }
                    }
                }
                else if (y > endpathTile.posY)
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(0, 8)], pos, Quaternion.Euler(0, 60, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    y--;
                }
                else if (y < endpathTile.posY)
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(0, 8)], pos, Quaternion.Euler(0, 60, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;
                    y++;
                }
                else
                {
                    pos = tileMap[x, y].tile.transform.position;
                    Destroy(tileMap[x, y].tile);
                    tileMap[x, y].tile = Instantiate(pathTiles[Random.Range(16, pathTiles.Length)], pos, Quaternion.Euler(0, 0, 0), levelParent.transform);
                    tileMap[x, y].typ = TileTyp.Path;

                    if (startpathTile.posY > y) tileMap[x, y].tile.transform.Rotate(new Vector3(0, -120, 0));
                    else if (startpathTile.posY < y) tileMap[x, y].tile.transform.Rotate(new Vector3(0, 60, 0));
                    else if (x%2 == 0)
                    {
                        if (startpathTile.posX > x) tileMap[x, y].tile.transform.Rotate(new Vector3(0, 300, 0));
                        else tileMap[x, y].tile.transform.Rotate(new Vector3(0, 180, 0));
                    }
                    else
                    {
                        if (startpathTile.posX > x) tileMap[x, y].tile.transform.Rotate(new Vector3(0, 0, 0));
                        else tileMap[x, y].tile.transform.Rotate(new Vector3(0, 120, 0));
                    }
                    break;
                }
            }
        }

    }

    private Tile GetMinDistanceTile(Tile t, List<Tile> closed)
    {
        Tile actualTile = t;
        List<Tile> actNeighbors = new List<Tile>();
        actNeighbors = GetNeighbors(t.posX, t.posY);

        if (actNeighbors.Count > 0)
        {
            //actualTile = actNeighbors[0];
            int minDistance = tileMap[t.posX, t.posY].distance;
            Debug.Log("Distance F: " + minDistance);
            foreach (Tile item in actNeighbors)
            {
                if (minDistance >= tileMap[item.posX, item.posY].distance && closed.Contains(item))
                {
                    actualTile = item;
                    minDistance = tileMap[item.posX, item.posY].distance;
                }
            }
            Debug.Log("Distance A: " + minDistance);
        }
        if (actualTile.tile != t.tile) return actualTile;
        else return new Tile();

    }

    private Tile GetNextPathTile(int i, int j)
    {
        if (i % 2 == 0)
        {
            if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
            {
                if (tileMap[i + 1, j - 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i + 1, j - 1]))
                {
                    pathTilesList.Add(tileMap[i + 1, j - 1]);
                    return tileMap[i + 1, j - 1];
                }
            }
            if (i - 1 >= 0 && j - 1 >= 0)
            {
                if (tileMap[i - 1, j - 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i - 1, j - 1]))
                {
                    pathTilesList.Add(tileMap[i - 1, j - 1]);
                    return tileMap[i - 1, j - 1];
                }
            }
        }
        else
        {
            if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i + 1, j + 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i + 1, j + 1]))
                {
                    pathTilesList.Add(tileMap[i + 1, j + 1]);
                    return tileMap[i + 1, j + 1];
                }
            }
            if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i - 1, j + 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i - 1, j + 1]))
                {
                    pathTilesList.Add(tileMap[i - 1, j + 1]);
                    return tileMap[i - 1, j + 1];
                }
            }
        }

        if (i - 1 >= 0)
        {
            if (tileMap[i - 1, j].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i - 1, j]))
            {
                pathTilesList.Add(tileMap[i - 1, j]);
                return tileMap[i - 1, j];
            }
        }
        if (i + 1 < tileMap.GetLength(0))
        {
            if (tileMap[i + 1, j].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i + 1, j]))
            {
                pathTilesList.Add(tileMap[i + 1, j]);
                return tileMap[i + 1, j];
            }
        }
        if (j + 1 < tileMap.GetLength(1))
        {
            if (tileMap[i, j + 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i, j + 1]))
            {
                pathTilesList.Add(tileMap[i, j + 1]);
                return tileMap[i, j + 1];
            }
        }
        if (j - 1 >= 0)
        {
            if (tileMap[i, j - 1].typ == TileTyp.Gras && !pathTilesList.Contains(tileMap[i, j - 1]))
            {
                pathTilesList.Add(tileMap[i, j - 1]);
                return tileMap[i, j - 1];
            }
        }
        return new Tile(null,TileTyp.Void,-1,-1,0);
    }

    public int GetNeighborsNumber(int i, int j)
    {
        int tileCounter = 0;
        if (i % 2 == 0)
        {
            if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
            {
                if (tileMap[i + 1, j - 1].typ != TileTyp.Void && tileMap[i + 1, j - 1].typ != TileTyp.Water) tileCounter++;
            }   
            if (i - 1 >= 0 && j - 1 >= 0)
            {
                if (tileMap[i - 1, j - 1].typ != TileTyp.Void && tileMap[i - 1, j - 1].typ != TileTyp.Water) tileCounter++;
            }
        }
        else
        {            
            if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i + 1, j + 1].typ != TileTyp.Void && tileMap[i + 1, j + 1].typ != TileTyp.Water) tileCounter++;
            }
            if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i - 1, j + 1].typ != TileTyp.Void && tileMap[i - 1, j + 1].typ != TileTyp.Water) tileCounter++;
            }
        }
        
        if (i - 1 >= 0)
        {
            if (tileMap[i - 1, j].typ != TileTyp.Void && tileMap[i - 1, j].typ != TileTyp.Water) tileCounter++;
        }
        if (i + 1 < tileMap.GetLength(0))
        {
            if (tileMap[i + 1, j].typ != TileTyp.Void && tileMap[i + 1, j].typ != TileTyp.Water) tileCounter++;
        }
        if (j + 1 < tileMap.GetLength(1))
        {
            if (tileMap[i, j + 1].typ != TileTyp.Void && tileMap[i, j + 1].typ != TileTyp.Water) tileCounter++;
        }
        if (j - 1 >= 0)
        {
            if (tileMap[i, j - 1].typ != TileTyp.Void && tileMap[i, j - 1].typ != TileTyp.Water) tileCounter++;
        }

        return tileCounter;
    }

    public List<Tile> GetNeighbors(int i, int j)
    {
        List<Tile> neighborTiles = new List<Tile>();
        if (i % 2 == 0)
        {
            if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
            {
                if (tileMap[i + 1, j - 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i + 1, j - 1]);
            }
            if (i - 1 >= 0 && j - 1 >= 0)
            {
                if (tileMap[i - 1, j - 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i - 1, j - 1]);
            }
        }
        else
        {
            if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i + 1, j + 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i + 1, j + 1]);
            }
            if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
            {
                if (tileMap[i - 1, j + 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i - 1, j + 1]);
            }
        }

        if (i - 1 >= 0)
        {
            if (tileMap[i - 1, j].typ != TileTyp.Void) neighborTiles.Add(tileMap[i - 1, j]);
        }
        if (i + 1 < tileMap.GetLength(0))
        {
            if (tileMap[i + 1, j].typ != TileTyp.Void) neighborTiles.Add(tileMap[i + 1, j]);
        }
        if (j + 1 < tileMap.GetLength(1))
        {
            if (tileMap[i, j + 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i, j + 1]);
        }
        if (j - 1 >= 0)
        {
            if (tileMap[i, j - 1].typ != TileTyp.Void) neighborTiles.Add(tileMap[i, j - 1]);
        }

        return neighborTiles;
    }

    public void FindStartPos()
    {
        List<Tile> checkedTiles = new List<Tile>();
        List<Tile> neighborTiles = new List<Tile>();

        Tile actualTile;

        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j].typ == TileTyp.Mountain)
                {
                    actualTile = tileMap[i, j];
                    checkedTiles.Add(tileMap[i, j]);

                    if (i % 2 == 0)
                    {
                        if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
                        {
                            if (tileMap[i + 1, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j - 1]);
                        }
                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            if (tileMap[i - 1, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j - 1]);
                        }
                    }
                    else
                    {
                        if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[i - 1, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j + 1]);
                        }
                        if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[i + 1, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j + 1]);
                        }
                    }


                    if (i - 1 >= 0)
                    {
                        if (tileMap[i - 1, j].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j]);
                    }
                    if (i + 1 < tileMap.GetLength(0))
                    {
                        if (tileMap[i + 1, j].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j]);
                    }

                    if (j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i, j + 1]);
                    }
                    if (j - 1 >= 0)
                    {
                        if (tileMap[i, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i, j - 1]);
                    }

                    while (neighborTiles.Count > 0)
                    {
                        int x = neighborTiles[0].posX;
                        int y = neighborTiles[0].posY;

                        if (x == 0 || y == 0 || x == tileMap.GetLength(0) - 1 || y == tileMap.GetLength(1) - 1)
                        {
                            SetStartTile(neighborTiles[0]);
                            return;
                        }

                        if (x % 2 == 0)
                        {
                            if (x + 1 < tileMap.GetLength(0) && y - 1 >= 0)
                            {
                                if (tileMap[x + 1, y - 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return;
                                }
                                else if (tileMap[x + 1, y - 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x + 1, y - 1]) && !checkedTiles.Contains(tileMap[x + 1, y - 1]))
                                        neighborTiles.Add(tileMap[x + 1, y - 1]);
                                }
                            }

                            if (x - 1 >= 0 && y - 1 >= 0)
                            {
                                if (tileMap[x - 1, y - 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return;
                                }
                                else if (tileMap[x - 1, y - 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x - 1, y - 1]) && !checkedTiles.Contains(tileMap[x - 1, y - 1]))
                                        neighborTiles.Add(tileMap[x - 1, y - 1]);
                                }
                            }
                        }
                        else
                        {
                            if (x - 1 >= 0 && y + 1 < tileMap.GetLength(1))
                            {
                                if (tileMap[x - 1, y + 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return;
                                }
                                else if (tileMap[x - 1, y + 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x - 1, y + 1]) && !checkedTiles.Contains(tileMap[x - 1, y + 1]))
                                        neighborTiles.Add(tileMap[x - 1, y + 1]);
                                }

                            }

                            if (x + 1 < tileMap.GetLength(0) && y + 1 < tileMap.GetLength(1))
                            {
                                if (tileMap[x + 1, y + 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return;
                                }
                                else if (tileMap[x + 1, y + 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x + 1, y + 1]) && !checkedTiles.Contains(tileMap[x + 1, y + 1]))
                                        neighborTiles.Add(tileMap[x + 1, y + 1]);
                                }
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            if (tileMap[x - 1, y].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return;
                            }
                            else if (tileMap[x - 1, y].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x - 1, y]) && !checkedTiles.Contains(tileMap[x - 1, y]))
                                    neighborTiles.Add(tileMap[x - 1, y]);
                            }
                        }
                        if (x + 1 < tileMap.GetLength(0))
                        {
                            if (tileMap[x + 1, y].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return;
                            }
                            else if (tileMap[x + 1, y].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x + 1, y]) && !checkedTiles.Contains(tileMap[x + 1, y]))
                                    neighborTiles.Add(tileMap[x + 1, y]);
                            }
                        }

                        if (y + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[x, y + 1].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return;
                            }
                            else if (tileMap[x, y + 1].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x, y + 1]) && !checkedTiles.Contains(tileMap[x, y + 1]))
                                    neighborTiles.Add(tileMap[x, y + 1]);
                            }

                        }
                        if (y - 1 >= 0)
                        {
                            if (tileMap[x, y - 1].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return;
                            }
                            else if (tileMap[x, y - 1].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x, y - 1]) && !checkedTiles.Contains(tileMap[x, y - 1]))
                                    neighborTiles.Add(tileMap[x, y - 1]);
                            }

                        }

                        checkedTiles.Add(neighborTiles[0]);
                        neighborTiles.Remove(neighborTiles[0]);
                    }
                }
            }
        }
    }

    public bool FindStartPos(TileTyp typ)
    {
        List<Tile> checkedTiles = new List<Tile>();
        List<Tile> neighborTiles = new List<Tile>();

        Tile actualTile;

        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {             
                if (tileMap[i, j].typ == typ)
                {
                    actualTile = tileMap[i, j];
                    checkedTiles.Add(tileMap[i, j]);

                    if(i % 2 == 0)
                    {              
                        if (i + 1 < tileMap.GetLength(0) && j - 1 >= 0)
                        {
                            if (tileMap[i + 1, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j - 1]);
                        }
                        if (i - 1 >= 0 && j - 1 >= 0)
                        {
                            if (tileMap[i - 1, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j - 1]);
                        }
                    }
                    else
                    {
                        if (i - 1 >= 0 && j + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[i - 1, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j + 1]);
                        }
                        if (i + 1 < tileMap.GetLength(0) && j + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[i + 1, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j + 1]);
                        }
                    }


                    if (i - 1 >= 0)
                    {
                        if (tileMap[i - 1, j].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i - 1, j]);                          
                    }
                    if (i + 1 < tileMap.GetLength(0))
                    {
                        if (tileMap[i + 1, j].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i + 1, j]);
                    }
                    
                    if (j + 1 < tileMap.GetLength(1))
                    {
                        if (tileMap[i, j + 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i, j + 1]);
                    }
                    if (j - 1 >= 0)
                    {
                        if (tileMap[i, j - 1].typ == TileTyp.Gras) neighborTiles.Add(tileMap[i, j - 1]);
                    }

                    while(neighborTiles.Count > 0)
                    {
                        int x = neighborTiles[0].posX;
                        int y = neighborTiles[0].posY;

                        if (x == 0 || y == 0 || x == tileMap.GetLength(0)-1 || y == tileMap.GetLength(1)-1)
                        {
                            SetStartTile(neighborTiles[0]);
                            return true;
                        }
                        
                        if (x % 2 == 0)
                        {
                            if (x + 1 < tileMap.GetLength(0) && y - 1 >= 0)
                            {
                                if (tileMap[x + 1, y - 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return true;
                                }
                                else if (tileMap[x + 1, y - 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x + 1, y - 1]) && !checkedTiles.Contains(tileMap[x + 1, y - 1]))
                                        neighborTiles.Add(tileMap[x + 1, y - 1]);
                                }
                            }

                            if (x - 1 >= 0 && y - 1 >= 0)
                            {
                                if (tileMap[x - 1, y - 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return true;
                                }
                                else if (tileMap[x - 1, y - 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x - 1, y - 1]) && !checkedTiles.Contains(tileMap[x - 1, y - 1]))
                                        neighborTiles.Add(tileMap[x - 1, y - 1]);
                                }
                            }
                        }
                        else
                        {
                            if (x - 1 >= 0 && y + 1 < tileMap.GetLength(1))
                            {
                                if (tileMap[x - 1, y + 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return true;
                                }
                                else if (tileMap[x - 1, y + 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x - 1, y + 1]) && !checkedTiles.Contains(tileMap[x - 1, y + 1]))
                                        neighborTiles.Add(tileMap[x - 1, y + 1]);
                                }

                            }

                            if (x + 1 < tileMap.GetLength(0) && y + 1 < tileMap.GetLength(1))
                            {
                                if (tileMap[x + 1, y + 1].typ == TileTyp.Void)
                                {
                                    SetStartTile(neighborTiles[0]);
                                    return true;
                                }
                                else if (tileMap[x + 1, y + 1].typ == TileTyp.Gras)
                                {
                                    if (!neighborTiles.Contains(tileMap[x + 1, y + 1]) && !checkedTiles.Contains(tileMap[x + 1, y + 1]))
                                        neighborTiles.Add(tileMap[x + 1, y + 1]);
                                }
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            if (tileMap[x - 1, y].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return true;
                            }else if (tileMap[x - 1, y].typ == TileTyp.Gras)
                            {
                                if(!neighborTiles.Contains(tileMap[x - 1, y]) && !checkedTiles.Contains(tileMap[x - 1, y]))
                                    neighborTiles.Add(tileMap[x - 1, y]);
                            }
                        }
                        if (x + 1 < tileMap.GetLength(0))
                        {
                            if (tileMap[x + 1, y].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return true;
                            }
                            else if(tileMap[x + 1, y].typ == TileTyp.Gras) {
                                if (!neighborTiles.Contains(tileMap[x + 1, y]) && !checkedTiles.Contains(tileMap[x + 1, y]))
                                    neighborTiles.Add(tileMap[x + 1, y]);
                            } 
                        }
                            
                        if (y + 1 < tileMap.GetLength(1))
                        {
                            if (tileMap[x, y + 1].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return true;
                            }
                            else if(tileMap[x, y + 1].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x, y + 1]) && !checkedTiles.Contains(tileMap[x, y + 1]))
                                    neighborTiles.Add(tileMap[x, y + 1]);
                            }
                                
                        }
                        if (y - 1 >= 0)
                        {
                            if (tileMap[x, y - 1].typ == TileTyp.Void)
                            {
                                SetStartTile(neighborTiles[0]);
                                return true;
                            }
                            else if(tileMap[x, y - 1].typ == TileTyp.Gras)
                            {
                                if (!neighborTiles.Contains(tileMap[x, y - 1]) && !checkedTiles.Contains(tileMap[x, y - 1]))
                                    neighborTiles.Add(tileMap[x, y - 1]);
                            }
                                
                        }
                        
                        checkedTiles.Add(neighborTiles[0]);
                        neighborTiles.Remove(neighborTiles[0]);
                    }
                }                
            }
        }
        return false;
    }

    public void SetStartTile(Tile start)
    {
        startingTile = start;
        GameObject startPlattform = Instantiate(startProp, start.tile.transform.position, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
        startingTile = new Tile(startPlattform,TileTyp.Special, start.posX,start.posY, start.distance);
        tileMap[start.posX, start.posY] = startingTile;
        Destroy(start.tile);
        return;
    }

    public void DeleteSeperatedIslands()
    {
        
        List<Tile> inspectTiles = new List<Tile>();
        List<Tile> neighbors = new List<Tile>();

        inspectTiles.Add(startingTile);

        while(inspectTiles.Count > 0)
        {
            if (inspectTiles[0].typ == TileTyp.Water || inspectTiles[0].typ == TileTyp.Island)
            {
                islandTilesList.Add(inspectTiles[0]);
                inspectTiles.Remove(inspectTiles[0]);
                continue;
            }

            neighbors = GetNeighbors(inspectTiles[0].posX, inspectTiles[0].posY);

            foreach (Tile item in neighbors)
            {
                if(item.typ != TileTyp.Void && !inspectTiles.Contains(item) && !islandTilesList.Contains(item))
                {
                    inspectTiles.Add(item);
                }
            }
            islandTilesList.Add(inspectTiles[0]);
            inspectTiles.Remove(inspectTiles[0]);
        }

        for (int i = 0; i < tileMap.GetLength(0); i++)
        {           
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if(tileMap[i,j].typ != TileTyp.Void)
                {
                    if (!islandTilesList.Contains(tileMap[i, j]))
                    {
                        Destroy(tileMap[i, j].tile);
                        tileMap[i, j].typ = TileTyp.Void;
                    }
                }
            }
        }

    }


    public Tile FindFarestTile(Tile start)
    {
        int distance = 0;
        Tile farestTile = start;
        foreach (Tile item in islandTilesList)
        {
            if(item.typ == TileTyp.Gras || item.typ == TileTyp.Mountain || item.typ == TileTyp.Hill)
            {
                int tmp = Mathf.Abs(start.posX - item.posX) + Mathf.Abs(start.posY - item.posY);
                if (tmp > distance)
                {
                    farestTile = item;
                    distance = tmp;
                }
            }            
        }
        return tileMap[farestTile.posX, farestTile.posY];
    }

    public void PlaceDungeon()
    {
        Tile farest = FindFarestTile(startingTile);
        Vector3 pos = tileMap[farest.posX, farest.posY].tile.transform.position;
        GameObject dungeon = Instantiate(dungeonTiles[Random.Range(0,dungeonTiles.Length)], pos, Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);

        Destroy(tileMap[farest.posX, farest.posY].tile);
        tileMap[farest.posX, farest.posY] = new Tile(dungeon, TileTyp.Special, farest.posX, farest.posY, farest.distance);
        finishTile = tileMap[farest.posX, farest.posY];
        Player.player.finishTile = finishTile;
    }

    public Tile GetFinishTile()
    {
        return finishTile;
    }

    public Tile GetStartingTile()
    {
        return startingTile;
    }

    bool tmp = true;
    public void TestingNeighbors()
    {       
        if (tmp)
        {
            tmp = false;
            tileHeightMap = mapGenerator.GetNoiseMap(Random.Range(0, 20));
            tileMap = new Tile[tileHeightMap.GetLength(0), tileHeightMap.GetLength(1)];
            
            float offset = 0;
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                if (i % 2 == 0) offset = 0;
                else offset = 2.5f;

                for (int j = 0; j < tileMap.GetLength(1); j++)
                {
                
                    tileMap[i, j].tile = Instantiate(grasTiles[Random.Range(0, grasTiles.Length)], new Vector3(i * 4.33f, 0, (j * 5f) + offset), Quaternion.Euler(0, Random.Range(0, 6) * 60, 0), levelParent.transform);
                    tileMap[i, j].typ = TileTyp.Gras;                    
                    tileMap[i, j].posX = i;
                    tileMap[i, j].posY = j;                
                }
            }
        }else if (deltTest)
        {
            if (Xp % 2 == 0)
            {
                if (Xp + 1 < tileMap.GetLength(0) && Yp - 1 >= 0)
                {
                    Destroy(tileMap[Xp + 1, Yp - 1].tile);
                }

                if (Xp - 1 >= 0 && Yp - 1 >= 0)
                {
                    Destroy(tileMap[Xp - 1, Yp - 1].tile);
                }
            }
            else
            {                
                if (Xp - 1 >= 0 && Yp + 1 < tileMap.GetLength(1))
                {
                    Destroy(tileMap[Xp - 1, Yp + 1].tile);
                }
                if (Xp + 1 < tileMap.GetLength(0) && Yp + 1 < tileMap.GetLength(1))
                {
                    Destroy(tileMap[Xp + 1, Yp + 1].tile);
                }                
            }

            if (Xp - 1 >= 0)
            {
                Destroy(tileMap[Xp - 1, Yp].tile);
            }
            if (Xp + 1 < tileMap.GetLength(0))
            {
                Destroy(tileMap[Xp + 1, Yp].tile);
            }
            if (Yp + 1 < tileMap.GetLength(1))
            {
                Destroy(tileMap[Xp, Yp + 1].tile);
            }
            if (Yp - 1 >= 0)
            {
                Destroy(tileMap[Xp, Yp - 1].tile);
            }
        }
    }

    public void PlaceItems()
    {
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if(tileMap[i,j].typ == TileTyp.Gras)
                {
                    if(Random.value > 0.1f)
                    {
                        Vector3 pos = tileMap[i, j].tile.transform.position;
                        Vector2 rndPos = Random.insideUnitCircle * 1.5f;
                        Vector3 placePos = pos + new Vector3(rndPos.x, 0, rndPos.y);
                        GameObject placeHolder = Instantiate(placeHolderProp, placePos, Quaternion.Euler(0, Random.Range(-180, 180), 0), levelParent.transform);
                        if (placeHolder.GetComponent<PlaceHolder>())
                        {
                            if (placeHolder.GetComponent<PlaceHolder>().GetCollisionState()) Debug.Log("Item Placed Wrong");
                            else
                            {
                                GameObject chest = Instantiate(itemProps[Random.Range(0, itemProps.Length)], placePos, placeHolder.transform.rotation, levelParent.transform);
                                chest.transform.localScale *= 0.4f;
                                Destroy(placeHolder);
                            }
                        }                                               
                    }
                }
            }
        }
    }


    public void PlaceEnemies()
    {
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (tileMap[i, j].typ == TileTyp.Gras)
                {
                    int rndEnemiesOnTile = Random.Range(0, 5);
                    for (int n = 0; n < rndEnemiesOnTile; n++)
                    {
                        Vector3 pos = tileMap[i, j].tile.transform.position;
                        Vector2 rndPos = Random.insideUnitCircle * 7.5f;
                        Vector3 placePos = pos + new Vector3(rndPos.x, 0, rndPos.y);

                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(placePos, out hit, 1.0f, NavMesh.AllAreas))
                        {
                            GameObject enemy = Instantiate(enemyProps[Random.Range(0, enemyProps.Length)], hit.position, Quaternion.identity, enemiesParent.transform);
                            enemy.transform.localScale *= Random.Range(0.5f,1f);
                        }
                    }
                }
            }
        }
    }

    public struct Tile
    {
        public GameObject tile;
        public TileTyp typ;
        public int posX;
        public int posY;
        public int distance;

        public Tile(GameObject t, TileTyp type, int x, int y, int n)
        {
            tile = t;
            typ = type;
            posX = x;
            posY = y;
            distance = n;
        }

        public void SetDistance(int n)
        {
            distance = n;
        }
    }
}
