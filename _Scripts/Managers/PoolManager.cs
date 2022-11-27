using TMPro;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;

    [SerializeField] private List<GameObject> _incomeTexts = new List<GameObject>();
    [SerializeField] private List<GameObject> _mergeEffects = new List<GameObject>();


    private void Awake()
    {
        if (instance != this && instance != null) 
        {
            Destroy(this);
            return;
        }
        instance = this;
    }


    public void PullIncomeText(float value, Vector3 desiredPosition)
    {
        value *= GameManager.instance.IncomeMultiplier;
        GameManager.instance.Money += value;
        GameObject temp = _incomeTexts[0];
        TextMeshPro txt = temp.GetComponent<TextMeshPro>();
        _incomeTexts.Remove(temp);
        txt.text = "$" + value.ToString("F1");
        temp.transform.localScale = Vector3.one;
        temp.transform.position = desiredPosition;
        temp.SetActive(true);
        temp.transform.DOMoveY(16, 1.1f).SetRelative(true).OnComplete(() => DisableObject(temp, _incomeTexts));
        temp.transform.DOScale(Vector3.zero, 1.2f).SetEase(Ease.InSine);
        ButtonManager.instance.SetButtonStats();
        CoreGameSignals.instance.OnSaveVariables?.Invoke();
    }

    public void PullConffetti(Vector3 desiredPosition)
    {
        GameObject temp = _mergeEffects[1];
        _mergeEffects.Remove(temp);
        temp.transform.position = desiredPosition;
        temp.SetActive(true);
        _mergeEffects.Add(temp);
    }

    private void DisableObject(GameObject obj,List<GameObject> pool )
    {
        obj.SetActive(false);
        pool.Add(obj);
    }
}
