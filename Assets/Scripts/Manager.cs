using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    public enum PowerUpType
    {
        SpeedUp,
        WorkSpeedUp,
        PriceLower
    }

    //UI
    [SerializeField]
    private Button _powerUpBtn;
    [SerializeField]
    private Text _timer;
    [SerializeField]
    private Vector3 _powerUpBtnPosition;

    public int _group;
    private int _factor;
    private bool _timerStarted;
    private float _timeLeft;
    private PowerUpType _type;
    private Color _originalColor;

    private const int DELIVERER_GROUP = -2;
    private const int ELEVATOR_GROUP = -1;

    public delegate void PowerUp(PowerUpType type, int factor, int group);
    public static event PowerUp OnPowerUp;
    public static event PowerUp OnPowerUpExpired;

    private void Start()
    {
        CalculateRandomType();

        _timeLeft = _factor;
        _originalColor = GetComponent<Renderer>().material.GetColor("_Color");

        CreatePowerUpButton();
        CreateTimerText();
    }

    private void Update()
    {
        // Start the timer when timerStarted is true
        if (_timerStarted)
        {
            _timeLeft -= Time.deltaTime;
            _timer.text = _timeLeft.ToString("0.0");
            if (_timeLeft <= 0)
            {
                if (GetComponent<Renderer>().material.GetColor("_Color") == Color.grey)
                {
                    PowerUpIsReady();
                }
                else
                    TimerFinished();
            }
        }
    }

    private void LateUpdate()
    {
        ClampUI();
    }

    private void OnEnable()
    {
        Worker.OnFinishedStoring += CheckWorker;
    }

    private void OnDisable()
    {
        Worker.OnFinishedStoring -= CheckWorker;
    }

    private void CalculateRandomType()
    {
        int randomType;
        // Deliverer and ElevatorOperator don't have a level to lower the upgrade price
        if (_group <= ELEVATOR_GROUP)
            randomType = Random.Range(0, 2);
        else
            randomType = Random.Range(0, 3);

        switch (randomType)
        {
            case 0:
                _type = PowerUpType.SpeedUp;
                break;
            case 1:
                _type = PowerUpType.WorkSpeedUp;
                break;
            case 2:
                _type = PowerUpType.PriceLower;
                break;
        }

        int randomRarity = Random.Range(0, 3);
        switch (randomRarity)
        {
            case 0:
                _factor = 3;
                break;
            case 1:
                _factor = 5;
                break;
            case 2:
                _factor = 7;
                break;
        }
    }

    // A Timer starts when the button is pressed that when ran out will reset the Power Up properties of the worker
    private void OnPowerUpBtnClicked()
    {
        _powerUpBtn.gameObject.SetActive(false);
        _timer.gameObject.SetActive(true);
        OnPowerUp(_type, _factor, _group);

        _timeLeft = _factor;
        _timerStarted = true;
    }

    // This sets another timer and fires the OnPowerUpExpired event
    private void TimerFinished()
    {
        _timeLeft = _factor * 2;
        GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
        OnPowerUpExpired(_type, _factor, _group);
    }

    // When the Manager is refreshed, the PowerUpBtn will be visible again
    private void PowerUpIsReady()
    {
        GetComponent<Renderer>().material.SetColor("_Color", _originalColor);
        _timer.gameObject.SetActive(false);
        _powerUpBtn.gameObject.SetActive(true);
        _timerStarted = false;
    }

    // If the worker belongs to this Manager, set IsReady to true so he starts working again
    private void CheckWorker(Worker worker)
    {
        if (_group == worker._group)
            worker.IsReady = true;
    }

    //Here we clamp ui elements to the Manager
    private void ClampUI()
    {
        Vector3 managerPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1f, 0));
        _powerUpBtn.transform.position = managerPos;
        _timer.transform.position = managerPos;
    }

    //Here we create the PowerUp button whichs power ups the worker
    private void CreatePowerUpButton()
    {
        _powerUpBtn = Instantiate(_powerUpBtn, GameObject.FindGameObjectWithTag("Canvas").transform);
        _powerUpBtn.onClick.AddListener(OnPowerUpBtnClicked);
        _powerUpBtn.GetComponentInChildren<Text>().text = _type.ToString() + "\nx" + _factor;
    }

    // Here we create the timer text that will display when the timer has started
    private void CreateTimerText()
    {
        _timer = Instantiate(_timer, GameObject.FindGameObjectWithTag("Canvas").transform);
        _timer.gameObject.SetActive(false);
    }
}
