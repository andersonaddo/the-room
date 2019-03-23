using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class objectPooler : MonoBehaviour
{
    public static objectPooler Instance;
    public List<objectPoolingInfo> objectsToPool;
    Dictionary<string, GameObject> namePairs = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> parentPairs = new Dictionary<string, Transform>();
    Dictionary<GameObject, List<GameObject>> listPairs = new Dictionary<GameObject, List<GameObject>>();

    void Awake()
    {
        Instance = this;
        foreach(objectPoolingInfo info in objectsToPool)
        {
            GameObject Parent = new GameObject();
            parentPairs[info.nameToCall] = Parent.transform;
            Parent.transform.SetParent(transform);
            Parent.name = info.nameToCall + " Pooling Parent";
            namePairs.Add(info.nameToCall, info.GO);
            listPairs.Add(info.GO, new List<GameObject>());
            List<GameObject> newList = listPairs[info.GO];

            for (int i = 0; i < info.initialNumber; i++)
            {
                GameObject newInstantiation = Instantiate(info.GO, Parent.transform);
                newInstantiation.SetActive(false);
                newList.Add(newInstantiation);
            }            
        }
    }


    public GameObject requestObject(string name)
    {
        GameObject fetchedGameObject = listPairs[namePairs[name]].Where(go => !go.activeSelf).FirstOrDefault();
        if (fetchedGameObject == null)
        {
            //That means all the GOs are in use right now. Make a new one!!
            GameObject newInstantiation = Instantiate(namePairs[name], transform);
            listPairs[namePairs[name]].Add(newInstantiation);
            newInstantiation.transform.SetParent(parentPairs[name]);
            return newInstantiation;
        }
        else
        {
          fetchedGameObject.SetActive(true);
          return fetchedGameObject;
        }
    }
}

[System.Serializable]
public class objectPoolingInfo
{
    public GameObject GO;
    public string nameToCall;
    public int initialNumber;
}