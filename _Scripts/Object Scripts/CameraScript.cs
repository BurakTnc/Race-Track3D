using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private bool onDebugMode = false;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void OnEnable()
    {
        CoreGameSignals.instance.OnDebugMode += DebugMode;
    }
    private void OnDisable()
    {
        CoreGameSignals.instance.OnDebugMode -= DebugMode;
    }

    private void DebugMode(bool mode)
    {
        onDebugMode = mode;
        if (!onDebugMode)
        {
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
        }
    }

    private void Start()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    void Update()
    {
        if (onDebugMode)
        {
            float posX = DebugManager.instance._camPosX.value;
            float posY = DebugManager.instance._camPosY.value;
            float posZ = DebugManager.instance._camPosZ.value;
            Vector3 debugPosition = new Vector3(posX, posY, posZ);

            float rotX = DebugManager.instance._camRotX.value;
            float rotY = DebugManager.instance._camRotY.value;
            float rotZ = DebugManager.instance._camRotZ.value;
            Quaternion debugRotation = Quaternion.Euler(rotX, rotY, rotZ);

            transform.position = debugPosition;
            transform.rotation = debugRotation;
        }
    }
}
