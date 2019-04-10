using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Please note that this code doesn't have error checking for if the 
//user request for a name which isn't recognized my the system, or
//if it tries to loadout a stage that hasn't been loaded in
public class objectPooler : MonoBehaviour
{
    //You'll notice that these stages are actually the stages of the game.
    //This pooling system creates (loads in) and destroys (loads out) pools only the player reaches the stage in 
    //which these objects are needed
    public enum poolingStage
    {
        firstRoom,
        escapeCorridor,
        bossBattle,
        permanent
    }

    [SerializeField] poolingStage currentStage;

    public static objectPooler Instance; //Just to make it easier to access this gameobject elsewhere

    public List<objectPoolingInfo> objectsToPool;

    Dictionary<string, GameObject> gameobjectPairs = new Dictionary<string, GameObject>();
    Dictionary<string, poolingStage> stagePairs = new Dictionary<string, poolingStage>();
    Dictionary<string, Transform> parentPairs = new Dictionary<string, Transform>();
    Dictionary<string, Queue<GameObject>> queuePairs = new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        initializeStageDictionary();
        loadInPermanentObjects();
        loadInObjects(currentStage);        
    }

    /// <summary>
    /// Returns an active gameobject. Will be null of the object is not accessible.
    /// </summary>
    public GameObject requestObject(string name)
    {
        if (stagePairs[name] != currentStage && stagePairs[name] != poolingStage.permanent) return null;

        Queue<GameObject> targetQueue = queuePairs[name];
        if (targetQueue.Count == 0) //That means all the available poolable instances are in use...
        {
            //Make a new one!!
            GameObject newInstantiation = Instantiate(gameobjectPairs[name], transform);
            newInstantiation.transform.SetParent(parentPairs[name]);
            return newInstantiation;
        }
        else
        {
            GameObject fetchedGameObject =targetQueue.Dequeue();
            fetchedGameObject.SetActive(true);
            return fetchedGameObject;
        }
    }

    /// <summary>
    /// Disables the object and places it back in pool. 
    /// Destroys the gameobject if it's no longer needed.
    /// </summary>
    public void returnObject(string name, GameObject gameObject)
    {
        //Is this object queue available?
        if (stagePairs[name] != poolingStage.permanent && stagePairs[name] != currentStage)
        {
            Destroy(gameObject);
            return;
        }
        gameObject.SetActive(false);
        queuePairs[name].Enqueue(gameObject);
    }

    public void switchStage(poolingStage stage)
    {
        loadOutObjects(currentStage);
        currentStage = stage;
        loadInObjects(currentStage);
    }

    void initializeStageDictionary()
    {
        foreach (objectPoolingInfo info in objectsToPool)
        {
            stagePairs.Add(info.nameToCall, info.stageNeeded);
        }
    }

    void loadInPermanentObjects()
    {
        loadInObjects(poolingStage.permanent);
    }

    void loadInObjects(poolingStage stage)
    {
        foreach (objectPoolingInfo info in objectsToPool)
        {
            if (info.stageNeeded != stage) continue;

            //Making parent for this pool's objects
            GameObject Parent = new GameObject();
            parentPairs[info.nameToCall] = Parent.transform;
            Parent.transform.SetParent(transform);
            Parent.name = info.nameToCall + " Pooling Parent";

            //Registering the prefab in case it needs to be used requestObject()
            gameobjectPairs.Add(info.nameToCall, info.GO);

            //Making the pool itself and populating it
            queuePairs.Add(info.nameToCall, new Queue<GameObject>());
            Queue<GameObject> poolingQueue = queuePairs[info.nameToCall];

            for (int i = 0; i < info.initialNumber; i++)
            {
                GameObject newInstantiation = Instantiate(info.GO, Parent.transform);
                newInstantiation.SetActive(false);
                poolingQueue.Enqueue(newInstantiation);
            }
        }
    }

    void loadOutObjects(poolingStage stage)
    {

        foreach (objectPoolingInfo info in objectsToPool)
        {
            if (info.stageNeeded != stage) continue;

            //Destroying all the gameobjects in the queue
            Queue<GameObject> objectQueue = queuePairs[info.nameToCall];
            while (objectQueue.Peek() != null)
            {
                Destroy(objectQueue.Dequeue());
            }
        }

        //It's not necessary to need to deal with the other dictoinaries. Yes, this is something like
        //memory leak, but it's not that significant. The most memory hungry dictionaty (the one with the pools themselves) have been dealt with.
    }
}

[System.Serializable]
public class objectPoolingInfo
{
    public GameObject GO;
    public objectPooler.poolingStage stageNeeded;
    public string nameToCall;
    public int initialNumber;
}