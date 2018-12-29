using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultBehaviour : MonoBehaviour {

    //TODO Have this scale according to applicable game play constants

    public Transform entryIndicator;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool CanEnter(Vector3 currentDirection)
    {
        Vector3 entryNormal = (transform.position - entryIndicator.position).normalized;
        Vector3 currentNormal = currentDirection.normalized;

        return (Vector3.Distance(entryNormal, currentNormal) < 0.1f);
    }
}
