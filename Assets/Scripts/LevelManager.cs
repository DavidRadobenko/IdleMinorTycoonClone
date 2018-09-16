using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _floor;
    private int _floorCount;

    public static LevelManager _instance = null;
    public List<Floor> _floors;

    public delegate void FloorAdded(int floorCount);
    public static event FloorAdded OnFloorAdded;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeFloors();
    }

    // For the correct index order the LevelManager adds the floor on floorLevel 0 and t
    private void InitializeFloors()
    {
        _floors = new List<Floor>();
        Floor[] floorsFound = FindObjectsOfType<Floor>();
        for (int i = 0; i < floorsFound.Length; i++)
        {
            if (floorsFound[i]._floorLevel == 0)
            {
                _floors.Add(floorsFound[i]);
                break;
            }
        }
        for (int i = 0; i < floorsFound.Length; i++)
        {
            if (floorsFound[i]._floorLevel != 0)
            {
                _floors.Add(floorsFound[i]);
            }
        }
        _floorCount = _floors.Count;
    }

    /*
     * Spawns the next floor 2 units under the givin yPosition
     */
    public void SpawnNextFloor(float yPosition)
    {
        GameObject floorInstance = Instantiate(_floor, new Vector3(0, yPosition - 2.0f), Quaternion.identity);
        _floors.Add(floorInstance.GetComponent<Floor>());

        floorInstance.GetComponent<Floor>()._floorLevel = _floorCount;
        _floorCount++;

        OnFloorAdded(_floorCount);
    }

    public uint GetMoneyFromFloor(int index)
    {
        uint removedMoney = _floors[index].gameObject.transform.Find("Treasure").GetComponent<MoneyHolder>().RemoveMoney();
        return removedMoney;
    }
}
