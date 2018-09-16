using UnityEngine;

public class Deliverer : Worker
{

    [SerializeField]
    private MoneyHolder _moneyHouse;
    private uint _moneyStored;

    public override void StoreCoins()
    {
        _isStoring = true;
        _isReady = false;
        _progressBar.value = 0;
        _progressBar.gameObject.SetActive(true);
        _moneyCount.gameObject.SetActive(false);
        _startProgressBar = true;
    }

    public override void Work(int floor)
    {
        _isStoring = false;
        _isReady = false;
        _progressBar.value = 0;
        _progressBar.gameObject.SetActive(true);
        _moneyCount.gameObject.SetActive(false);
        _startProgressBar = true;
    }

    // when storing, the Deliverer stores the collected money to the total money hold in the Game Manager
    public override void FinishedWorking()
    {
        if (_isStoring)
        {
            _isReady = false;
            uint moneyToGive = _moneyStored;
            _moneyStored = 0;
            _moneyCount.text = "0";
            GameManager._instance.AddMoney(moneyToGive);
        }
        else
        {
            _moneyStored = _moneyHouse.RemoveMoney();
            _moneyCount.text = "" + _moneyStored;

        }
        base.FinishedWorking();
    }
}
