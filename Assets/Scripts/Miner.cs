using UnityEngine;

public class Miner : Worker
{
    [SerializeField]
    private MoneyHolder _treasure;
    private int _level = 1;
    private uint moneyHolding = 0;

    public override void Start()
    {
        // Before base.Start() since base.Start() sets the parent of this object to an AnimationHelper
        _group = GetComponentInParent<Floor>()._floorLevel;
        base.Start();
        _carryLimit = (uint)(_level + (_group * 5)) * 10;
    }

    override public void OnEnable()
    {
        base.OnEnable();
        Floor.OnLevelUp += LevelUp;
    }

    override public void OnDisable()
    {
        base.OnDisable();
        Floor.OnLevelUp -= LevelUp;
    }

    // The miner here stores coins into the treasure
    public override void StoreCoins()
    {
        _isStoring = true;
        _isReady = false;
        _treasure.AddMoney(moneyHolding);
        moneyHolding = 0;
        _moneyCount.text = "0";
        FinishedWorking();
    }

    // The miner here starts the working progressBar
    public override void Work(int floor)
    {
        _isStoring = false;
        _isReady = false;
        _progressBar.value = 0;
        _progressBar.gameObject.SetActive(true);
        _moneyCount.gameObject.SetActive(false);
        _startProgressBar = true;
        _moneyCount.text = "" + _carryLimit;
        moneyHolding += _carryLimit;
    }

    // This will get called whenever the UpgradeBtn get's pressed
    private void LevelUp(int floorLevel)
    {
        if (floorLevel == _group)
        {
            _level++;
            _carryLimit = (uint)(_level + (_group * 5)) * 10;
        }
    }
}
