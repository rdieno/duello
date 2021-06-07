using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastZone : MonoBehaviour {
    //handling for the killfield below the map.
    private float damage = 100;
    private Collider self;

	// Use this for initialization
	void Start () {
        self = gameObject.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
        Collider[] cols = Physics.OverlapBox(self.bounds.center, self.bounds.extents, self.transform.rotation, LayerMask.GetMask("Hitbox"));
        foreach (Collider c in cols)
        {
            if (c.transform.root == transform)
            {
                continue;
            }
            c.SendMessageUpwards("TakeDamage", damage);

        }
    }

    

}
