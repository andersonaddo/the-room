using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corridorScroller : MonoBehaviour
{
    [SerializeField] float chunkLength, speed, maximumDistance;
    [Tooltip("Counting from away to player")][SerializeField] List<Transform> chunks;
    [SerializeField] Transform player;

    void Update()
    {
        foreach (Transform chunk in chunks)
        {
            chunk.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Vector3.Distance(player.position, chunks[0].position) >= maximumDistance)
        {
            //Shift the farthest chunk back...
            chunks[0].position = chunks[chunks.Count - 1].position + Vector3.back * chunkLength;
            Transform movedChunk = chunks[0];
            chunks.RemoveAt(0);
            chunks.Insert(chunks.Count, movedChunk);
        }
    }
}
