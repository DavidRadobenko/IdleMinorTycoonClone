using System.Collections.Generic;
using UnityEngine;

public class ElevatorOperator : Worker
{
    //UI
    [SerializeField]
    private MoneyHolder _moneyHouse;

    //Animation
    private AnimationClip _elevateClip;
    private AnimationEvent _storeCoins;
    private List<AnimationEvent> _workEvents;

    private LevelManager _levelManager;
    private uint _moneyStored;
    private int _currentFloor;

    override public void Start()
    {
        base.Start();
        _levelManager = LevelManager._instance;
        _elevateClip = _charAnimator.runtimeAnimatorController.animationClips[0];
        _workEvents = new List<AnimationEvent>();
        CreateElevateAnim();
    }

    override public void OnEnable()
    {
        base.OnEnable();
        LevelManager.OnFloorAdded += ExpandAnimation;
    }

    override public void OnDisable()
    {
        base.OnDisable();
        LevelManager.OnFloorAdded -= ExpandAnimation;
    }

    // The collected coins will get stored into the moneyHolder House
    public override void StoreCoins()
    {
        _isStoring = true;
        _isReady = false;
        _progressBar.value = 0;
        _progressBar.gameObject.SetActive(true);
        _moneyCount.gameObject.SetActive(false);
        _startProgressBar = true;
    }

    // The Operator will visit every floor and store the money from the treasures
    public override void Work(int floor)
    {
        _isStoring = false;
        _isReady = false;
        _progressBar.value = 0;
        _progressBar.gameObject.SetActive(true);
        _moneyCount.gameObject.SetActive(false);
        _startProgressBar = true;
        _currentFloor = floor;
    }

    // When a new Floor get's spawned, the Operator need to adjust his animation and create new events
    private void ExpandAnimation(int floorCount)
    {
        floorCount -= 2; // we only need the unlocked floors - the first floor
        float firstFloorKeyframeTime = 0.3f / 2.3f;

        AnimationCurve curve = AnimationCurve.Constant(0, floorCount + firstFloorKeyframeTime, 0);

        Keyframe floorKeyframe = new Keyframe(floorCount, floorCount * -2.0f - 0.3f);
        curve.AddKey(floorKeyframe);

        curve.RemoveKey(curve.keys.Length - 1);
        Keyframe endKeyframe = new Keyframe(floorCount * 2, 0);
        curve.AddKey(endKeyframe);

        _elevateClip.SetCurve("", typeof(Transform), "localPosition.y", curve);

        AnimationEvent work = new AnimationEvent();
        work.functionName = "Work";
        work.time = floorKeyframe.time;
        work.intParameter = floorCount;
        _elevateClip.AddEvent(work);
        _workEvents.Add(work);

        RemoveEventFromAnimation("StoreCoins");
        _storeCoins.time = floorCount * 2;
        _elevateClip.AddEvent(_storeCoins);
    }

    // When the Animation get's manipulated, it need's to reset when the game restarts and here it recreate the beginning
    private void CreateElevateAnim()
    {
        float firstFloorKeyframeTime = 0.3f / 2.3f;

        _elevateClip.ClearCurves();
        AnimationCurve curve = AnimationCurve.Constant(0, firstFloorKeyframeTime * 2, 0);

        Keyframe firstKeyframe = new Keyframe(0, 0);
        Keyframe firstFloorKeyframe = new Keyframe(firstFloorKeyframeTime, -0.3f);
        curve.AddKey(firstKeyframe);
        curve.AddKey(firstFloorKeyframe);

        _elevateClip.SetCurve("", typeof(Transform), "localPosition.y", curve);

        AnimationEvent firstWork = new AnimationEvent();
        firstWork.functionName = "Work";
        firstWork.time = firstFloorKeyframeTime;
        firstWork.intParameter = 0;
        _elevateClip.AddEvent(firstWork);
        _workEvents.Add(firstWork);

        _storeCoins = new AnimationEvent();
        _storeCoins.functionName = "StoreCoins";
        _storeCoins.time = firstFloorKeyframeTime * 2;
        _elevateClip.AddEvent(_storeCoins);
    }

    //Helper function to remove an Event from the Animation this GameObject is attached to
    private void RemoveEventFromAnimation(string functionName)
    {
        AnimationEvent[] animationEvents = _elevateClip.events;
        List<AnimationEvent> updatedAnimationEvents = new List<AnimationEvent>();

        for (int i = 0; i < animationEvents.Length; i++)
        {
            AnimationEvent animationEvent = animationEvents[i];
            if (animationEvent.functionName != functionName)
            {
                updatedAnimationEvents.Add(animationEvent);
            }
        }

        _elevateClip.events = updatedAnimationEvents.ToArray();
    }

    public override void FinishedWorking()
    {
        if (_isStoring)
        {
            _isReady = false;
            uint moneyToGive = _moneyStored;
            _moneyStored = 0;
            _moneyCount.text = "0";
            _moneyHouse.AddMoney(moneyToGive);
        }
        else
        {
            _moneyStored += _levelManager.GetMoneyFromFloor(_currentFloor);
            _moneyCount.text = _moneyStored + "";
        }
        base.FinishedWorking();
    }
}
