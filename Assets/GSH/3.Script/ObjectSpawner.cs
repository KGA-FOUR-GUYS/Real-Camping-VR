using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSpawner : XRGrabInteractable
{
    [SerializeField] GameObject prefab = default;
    private Vector3 attachOffset = Vector3.zero;
    public int SpawnPooling = 0;
    public GameObjectPool prefabPool { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        prefabPool = new GameObjectPool(() => CreatePrefab(), SpawnPooling);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        CreateAndSelectPrefab(args);
    }

    void CreateAndSelectPrefab(SelectEnterEventArgs args)
    {
        GameObject newObject = prefabPool.GetObject();
        if (newObject != null)
        {
            SpawnObject newSpawnObject = newObject.GetComponent<SpawnObject>();
            if (newSpawnObject != null)
            {
                newObject.SetActive(true);
                interactionManager.SelectEnter(args.interactorObject, newSpawnObject);
            }
            else
            {
                prefabPool.ReturnObject(newObject);
            }
        }
    }

    private GameObject CreatePrefab()
    {
        if (prefab == null)
        {
            return null;
        }

        GameObject newPrefab = Instantiate(prefab, transform.position, transform.rotation, transform);
        newPrefab.SetActive(false);
        return newPrefab;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        if (objectToReturn != null)
        {
            prefabPool.ReturnObject(objectToReturn);
        }
    }
    public void ResetObject()
    {
        
    }
}

public class GameObjectPool
{
    private Queue<GameObject> objectQueue = new Queue<GameObject>();
    private System.Func<GameObject> createFunc;

    public GameObjectPool(System.Func<GameObject> createFunc, int initialSize = 0)
    {
        this.createFunc = createFunc;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = createFunc();
            if (obj != null)
            {
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }
        }
    }

    public GameObject GetObject()
    {
        return objectQueue.Count > 0 ? objectQueue.Dequeue() : null;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
            objectQueue.Enqueue(obj);
        }
    }
}