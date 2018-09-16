using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{

    //UI
    [SerializeField]
    private Text _totalMoneyText;

    public static GameManager _instance;
    private uint _totalMoney = 0;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        Load();
    }

    private void OnDestroy()
    {
        Save();
    }

    public void AddMoney(uint money)
    {
        _totalMoney += money;
        _totalMoneyText.text = _totalMoney + "";
    }

    // This checks if totalMoney is enough to buy things like a manager or an upgrade and returns true if the payment succeeded
    public bool Buy(uint money)
    {
        if (_totalMoney >= money)
        {
            _totalMoney -= money;
            _totalMoneyText.text = _totalMoney + "";
            return true;
        }
        return false;
    }

    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData data = new PlayerData();
        data.totalMoney = _totalMoney;

        bf.Serialize(file, data);
        file.Close();
    }

    private void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            _totalMoney = data.totalMoney;
            _totalMoneyText.text = _totalMoney + "";
        }
    }

    [Serializable]
    class PlayerData
    {
        public uint totalMoney;
    }

}
