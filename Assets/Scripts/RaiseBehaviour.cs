using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseBehaviour : MonoBehaviour {

    [System.Serializable] public class RaiseEvent : UnityEngine.Events.UnityEvent<RaiseBehaviour> {}

    public float raiseTime = 0.1f;
    public bool isRaised;
    public bool shouldLower;

    [Header("Bound Events")]
    public RaiseEvent onRaise;
    public RaiseEvent onLower;

    [SerializeField] [HideInInspector] public float raiseDeltaT;
    [SerializeField] [HideInInspector] private Vector3 loweredPosition;
    [SerializeField] [HideInInspector] private Vector3 raisedPosition;

    // Use this for initialization
    void Start () {
        raiseDeltaT = 1.0f;
        loweredPosition = transform.position;
        raisedPosition = transform.position + Vector3.back * GameplayConstants.ballRadius * 2;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isRaised && shouldLower)
        {
            shouldLower = false;
            isRaised = false;
            raiseDeltaT = 0.0f;
            onLower.Invoke(this);
        }

        raiseDeltaT += Time.deltaTime / raiseTime;
        raiseDeltaT = Mathf.Clamp01(raiseDeltaT);
        if (isRaised)
        {
            transform.position = Vector3.Lerp(loweredPosition + Vector3.back * GameplayConstants.ballRadius * 0.2f, raisedPosition, raiseDeltaT);
        }
        else
        {
            transform.position = Vector3.Lerp(raisedPosition, loweredPosition, raiseDeltaT);
        }
    }

    public void Raise()
    {
        transform.position = transform.position + Vector3.back * GameplayConstants.ballRadius * 0.2f;
        isRaised = true;
        raiseDeltaT = 0;
        onRaise.Invoke(this);
    }

    public void Lower()
    {
        shouldLower = true;
    }
}
