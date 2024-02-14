using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ObjectSpawner : XRGrabInteractable
{
    [SerializeField] GameObject prefab = default;
    private Vector3 attachOffset = Vector3.zero;
    public int SpawnPooling = 0;
    [SerializeField] private ObjectPool<SpawnObject> prefabPool;
    protected override void Awake()
    {
        base.Awake();
        prefabPool = new ObjectPool<SpawnObject>(() => CreatePrefab(), SpawnPooling);
        
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        CreateAndSelectPrefab();
    }
    void CreateAndSelectPrefab()
    {
        SpawnObject interactable = prefabPool.GetObject();
        SelectPrefab(interactable);
    }

    SpawnObject CreatePrefab()
    {
        SpawnObject newObj = Instantiate(prefab, transform.position - attachOffset, transform.rotation).GetComponent<SpawnObject>();
        newObj.gameObject.SetActive(false);
        return newObj;
    }

    void SelectPrefab(SpawnObject interactable)
    {
        if (interactable != null)
        {
            var args = new SelectEnterEventArgs();
            interactable.SelectPrefab(args);
        }
    }
}
public class ObjectPool<T> where T : MonoBehaviour
{
    private Queue<T> objectQueue = new Queue<T>();
    private System.Func<T> createFunc;

    public ObjectPool(System.Func<T> createFunc, int initialSize = 0)
    {
        this.createFunc = createFunc;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = createFunc();
            obj.gameObject.SetActive(false);
            objectQueue.Enqueue(obj);
        }
    }

    public T GetObject()
    {
        if (objectQueue.Count > 0)
        {
                T obj = objectQueue.Dequeue();
                obj.gameObject.SetActive(true);
                return obj;
        }
        else
        {
            T newObj = createFunc();
            return newObj;
        }
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        objectQueue.Enqueue(obj);
    }
}
