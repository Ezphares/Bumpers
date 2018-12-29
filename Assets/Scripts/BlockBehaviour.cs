using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehaviour : MonoBehaviour {

    public Transform wallPrefab;

	// Use this for initialization
	void Start () {
        float diameter = GameplayConstants.ballRadius * 2;
        transform.localScale = new Vector3(GameplayConstants.tileEdgeSize - diameter, GameplayConstants.tileEdgeSize - diameter, diameter);

        Ray blockRay = new Ray(transform.position, Vector3.up);
        RaycastHit hit;
        
        if (Physics.Raycast(blockRay, out hit, GameplayConstants.tileEdgeSize))
        {
            if (hit.collider.GetComponent<BlockBehaviour>() != null)
            {
                Transform wall = Instantiate(wallPrefab, transform.position + Vector3.up * GameplayConstants.tileEdgeSize / 2, Quaternion.identity, transform.parent);
                wall.localScale = new Vector3(GameplayConstants.tileEdgeSize - diameter, diameter, diameter);
            }
        }

        blockRay = new Ray(transform.position, Vector3.right);
        if (Physics.Raycast(blockRay, out hit, GameplayConstants.tileEdgeSize))
        {
            if (hit.collider.GetComponent<BlockBehaviour>() != null)
            {
                Transform wall = Instantiate(wallPrefab, transform.position + Vector3.right * GameplayConstants.tileEdgeSize / 2, Quaternion.identity, transform.parent);
                wall.localScale = new Vector3(diameter, GameplayConstants.tileEdgeSize - diameter,diameter);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
