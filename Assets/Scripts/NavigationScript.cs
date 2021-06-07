using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScript : MonoBehaviour {

    private List<GameObject> navMesh;
    public GameObject currentPlayerNode;
    public GameObject currentEnemyNode;
    public GameObject player;
    public GameObject enemy;

    public GameObject targetNode;
    public bool seekingNextNode = false;

    public float targetNodeDirection = -1.0f;

    public bool drawNodes = false;

    // Use this for initialization
    void Start () {
        navMesh = new List<GameObject>();
        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        foreach(Transform child in ts)
        {
            if (child.tag == "NavNode")
            {
                navMesh.Add(child.gameObject);
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
        Renderer renderer;
        for (int i = 0; i < navMesh.Count; i++)
        {
            renderer = navMesh[i].GetComponent<Renderer>();
            renderer.material.color = Color.magenta;
            if(!drawNodes)
            {
                renderer.enabled = false;
            }
        }
        
        currentPlayerNode = getClosestNode(player);
        currentEnemyNode = getClosestNode(enemy);
        getNextNode();
        GetTargetNodeDirection();


        renderer = targetNode.GetComponent<Renderer>();
        renderer.material.color = Color.yellow;


    }

    private GameObject getClosestNode(GameObject entity)
    {
        int closestNodeIndex = 0;
        float closestNodeDistance = 1000.0f;
        float distanceToNode = 0.0f;

        for (int i = 0; i < navMesh.Count; i++)
        {
            distanceToNode = Vector2.Distance(navMesh[i].transform.position, entity.transform.position);
            

            if (distanceToNode < closestNodeDistance)
            {
                closestNodeDistance = distanceToNode;
                closestNodeIndex = i;
            }
        }

        Renderer renderer = navMesh[closestNodeIndex].GetComponent<Renderer>();
        

        if(navMesh[closestNodeIndex].GetComponent<NavNodeScript>().isEdge)
        {
            //renderer.material.color = Color.cyan;
        }
        else
        {
            //renderer.material.color = Color.green;
        }

        return navMesh[closestNodeIndex];
    }

    private void getNextNode()
    {
        NavNodeScript currentNavNode = currentEnemyNode.GetComponent<NavNodeScript>();
        List<GameObject> neighbors = currentNavNode.neighborNodes;
        neighbors.Add(currentNavNode.gameObject);
        int closestNodeIndex = 0;
        float closestNodeDistance = 1000.0f;
        float distanceToNode = 0.0f;
        
        for (int i = 0; i < neighbors.Count; i++)
        {
            distanceToNode = Vector2.Distance(neighbors[i].transform.position, currentPlayerNode.transform.position);

            if (distanceToNode < closestNodeDistance)
            {
                closestNodeDistance = distanceToNode;
                closestNodeIndex = i;
            }
        }

        targetNode = neighbors[closestNodeIndex];
    }

    private void GetTargetNodeDirection()
    {
        float dir = enemy.transform.position.x - targetNode.transform.position.x;
        if(dir < 0)
        {
            targetNodeDirection = 1.0f;
        } else
        {
            targetNodeDirection = -1.0f;
        }
       
    }


}
