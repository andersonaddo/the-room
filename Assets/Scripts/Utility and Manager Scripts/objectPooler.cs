using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class objectPooler : MonoBehaviour
{
    public static objectPooler Instance; //Just to make it easier to access this gameobject elsewhere
    public List<objectPoolingInfo> objectsToPool;

    Dictionary<string, GameObject> namePairs = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> parentPairs = new Dictionary<string, Transform>();
    Dictionary<GameObject, Queue<GameObject>> queuePairs = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
        //Creating the initial pools 
        foreach (objectPoolingInfo info in objectsToPool)
        {
            GameObject Parent = new GameObject();
            parentPairs[info.nameToCall] = Parent.transform;
            Parent.transform.SetParent(transform);
            Parent.name = info.nameToCall + " Pooling Parent";
            namePairs.Add(info.nameToCall, info.GO);
            queuePairs.Add(info.GO, new Queue<GameObject>());
            Queue<GameObject> poolingQueue = queuePairs[info.GO];

            for (int i = 0; i < info.initialNumber; i++)
            {
                GameObject newInstantiation = Instantiate(info.GO, Parent.transform);
                newInstantiation.SetActive(false);
                poolingQueue.Enqueue(newInstantiation);
            }            
        }
    }

    /// <summary>
    /// Returns an active gameobject
    /// </summary>
    public GameObject requestObject(string name)
    {
        Queue<GameObject> targetQueue = queuePairs[namePairs[name]];
        if (targetQueue.Count == 0) //That means all the available poolable instances are in use...
        {
            //Make a new one!!
            GameObject newInstantiation = Instantiate(namePairs[name], transform);
            newInstantiation.transform.SetParent(parentPairs[name]);
            return newInstantiation;
        }
        else
        {
            GameObject fetchedGameObject = queuePairs[namePairs[name]].Dequeue();
            fetchedGameObject.SetActive(true);
            return fetchedGameObject;
        }
    }

    /// <summary>
    /// Disables the object and places it back in pool
    /// </summary>
    public void returnObject(string name, GameObject gameObject)
    {
        gameObject.SetActive(false);
        queuePairs[namePairs[name]].Enqueue(gameObject);
    }
}

[System.Serializable]
public class objectPoolingInfo
{
    public GameObject GO;
    public string nameToCall;
    public int initialNumber;
}