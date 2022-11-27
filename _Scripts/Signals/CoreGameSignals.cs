using UnityEngine.Events;
using UnityEngine;

public class CoreGameSignals : MonoBehaviour
{
    public static CoreGameSignals instance;

    public UnityAction<int> OnGameInitialize = delegate { };
    public UnityAction<int> OnSplineChange = delegate { };
    public UnityAction OnKeepCurrentCars = delegate { };
    public UnityAction BeginCarMerge = delegate { };
    public UnityAction StopCarMerge = delegate { };
    public UnityAction OnCheckPoint = delegate { };
    public UnityAction OnSpeeding = delegate { };
    public UnityAction OnSlowing = delegate { };
    public UnityAction OnSaveVariables = delegate { };
    public UnityAction OnSpawnCar = delegate { };

    #region Debug
    public UnityAction<bool> OnDebugMode = delegate { };
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

}
