using Lofelt.NiceVibrations;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager instance;
    private bool hapticsSupported;
    private void Awake()
    {
        if (instance != this && instance != null) 
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        hapticsSupported = DeviceCapabilities.isVersionSupported;
    }


    public void LightHaptic()
    {
        if (hapticsSupported)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
    }

    public void SoftHaptic()
    {
        if (hapticsSupported)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);
        }
    }

    public void RigidHaptic()
    {
        if (hapticsSupported)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
        }
    }

    public void SuccessHaptic()
    {
        if (hapticsSupported)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }
    }
}
