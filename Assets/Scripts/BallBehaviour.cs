using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour {

    public Transform meshTransform;
    public Transform forwardIndicatorTransform;

    public Vector3 forward = Vector3.right;

    // Use this for initialization
    void Start () {
		if (meshTransform)
        {
            meshTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * 2 * GameplayConstants.ballRadius;
        }
        if (forwardIndicatorTransform)
        {
            forwardIndicatorTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * 5 * GameplayConstants.ballRadius;

            float forwardAngle = Vector3.Angle(forward, Vector3.right);
            if (forward.y < 0.0f)
            {
                forwardAngle *= -1.0f;
            }
            forwardIndicatorTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, forwardAngle);
        }

    }
	
	// Update is called once per frame
	void Update () {
        Debug.DrawRay(transform.position, forward, Color.red);
	}
}
