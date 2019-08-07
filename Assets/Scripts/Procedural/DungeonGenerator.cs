using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject layoutParent;
    public GameObject tileParent;

    public GameObject [] levelLayouts;
    public GameObject [] tiles;
    public int seed;

    // Start is called before the first frame update
    void Start()
    {
        //Random.InitState(seed);
        //int counter = 0;
        //foreach (Transform item in layoutParent.transform)
        //{
        //    if (item.transform == layoutParent.transform) continue;
        //    levelLayouts[counter] = item.gameObject;
        //    counter++;
        //}
        //counter = 0;
        //foreach (Transform item in tileParent.GetComponentsInChildren<Transform>())
        //{
        //    if (item.transform == tileParent.transform) continue;
        //    tiles[counter] = item.gameObject;
        //}

        int layoutRand = Random.Range(0, layoutParent.transform.childCount);
        GameObject level = Instantiate(layoutParent.transform.GetChild(layoutRand).gameObject, new Vector3(0, 0, 0), Quaternion.identity);

        foreach (Transform tile in level.transform)
        {
            if (tile.transform == level.transform) continue;
            int randTile = Random.Range(0, tileParent.transform.childCount);
            GameObject newTile = Instantiate(tileParent.transform.GetChild(randTile).gameObject, tile.transform.position, tile.rotation);
            Destroy(tile.gameObject);
        }

        layoutParent.SetActive(false);
        tileParent.SetActive(false);
    }


}
