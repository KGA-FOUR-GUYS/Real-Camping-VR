using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public struct PublicProp
{
    public Transform spawnPoint;
    public GameObject prefab;
}

public class PublicPropSpawner : NetworkBehaviour
{
    public List<PublicProp> propList = new List<PublicProp>();

    private void Start()
    {
        if (!isServer) return;

        foreach (var prop in propList)
        {
            var gameObj = Instantiate(prop.prefab, prop.spawnPoint);
            NetworkServer.Spawn(gameObj);
        }
    }
}

