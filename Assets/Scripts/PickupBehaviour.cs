using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupBehaviour : MonoBehaviour {

    [System.Serializable]
    public class PickupEvent : UnityEvent<BallBehaviour> { }

    public enum PickupAnimation
    {
        None,
        SpinZ,
        Hover,
        SpinY,

    }

    public Transform graphicsTransform;
    public PickupEvent onPickUp;
    public PickupAnimation pickupAnimation;

    [SerializeField] [HideInInspector] private float accumulator;

	// Use this for initialization
	void Start () {
        accumulator = 0;
	}
	
	// Update is called once per frame
	void Update () {
        accumulator += Time.deltaTime;

        switch (pickupAnimation)
        {
            case PickupAnimation.SpinZ:
                transform.Rotate(Vector3.forward, Time.deltaTime * 360.0f);
                break;

            case PickupAnimation.SpinY:
                transform.Rotate(Vector3.up, Time.deltaTime * 360.0f);
                break;

            case PickupAnimation.Hover:
                graphicsTransform.localPosition = Quaternion.Euler(0.0f, 0.0f, accumulator * 360.0f) * Vector3.right * 0.1f;
                break;
        }
    }

    public void PickUp(BallBehaviour ball)
    {
        onPickUp.Invoke(ball);
        Destroy(gameObject);
    }

    public void OnPickupDestroyBall(BallBehaviour ball)
    {
        Destroy(ball.gameObject);
    }

}
