using UnityEngine;

[CreateAssetMenu(fileName = "Main Data", menuName = "Create Main Data", order = 1)] 
public class MainCarSpecs : ScriptableObject
{
    public float FollowSpeed = 100;
    [Range(1.1f, 3f)]
    public float SpeedUpRate = 1.3f;
    [Range(0.5f, 2f)]
    public float SpeedUpTimeLength = 0.5f;
}
