using UnityEngine;

public class CheckPoint : MonoBehaviour
{   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            if (other.TryGetComponent(out CarRoot carRoot))
            {
                Vector3 offset = new Vector3(0, 15f, -10);
                Vector3 particleOffset = new Vector3(1, 5, 0);

                switch (carRoot.CarLevel)
                {
                    case 0:
                        PoolManager.instance.PullIncomeText(1, other.transform.position + offset);
                        PoolManager.instance.PullConffetti(other.transform.position + particleOffset);
                        break;

                    case 1:
                        PoolManager.instance.PullIncomeText(3, other.transform.position + offset);
                        PoolManager.instance.PullConffetti(other.transform.position + particleOffset);
                        break;

                    case 2:
                        PoolManager.instance.PullIncomeText(6, other.transform.position + offset);
                        PoolManager.instance.PullConffetti(other.transform.position + particleOffset);
                        break;

                    case 3:
                        PoolManager.instance.PullIncomeText(10, other.transform.position + offset);
                        PoolManager.instance.PullConffetti(other.transform.position + particleOffset);
                        break;
                    default:
                        break;
                }
            }
        }        
    }
}
