using UnityEngine;
using UnityEngine.UI;

public class MoneyHolder : MonoBehaviour
{

    private uint _moneyStored = 0;
    [SerializeField]
    private Text _moneyCount;
    [SerializeField]
    private Vector3 _textOffset;

    private void Awake()
    {
        _moneyCount = Instantiate(_moneyCount, GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    private void LateUpdate()
    {
        ClampUI();
    }

    //Here we clamp ui elements to the moneyHolder 
    private void ClampUI()
    {
        Vector3 treasureScreenPos = Camera.main.WorldToScreenPoint(transform.position + _textOffset);
        _moneyCount.transform.position = treasureScreenPos;
    }

    public void AddMoney(uint money)
    {
        _moneyStored += money;
        _moneyCount.text = "" + _moneyStored;
    }

    public uint RemoveMoney()
    {
        uint moneyToReturn = _moneyStored;
        _moneyStored = 0;
        _moneyCount.text = "" + _moneyStored;
        return moneyToReturn;
    }
}
