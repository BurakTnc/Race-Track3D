using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    [SerializeField] private int[] _carPrices, _incomePrices, _trackPrices, _mergePrices;
    [SerializeField] private Button _addCarButton, _incomeButton, _trackButton, _mergeButton;
    [SerializeField] private TextMeshProUGUI _carButtonText, _incomeButtonText, _trackButtonText, _mergeButtonText, _moneyText;

    private int addCarLevel, incomeLevel, trackLevel, mergeLevel;

    #region Debug
    [SerializeField] private GameObject _debugPanel;
    private bool onDebugMode = false;
    #endregion



    private void Awake()
    {
        if (instance != this && instance != null) 
        {
            Destroy(this);
            return;
        }
        instance = this;
        GetStats();
    }

    private void Start()
    {
        SetButtonStats();
    }


    #region Public Functions
    public void SetButtonStats()
    {
        CalculatePriceTag(GameManager.instance.Money, _moneyText); //For Money Text
        CheckButtonStats(_carPrices, addCarLevel, _addCarButton, _carButtonText);
        CheckButtonStats(_incomePrices, incomeLevel, _incomeButton, _incomeButtonText);
        CheckButtonStats(_trackPrices, trackLevel, _trackButton, _trackButtonText);
        CheckButtonStats(_mergePrices, mergeLevel, _mergeButton, _mergeButtonText);

    }
    #endregion

    private void CalculatePriceTag(float price,TextMeshProUGUI priceTag)
    {
        if (price>=1000)
        {
            int thousand = Mathf.FloorToInt((int)price / 1000);
            int hundred = Mathf.FloorToInt((int)price % 1000);
            hundred = Mathf.FloorToInt(hundred / 100);

            priceTag.text = "$ " + thousand + "." + hundred + "k";
        }
        else
        {
            priceTag.text = "$ " + (int)price;
        }
    }

    private void CheckButtonStats(int[] priceList, int level, Button button, TextMeshProUGUI tag)
    {
        if (button == _addCarButton)
        {
            if (GameManager.instance.GetCurrentCarCount() >= GameManager.instance.GetCarCountLimit())
            {
                button.interactable = false;
                tag.text = "MAX";
            }
            else
            {
                if (GameManager.instance.Money < priceList[level])
                {
                    button.interactable = false;
                    CalculatePriceTag(priceList[level], tag);
                }

                else
                {
                    button.interactable = true;
                    CalculatePriceTag(priceList[level], tag);
                }
            }
        }

        else if (button == _mergeButton)
        {
            if (MergeIsAvaliable() && GameManager.instance.Money >= priceList[level])
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }


            CalculatePriceTag(priceList[level], tag);
        }

        else if (button == _trackButton) 
        {
            if (trackLevel == priceList.Length - 1) 
            {
                button.interactable = false;
                tag.text = "MAX";
            }
            else
            {
                if (GameManager.instance.Money < priceList[level])
                {
                    button.interactable = false;
                    CalculatePriceTag(priceList[level], tag);
                }

                else
                {
                    button.interactable = true;
                    CalculatePriceTag(priceList[level], tag);
                }
            }
        }

        else
        {

            if (GameManager.instance.Money < priceList[level])
            {
                button.interactable = false;
                
            }

            else
            {
                button.interactable = true;
                
            }
            CalculatePriceTag(priceList[level], tag);
        }     
    }

    private bool MergeIsAvaliable()
    {
        int lv1 = GameObject.FindGameObjectsWithTag("LV1").Length;
        int lv2= GameObject.FindGameObjectsWithTag("LV2").Length;
        int lv3 = GameObject.FindGameObjectsWithTag("LV3").Length;
        int lv4 = GameObject.FindGameObjectsWithTag("LV4").Length;
        if (lv1>1)
        {
            return true;
        }
        else
        {
            if (lv2>1)
            {
                return true;
            }
            else
            {
                if (lv3>1)
                {
                    return true;
                }
                else
                {
                    if (lv4>1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }

    private void GetStats()
    {
        addCarLevel = PlayerPrefs.GetInt("addCarLevel", 0);
        incomeLevel = PlayerPrefs.GetInt("incomeLevel", 0);
        trackLevel = PlayerPrefs.GetInt("trackLevel", 0);
        mergeLevel = PlayerPrefs.GetInt("mergeLevel", 0);
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("addCarLevel", addCarLevel);
        PlayerPrefs.SetInt("incomeLevel", incomeLevel);
        PlayerPrefs.SetInt("trackLevel", trackLevel);
        PlayerPrefs.SetInt("mergeLevel", mergeLevel);
    }

    public void AddCarButton()
    {
        HapticManager.instance.LightHaptic();
        GameManager.instance.BuyItem(_carPrices[addCarLevel]);
        CoreGameSignals.instance.OnSpawnCar?.Invoke();
        if (addCarLevel < _carPrices.Length -1)
        {
            addCarLevel++;
        }
        SaveStats();
        SetButtonStats();
    }

    public void ExpandTrackButton()
    {
        HapticManager.instance.LightHaptic();
        if (trackLevel < _trackPrices.Length-1 )
        {

            GameManager.instance.BuyItem(_trackPrices[trackLevel]);
            GameManager.instance.ChangeSpline();
            CoreGameSignals.instance.OnKeepCurrentCars?.Invoke();
            CoreGameSignals.instance.OnSaveVariables?.Invoke();
            trackLevel++;
        }
        SaveStats();
        SetButtonStats();
    }

    public void IncomeButton()
    {
        HapticManager.instance.LightHaptic();
        GameManager.instance.BuyItem(_incomePrices[incomeLevel]);
        if (incomeLevel < _incomePrices.Length-1)
        {
            incomeLevel++;            
        }
        GameManager.instance.IncomeMultiplier += .1f;
        CoreGameSignals.instance.OnSaveVariables?.Invoke();
        SaveStats();
        SetButtonStats();
    }

    public void MergeButton()
    {
        HapticManager.instance.LightHaptic();
        GameManager.instance.BuyItem(_mergePrices[mergeLevel]);
        CoreGameSignals.instance.BeginCarMerge?.Invoke();
        CoreGameSignals.instance.OnSlowing?.Invoke();
        CoreGameSignals.instance.OnSaveVariables?.Invoke();
        if (mergeLevel<_mergePrices.Length-1)
        {
            mergeLevel++;
        }
        SaveStats();
        SetButtonStats();
    }

    #region Debug
    public void OpenDebugMenuButton()
    {
        HapticManager.instance.LightHaptic();
        _debugPanel.SetActive(true);     
    }
    public void CloseDebugMenuButton()
    {
        HapticManager.instance.LightHaptic();
        _debugPanel.SetActive(false);
    }
    public void DebugModeButton(bool mode)
    {
        HapticManager.instance.LightHaptic();
        onDebugMode = !onDebugMode;
        CoreGameSignals.instance.OnDebugMode?.Invoke(onDebugMode);
    }
    public void AddMoneyButton()
    {
        HapticManager.instance.LightHaptic();
        GameManager.instance.Money += 100000;
        SetButtonStats();
    }
    public void ResetGameButton()
    {
        HapticManager.instance.LightHaptic();
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }

    #endregion
}
