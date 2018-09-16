using UnityEngine;
using UnityEngine.UI;

abstract public class Worker : MonoBehaviour
{

    // UI
    [SerializeField]
    protected Text _moneyCount;
    [SerializeField]
    protected Slider _progressBar;
    private Transform _canvasTransform;

    // Attributes
    public int _group;
    [SerializeField]
    protected float _speed = 1.0f;
    [SerializeField]
    protected float _workSpeed = 1.0f;
    [SerializeField]
    protected uint _carryLimit = 10;
    protected bool _isReady = false;
    protected bool _startProgressBar = false;
    protected bool _isStoring;

    protected Animator _charAnimator;
    protected GameObject _animationHelper;
    protected Vector3 _workerScreenPos;

    // Animation events to override
    abstract public void Work(int floor);
    abstract public void StoreCoins();

    public delegate void FinishedStoring(Worker miner);
    public static event FinishedStoring OnFinishedStoring;

    public virtual void Start()
    {
        _canvasTransform = GameObject.FindGameObjectWithTag("Canvas").transform;
        CreateAnimHelper();
        CreateUIElements();
    }

    void Update()
    {
        // TODO this don't need to be called every frame
        HandleAnimationSpeed();
        if (_startProgressBar)
        {
            StartWorking();
        }
    }

    void LateUpdate()
    {
        ClampUI();
    }

    virtual public void OnEnable()
    {
        Manager.OnPowerUp += PowerUp;
        Manager.OnPowerUpExpired += PowerUpExpired;
    }

    virtual public void OnDisable()
    {
        Manager.OnPowerUp -= PowerUp;
        Manager.OnPowerUpExpired -= PowerUpExpired;
    }

    // When finished Working and the worker was storing money, the OnFinishedStoring event will fire
    public virtual void FinishedWorking()
    {
        //We check for null because a manager could not subscribe to the event yet
        if (_isStoring && OnFinishedStoring != null)
            OnFinishedStoring(this);
    }


    //Using animator.speed than setting on of the paremeters in the controller since setting the parameter it will reset the animation
    //even when the animation didn't finished.
    private void HandleAnimationSpeed()
    {
        _charAnimator.SetFloat("speed", _speed);
        if (_isReady)
        {
            _charAnimator.speed = 1f;
        }
        else
            _charAnimator.speed = 0f;
    }

    //Without an parent, the animation won't take the relative position of this object and instead reset the position of this object.
    //The parent holds the last position of the this object so it stays where it's position was.
    private void CreateAnimHelper()
    {
        _charAnimator = GetComponent<Animator>();
        _animationHelper = new GameObject();
        _animationHelper.transform.position = transform.position;
        _animationHelper.name = "AnimHelper";
        if (transform.parent != null)
            _animationHelper.transform.SetParent(transform.parent);
        transform.SetParent(_animationHelper.transform);
    }

    // Creates the UI elements that gets clamped every frame
    private void CreateUIElements()
    {
        _moneyCount = Instantiate(_moneyCount, _canvasTransform);

        _progressBar = Instantiate(_progressBar, _canvasTransform);
        _progressBar.maxValue = _workSpeed;
    }

    //Here we clamp UI elements to the worker 
    private void ClampUI()
    {
        _workerScreenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 0.5f, 0));
        _moneyCount.transform.position = _workerScreenPos;
        _progressBar.transform.position = _workerScreenPos;
    }

    // If the Worker isn't working at the moment set isReady to true
    private void OnMouseDown()
    {
        if (_startProgressBar == false)
        {
            _isReady = true;
        }

    }

    // This starts the working timer for the progressBar
    private void StartWorking()
    {
        _progressBar.value += Time.deltaTime * _workSpeed;
        if (_progressBar.value >= _progressBar.maxValue)
        {
            _startProgressBar = false;
            _isReady = true;
            _progressBar.gameObject.SetActive(false);
            _moneyCount.gameObject.SetActive(true);
            FinishedWorking();
        }
    }

    // This get's called whenever the event Manager.OnPowerUp fires
    // This increase the attributes of the Worker
    private void PowerUp(Manager.PowerUpType type, int factor, int group)
    {
        if (this._group == group)
        {
            switch (type)
            {
                case Manager.PowerUpType.SpeedUp:
                    _speed *= factor;
                    break;
                case Manager.PowerUpType.WorkSpeedUp:
                    _workSpeed *= factor;
                    break;
                case Manager.PowerUpType.PriceLower:
                    if (transform.root != null)
                        transform.root.GetComponent<Floor>().UpdateUpgradePrice(factor, false);
                    break;
            }
        }
    }

    // This get's called whenever the event Manager.OnPowerExpired fires
    // This resets the attributes of the Worker
    private void PowerUpExpired(Manager.PowerUpType type, int factor, int group)
    {
        if (this._group == group)
        {
            switch (type)
            {
                case Manager.PowerUpType.SpeedUp:
                    _speed /= factor;
                    break;
                case Manager.PowerUpType.WorkSpeedUp:
                    _workSpeed /= factor;
                    break;
                case Manager.PowerUpType.PriceLower:
                    GetComponentInParent<Floor>().UpdateUpgradePrice(factor, true);
                    break;
            }
        }
    }

    // Getter/Setter

    public bool IsReady
    {
        get
        {
            return _isReady;
        }

        set
        {
            _isReady = value;
        }
    }

    public uint CarryLimit
    {
        get
        {
            return _carryLimit;
        }

        set
        {
            _carryLimit = value;
        }
    }
}
