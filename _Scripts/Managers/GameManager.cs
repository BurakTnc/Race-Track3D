using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public  GameObject[] Splines;
    public float Money;
    public float IncomeMultiplier;
    [HideInInspector]
    public List<CarRoot> carList = new List<CarRoot>();

    [SerializeField] private float _speedUpCooldown;

    private int trackLevel;
    private bool isSpeeding = false;
    private MainCarSpecs mainSpecs;
    private int carCountLimit;

    private void Awake()
    {
        if (instance!=this && instance!=null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        mainSpecs = Resources.Load<MainCarSpecs>(path: "Datas/Main Data");
    }

    private void Start()
    {
        SetVariables();
        SetSpline();
    }


    #region Subscribtions
    private void OnEnable()
    {
        Subscribe();
    }
    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        CoreGameSignals.instance.OnSaveVariables += SaveVariables;
    }
    private void UnSubscribe()
    {
        CoreGameSignals.instance.OnSaveVariables -= SaveVariables;
    }
    #endregion

    #region Public Functions
    public void ChangeSpline()
    {
        trackLevel++;
        Splines[trackLevel - 1].SetActive(false);
        Splines[trackLevel].SetActive(true);
        switch (trackLevel)
        {
            case 1:
                carCountLimit = 12;
                break;
            case 2:
                carCountLimit = 14;
                break;
            case 3:
                carCountLimit = 16;
                break;
          
            default:
                break;
        }
        CoreGameSignals.instance.OnSplineChange?.Invoke(trackLevel);
    }

    private void Vibrate()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer) 
        {
            HapticManager.instance.SoftHaptic();
        }
        else
        {
            HapticManager.instance.LightHaptic();
        }
    }

    public void BeginSpeeding()
    {
        Vibrate();
        if (isSpeeding)
        {
            StopAllCoroutines();
            isSpeeding = false;
        }
        if (!isSpeeding)
        {
            isSpeeding = true;
            CoreGameSignals.instance.OnSpeeding?.Invoke();
            StartCoroutine(CoolDown());
        }
    }

    public void BuyItem(int price)
    {
        Money -= price;
    }

    public void AddCarToList(CarRoot carRoot)
    {
        carList.Add(carRoot);
    }

    public void RemoveCarFromList(CarRoot carRoot)
    {
        carList.Remove(carRoot);
    }
#endregion

    private void SetSpline()
    {
        Splines[trackLevel].SetActive(true);
        switch (trackLevel)
        {
            case 0:
                carCountLimit = 10;
                break;
            
            case 1:
                carCountLimit = 12;
                break;
            case 2:
                carCountLimit = 14;
                break;
            case 3:
                carCountLimit = 16;
                break;

            default:
                break;
        }
    }

    private void SetVariables()
    {
        _speedUpCooldown = mainSpecs.SpeedUpTimeLength;
        Money = PlayerPrefs.GetFloat("Money", 0);
        trackLevel = PlayerPrefs.GetInt("trackLevel", 0);
        IncomeMultiplier = PlayerPrefs.GetFloat("IncomeMultiplier", 1);
    }

    private void SaveVariables()
    {
        for (int i = 0; i < carList.Count; i++)
        {
            PlayerPrefs.SetInt("Car" + i + "Level", carList[i].CarLevel);
        }
        PlayerPrefs.SetInt("trackLevel", trackLevel);
        PlayerPrefs.SetFloat("Money", Money);
        PlayerPrefs.SetInt("CurrentCarCount", carList.Count);
        PlayerPrefs.SetFloat("IncomeMultiplier", IncomeMultiplier);

    }

    private void StopSpeeding()
    {
        if (isSpeeding)
        {
            isSpeeding = false;
            CoreGameSignals.instance.OnSlowing?.Invoke();
        }      
    }

#region Coroutines
    IEnumerator CoolDown()
    {
        
        yield return new WaitForSeconds(_speedUpCooldown);
        StopSpeeding();
    }
#endregion

#region Return Functions
    public int GetTrackLevel() => trackLevel;
    public int GetCarCountLimit() => carCountLimit;
    public int GetCurrentCarCount() => carList.Count;
#endregion

}
