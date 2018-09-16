using UnityEngine;
using UnityEngine.UI;

public class Floor : MonoBehaviour
{

    // UI
    [SerializeField]
    private Button _unlockBtn;
    [SerializeField]
    private Button _upgradeBtn;

    private GameManager _gm;
    private LevelManager _levelManager;
    private Manager _managerInstance;
    [SerializeField]
    private bool _isUnlocked = false;
    private uint _floorPrice = 30;
    private uint _upgradePrice = 30;
    private int _maxLevel = 10;
    private int _minerLevel = 1;
    public int _floorLevel;

    public delegate void LevelUp(int floorLevel);
    public static event LevelUp OnLevelUp;

    private void Start()
    {
        _levelManager = LevelManager._instance;
        _gm = GameManager._instance;

        InitializeFloor();
    }

    private void LateUpdate()
    {
        ClampUI();
    }

    // if the floor is already unlocked (like the first static floor in the scene) than skip the "Unlocked state"
    private void InitializeFloor()
    {
        if (_isUnlocked)
        {
            CreateUpgradeButton();
            transform.Find("ManagerSpawn").GetComponent<ManagerSpawn>().CreateBuyManagerButton(this);
        }
        else
        {
            uint carryLimitPreviousWorker = (uint)(_minerLevel + (_floorLevel - 1) * 5) * 10;
            _floorPrice = carryLimitPreviousWorker * 15;
            _upgradePrice = (uint)(_minerLevel + ((_floorLevel) * 5)) * 10 * 3;
            CreateUnlockButton();
        }
    }

    // Here we create the Unlock button whichs unlocks the floor 
    private void CreateUnlockButton()
    {
        _unlockBtn = Instantiate(_unlockBtn, GameObject.FindGameObjectWithTag("Canvas").transform);
        _unlockBtn.onClick.AddListener(OnClickUnlockBtn);
        _unlockBtn.GetComponentInChildren<Text>().text = "Unlock Floor" + "\n" + _floorPrice;
    }

    // Here we create the Upgrade button whichs upgrades the floor 
    private void CreateUpgradeButton()
    {
        _upgradeBtn = Instantiate(_upgradeBtn, GameObject.FindGameObjectWithTag("Canvas").transform);
        _upgradeBtn.onClick.AddListener(OnClickUpgradeBtn);
        _upgradeBtn.GetComponentInChildren<Text>().text = "Level " + _minerLevel + "\n" + _upgradePrice;
    }

    //Here we clamp ui elements to the floor and the ores
    private void ClampUI()
    {
        if (_unlockBtn != null)
        {
            Vector3 floorScreenPos = Camera.main.WorldToScreenPoint(transform.position);
            _unlockBtn.transform.position = floorScreenPos;
        }

        Vector3 oresScreenPos = Camera.main.WorldToScreenPoint(transform.Find("Ores").position);
        _upgradeBtn.transform.position = new Vector3(oresScreenPos.x + 150, oresScreenPos.y);
    }

    // This unlocks the floor and creates the Upgrade- and the Buy Manager Button
    public void OnClickUnlockBtn()
    {
        if (_gm.Buy(_floorPrice))
        {
            Destroy(_unlockBtn.gameObject);
            Vector3 groundPosition = transform.Find("Ground").position;
            transform.Find("Ground").position = new Vector3(groundPosition.x, groundPosition.y, 0.0f);
            _levelManager.SpawnNextFloor(transform.position.y);

            CreateUpgradeButton();
            transform.Find("ManagerSpawn").GetComponent<ManagerSpawn>().CreateBuyManagerButton(this);
        }
    }

    // This fires the OnLevelUp Event which the workers get notified
    public void OnClickUpgradeBtn()
    {
        if (_gm.Buy(_upgradePrice))
        {
            if (_minerLevel < _maxLevel)
            {
                _minerLevel++;
                _upgradePrice = _upgradePrice * 2;
                _upgradeBtn.GetComponentInChildren<Text>().text = "Level " + _minerLevel + "\n" + _upgradePrice;
                OnLevelUp(_floorLevel);
            }
        }

    }

    // Upgrade/Reset the Upgrade price
    public void UpdateUpgradePrice(int factor, bool shouldIncrease)
    {
        _upgradePrice = shouldIncrease ? _upgradePrice * (uint)factor : _upgradePrice / (uint)factor;
        _upgradeBtn.GetComponentInChildren<Text>().text = "Level " + _minerLevel + "\n" + _upgradePrice;
    }
}
