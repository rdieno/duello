using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNodeScript : MonoBehaviour {

    public List<GameObject> neighborNodes;
    public int level;
    public bool isEdge;
    public bool isGapEdge;
    void Start()
    {

    }

    void Update()
    {
        for (int i = 0; i < neighborNodes.Count; i++)
        {
           // Debug.Log("Drew Line: 1x: " + gameObject.transform.position.x + ", 2x: " + neighborNodes[i].transform.position.x);
            Debug.DrawLine(gameObject.transform.position, neighborNodes[i].transform.position, Color.magenta);
        }
    }
}
