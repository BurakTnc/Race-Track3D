using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private LayerMask _carLayer;

    private List<GameObject> initializedCars = new List<GameObject>();
    private Collider[] colliders = new Collider[1];
    private float timer;
    private int visibleObjectCount;
    private int spawnedCarAtBeginning = 0;
    private bool isSpawnable = false;
    private bool carsInitialized = false;


    private void Start()
    {

        InitializeCars();

    }

    private void OnEnable()
    {
        CoreGameSignals.instance.OnSpawnCar += BeginSpawn;
        CoreGameSignals.instance.OnKeepCurrentCars += KeepCurrentCars;
    }

    private void OnDisable()
    {
        CoreGameSignals.instance.OnSpawnCar -= BeginSpawn;
        CoreGameSignals.instance.OnKeepCurrentCars -= KeepCurrentCars;
    }

    private void Update()
    {       
        ScanTheArea();
        SpawnQueue();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 10);
    }

    private void ScanTheArea()
    {
        visibleObjectCount = Physics.OverlapSphereNonAlloc(transform.position, 10, colliders, _carLayer);
    }

    private void BeginSpawn()
    {
        isSpawnable = true;
    }

    private void KeepCurrentCars()
    {
        carsInitialized = true;
    }

    private void SpawnQueue()
    {
        if (isSpawnable)
        {
            if (visibleObjectCount<1)
            {
                isSpawnable = false;
                Spawn();
            }
        }
        if (!carsInitialized)
        {
            
            if (initializedCars.Count > 0 && timer<=0) 
            {
                PlaceCars();
                timer += .25f;
            }
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, 1);
        }
    }

    private void Spawn()
    {
        GameObject car = Instantiate(Resources.Load<GameObject>(path: "Spawnables/Car Roots/CarRoot"),
            transform.position, Quaternion.identity);
        GameManager.instance.AddCarToList(car.GetComponent<CarRoot>());
        car.GetComponent<CarRoot>().StartMove();
        CoreGameSignals.instance.OnSlowing?.Invoke();
        CoreGameSignals.instance.OnSaveVariables?.Invoke();
    }


    private void PlaceCars()
    {       
        if (spawnedCarAtBeginning < PlayerPrefs.GetInt("CurrentCarCount", 1))
        {
            GameObject car = initializedCars[spawnedCarAtBeginning];
            car.transform.position = transform.position;
            car.GetComponent<CarRoot>().StartMove();
            spawnedCarAtBeginning++;
        }

        else
        {
            carsInitialized = true;
        }
    }

    private void InitializeCars()
    {
        if (!carsInitialized)
        {
            for (int i = 0; i < PlayerPrefs.GetInt("CurrentCarCount", 1); i++)
            {
                CarRoot car = Instantiate(Resources.Load<GameObject>(path: "Spawnables/Car Roots/CarRoot"),
                    transform.position, Quaternion.identity).GetComponent<CarRoot>();
                car.ResetPath();
                initializedCars.Add(car.gameObject);
                GameManager.instance.AddCarToList(car);
                CoreGameSignals.instance.OnSlowing?.Invoke();
                car.SetCarLevel(PlayerPrefs.GetInt("Car" + i + "Level", 0));

#if UNITY_EDITOR
                car.gameObject.name = (i + 1).ToString();
#endif
            }
        }     
    }

}
