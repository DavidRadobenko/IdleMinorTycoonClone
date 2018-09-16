using UnityEngine;
using UnityEngine.UI;

public class ManagerSpawn : MonoBehaviour
{

    //UI
    public Button _managerBtn;
    public Manager _managerToSpawn;

    public int _group;
    private Floor _floor;
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager._instance;
        if (_group < 0)
        {
            CreateBuyManagerButton(null);
        }
    }

    private void LateUpdate()
    {
        ClampUI();
    }

    /*
     * Here we create the Upgrade button whichs upgrades the floor 
     */
    public void CreateBuyManagerButton(Floor floor)
    {
        if (floor != null)
        {
            this._floor = floor;
            _group = floor._floorLevel;
            _managerBtn = Instantiate(_managerBtn, GameObject.FindGameObjectWithTag("Canvas").transform);
        }
        else
            _managerBtn = Instantiate(_managerBtn, GameObject.FindGameObjectWithTag("Canvas").transform);

        _managerBtn.onClick.AddListener(OnBuyManagerBtn);
    }

    private void ClampUI()
    {
        if (_managerBtn != null)
        {
            Vector3 managerPos = Camera.main.WorldToScreenPoint(transform.position);
            _managerBtn.transform.position = managerPos + new Vector3(0, 55, 0);
        }

    }

    public void OnBuyManagerBtn()
    {
        if (_gm.Buy(100))
        {
            Manager managerInstance;
            if (_floor != null)
                managerInstance = Instantiate(_managerToSpawn, transform.position, Quaternion.identity, _floor.transform);
            else
                managerInstance = Instantiate(_managerToSpawn, transform.position, Quaternion.identity);

            managerInstance._group = _group;
            _managerBtn.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
