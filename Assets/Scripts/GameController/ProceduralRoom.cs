using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralRoom : MonoBehaviour
{


    int laborRooms = 5;
    int chestRooms = 2;
    public GameObject Floor;
    public GameObject Cross;

    [SerializeField]
    private List<Vector2> roomPositions = new List<Vector2>();
    List<Vector2> floors;

    [SerializeField]
    private Dictionary<Vector2, GameObject> roomMap = new Dictionary<Vector2,GameObject>();

    public enum Rooms
    {
        Labor, Dungeon, Floor, Boss, Portal, Chest
    }


    // Start is called before the first frame update
    void Start()
    {
        StartingRoom();
        CreateNewRooms();
        FillFloors();
    }

    public void StartingRoom()
    {
        GameObject StartingCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        StartingCube.transform.position = new Vector3(0, 0, 0);
        StartingCube.GetComponent<Renderer>().material.color = Color.black;
        roomMap.Add(new Vector2(0, 0), StartingCube);
    }

    public void CreateNewRooms()
    {
        PortalRoom();
        for (int i = 0; i < chestRooms; i++)
        {
            ChestRoom();
        }
        for (int i = 0; i < laborRooms; i++)
        {
            LaborRoom();
        }
    }

    public void FillFloors()
    {
        floors = new List<Vector2>(roomPositions);

        foreach(Vector2 room in roomPositions)
        {
            int tmp = Random.Range(0, 2);
            if(tmp == 0)
            {
                int x = (int)room.x;
                while (x != 0)
                {
                    if (x > 0) x--;
                    else if (x < 0) x++;
                    CreateFloor(x, (int)room.y,0);
                }

                int y = (int)room.y;
                while (y != 0)
                {
                    if (y > 1) y--;
                    else if (y < 1) y++;
                    else
                    {
                        y = 0;
                        continue;
                    }
                    CreateFloor(0, y);
                }
            }
            else
            {
                int y = (int)room.y;
                while (y != 0)
                {
                    if (y > 0) y--;
                    else if (y < 0) y++;
                    CreateFloor((int)room.x, y);
                }

                int x = (int)room.x;
                while (x != 0)
                {
                    if (x > 1) x--;
                    else if (x < 1) x++;
                    else
                    {
                        x = 0;
                        continue;
                    }
                    CreateFloor(x, 0,0);
                }                
            }
        }
    }

    private void CreateFloor(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);
        if (roomPositions.Contains(pos)) return;
        if (floors.Contains(pos))
        {
            if (pos != new Vector2(0, 0))
            {
                Destroy(roomMap[pos]);
                GameObject Room2 = Instantiate(Cross);
                Room2.transform.position = new Vector3(pos.x, 0, pos.y);
                roomMap.Remove(pos);
                roomMap.Add(pos, Room2);
            }
            return;
        }
        else floors.Add(pos);
        //GameObject Room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Room = Instantiate(Floor);
        Room.transform.position = new Vector3(pos.x, 0, pos.y);
        //Room.transform.localScale *= 0.5f;
        if(!roomMap.ContainsKey(pos)) roomMap.Add(pos, Room);
    }

    private void CreateFloor(int x, int y, int direction)
    {
        Vector2 pos = new Vector2(x, y);
        if (roomPositions.Contains(pos)) return;
        if (floors.Contains(pos))
        {
            //if(pos != new Vector2(0,0)) roomMap[pos].GetComponent<Renderer>().material.color = Color.red;
            return;
        }
        else floors.Add(pos);
        //GameObject Room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Room = Instantiate(Floor);
        Room.transform.position = new Vector3(pos.x, 0, pos.y);
        //Room.transform.localScale *= 0.5f;
        if (direction == 0) Room.transform.rotation = Quaternion.Euler(0, 90, 0);
        if (!roomMap.ContainsKey(pos)) roomMap.Add(pos, Room);
    }

    private void LaborRoom()
    {
        Vector2 pos = RoomPosition();
        if (roomPositions.Contains(pos))
        {
            LaborRoom();
            return;
        }
        else roomPositions.Add(pos);
        GameObject Room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Room.transform.position = new Vector3(pos.x, 0, pos.y);
        Room.GetComponent<Renderer>().material.color = Color.green;
        roomMap.Add(pos, Room);
    }

    private void ChestRoom()
    {
        Vector2 pos = RoomPosition();
        if (roomPositions.Contains(pos))
        {
            ChestRoom();
            return;
        }
        else roomPositions.Add(pos);
        GameObject Room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Room.transform.position = new Vector3(pos.x, 0, pos.y);
        Room.GetComponent<Renderer>().material.color = Color.yellow;
        roomMap.Add(pos, Room);
    }

    private void PortalRoom()
    {
        int posX = Random.Range(-4, 5);
        int posY = Random.Range(-4, 5);
        if (posX > 0) posX += 5;
        else if(posX < 0) posX -= 5;

        if (posY > 0) posY += 3;
        else if(posY < 0) posY -= 3;

        if (posY == 0 && posX == 0)
        {
            PortalRoom();
            return;
        }
        Vector2 pos = new Vector2(posX, posY);
        if (roomPositions.Contains(pos))
        {
            PortalRoom();
            return;
        }
        else roomPositions.Add(pos);
        GameObject Room = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Room.transform.position = new Vector3(pos.x, 0, pos.y);
        Room.GetComponent<Renderer>().material.color = Color.blue;
        roomMap.Add(pos, Room);
    }


    public Vector2 RoomPosition()
    {
        int posX = Random.Range(-6, 7);
        int posY = Random.Range(-6, 7);
        if (posX > 0) posX += 3;
        else if (posX < 0) posX -= 3;

        if (posY > 0) posY += 1;
        else if (posY < 0) posY -= 1;
        if (posY == 0 && posX == 0) return RoomPosition();
        else return new Vector2(posX, posY);
    }


}
