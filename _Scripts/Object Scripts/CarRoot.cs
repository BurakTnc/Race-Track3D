using DG.Tweening;
using UnityEngine;
using Dreamteck.Splines;

[RequireComponent(typeof(AudioSource))]
public class CarRoot : MonoBehaviour
{
    public int CarLevel;

    [SerializeField] private GameObject[] _cars;
    [SerializeField] private GameObject _mergeAnalizer;
    [SerializeField] private AudioClip checkPointSound, mergeSound;

    private TrailRenderer _trail;
    private float speedUpRate;
    private float followSpeed;
    private SplineFollower _splineFollower;
    private MainCarSpecs mainCarSpecs;
    private Vector3 dragOffset;
    private Vector3 firstPosition;
    private bool isMergable = false;
    private bool isDragAllowed = false;
    private AudioSource source;

    #region Debug
    private bool onDebugMode = false;
    #endregion



    private void Awake()
    {
        _splineFollower = GetComponent<SplineFollower>();
        mainCarSpecs = Resources.Load<MainCarSpecs>(path: "Datas/Main Data");
        _trail = GetComponent<TrailRenderer>();
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Start()
    {
        Initialize();
    }

    #region Debug
    private void Update()
    {
        if (onDebugMode)
        {
            followSpeed = DebugManager.instance._carSpeed.value;
            _splineFollower.followSpeed = followSpeed;
        }
    }

    private void DebugMode(bool mode)
    {
        onDebugMode = mode;
        if (!onDebugMode)
        {
            followSpeed = mainCarSpecs.FollowSpeed;
            _splineFollower.followSpeed = followSpeed;
        }
    }
    #endregion

    private void Initialize()
    {
        followSpeed = mainCarSpecs.FollowSpeed;
        speedUpRate = mainCarSpecs.SpeedUpRate;
        _splineFollower.followSpeed = followSpeed;

        ChangeSpline(GameManager.instance.GetTrackLevel());
        SetCarLevel(CarLevel);
        StopSpeeding();
    }

    private void Subscribe()
    {
        CoreGameSignals.instance.OnSpeeding += BeginSpeedUp;
        CoreGameSignals.instance.OnSplineChange += ChangeSpline;
        CoreGameSignals.instance.OnSlowing += StopSpeeding;
        CoreGameSignals.instance.BeginCarMerge += StopCar;
        CoreGameSignals.instance.StopCarMerge += BeginContinue;
        CoreGameSignals.instance.OnDebugMode += DebugMode;
    }

    private void UnSubscribe()
    {
        CoreGameSignals.instance.OnSpeeding -= BeginSpeedUp;
        CoreGameSignals.instance.OnSplineChange -= ChangeSpline;
        CoreGameSignals.instance.OnSlowing -= StopSpeeding;
        CoreGameSignals.instance.BeginCarMerge -= StopCar;
        CoreGameSignals.instance.StopCarMerge -= BeginContinue;
        CoreGameSignals.instance.OnDebugMode -= DebugMode;
    }

    #region Public Functions
    public void SetCarLevel(int lvl)
    {
        if (lvl<_cars.Length)
        {
            CarLevel = lvl;

            for (int i = 0; i < _cars.Length; i++)
            {
                if (i == lvl)
                {
                    _cars[i].SetActive(true);
                }
                else
                {
                    _cars[i].SetActive(false);
                }
            }
        }
        switch (CarLevel)
        {
            case 0:
                _mergeAnalizer.tag = "LV1";
                break;
            case 1:
                _mergeAnalizer.tag = "LV2";
                break;
            case 2:
                _mergeAnalizer.tag = "LV3";
                break;
            case 3:
                _mergeAnalizer.tag = "LV4";
                break;

            default:
                break;
        }
    }

    public void StartMove()
    {
        _splineFollower.enabled = true;
        _splineFollower.Restart(0);
        _splineFollower.follow = true;

    }
    public void ResetPath()
    {
        _splineFollower.Restart(0);
    }

    public void UpgradeAnimation()
    {
        transform.DOScale(Vector3.one*2, .4f).SetLoops(2, LoopType.Yoyo);
    }
    #endregion

    private void PlayAudio(AudioClip clip)
    {
        source.clip = clip;
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.Play();
    }

    private void StopCar()
    {
        isDragAllowed = true;
        isMergable = true;
        transform.tag = "Car";
        _splineFollower.follow = false;
    }

    private void BeginContinue()
    {
        transform.tag = "Car";
        isDragAllowed = false;
        isMergable = false;
        Invoke("ContinueFollow", 1);
    }

    private void ContinueFollow()
    {
        _splineFollower.enabled = true;
        _splineFollower.follow = true;
        isMergable = false;
    }

    private void SetSplineBack()
    {
        _splineFollower.enabled = true;
    }

    private void BeginSpeedUp()
    {
        _splineFollower.followSpeed = followSpeed * speedUpRate;
        _trail.emitting = true;
    }

    private void StopSpeeding()
    {
        _splineFollower.followSpeed = followSpeed / speedUpRate;
        _trail.emitting = false;
    }

    private void ChangeSpline(int splineLevel)
    {
        _splineFollower.spline = GameManager.instance.Splines[splineLevel].GetComponent<SplineComputer>();
    }

    private void CompareLevels(CarRoot script)
    {
        if (script.CarLevel==CarLevel)
        {
            script.isMergable = true;
            MergeCars(script);
        }
    }
    private void Vibrate()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HapticManager.instance.SuccessHaptic();
        }
        else
        {
            HapticManager.instance.RigidHaptic();
        }
    }

    private void MergeCars(CarRoot car)
    {
        if (isMergable)
        {
            Vibrate();
            car.PlayAudio(mergeSound);
            car.transform.tag = "Untagged";
            car.SetCarLevel(CarLevel + 1);
            car.UpgradeAnimation();
            CoreGameSignals.instance.StopCarMerge?.Invoke();
            GameManager.instance.RemoveCarFromList(this);
            CoreGameSignals.instance.OnSaveVariables?.Invoke();
            Destroy(gameObject);
        }        
    }

    #region Drag & Drop
    private void OnMouseDown()
    {
        if (transform.tag=="Car" && isDragAllowed)
        {
            firstPosition = transform.position;
            _splineFollower.enabled = false;
            dragOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, 0, transform.position.z));
        }
    }

    private void OnMouseUp()
    {
        if (transform.tag=="Car" && isDragAllowed)
        {
            transform.DOMove(firstPosition, .3f).SetEase(Ease.OutBack).OnComplete(() => SetSplineBack());
        }
    }

    private void OnMouseDrag()
    {
        if (transform.tag == "Car" && isDragAllowed)
        {
            Vector3 desiredPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - dragOffset);
            transform.position = new Vector3(desiredPos.x, transform.position.y, desiredPos.y + desiredPos.z);
        }
    }
    #endregion

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Car":
                if (isMergable)
                {
                    CompareLevels(other.GetComponent<CarRoot>());
                }                
                break;

            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            PlayAudio(checkPointSound);
        }
    }
}
