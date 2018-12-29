using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaiseBehaviour))]
public class LockBehaviour : MonoBehaviour {

    public enum LockColor
    {
        Red,
        Green,
        Blue
    }

    public LockColor lockColor;
    public Material unlockedMaterial;

    public MeshRenderer blockRenderer;
    public MeshRenderer keyholeRenderer;
    public RaiseBehaviour raiseBehaviour;

    [SerializeField][HideInInspector] private bool isBound;

	// Use this for initialization
	void Start () {
        raiseBehaviour = GetComponent<RaiseBehaviour>();
        isBound = false;

        float diameter = GameplayConstants.ballRadius * 2;
        transform.localScale = new Vector3(GameplayConstants.tileEdgeSize - diameter, GameplayConstants.tileEdgeSize - diameter, diameter);
    }

    // Update is called once per frame
    void Update () {
        if (!isBound)
        {
            isBound = true;
            foreach (GameObject key in GameObject.FindGameObjectsWithTag("Key"))
            {
                KeyBehaviour keyBehavour = key.GetComponent<KeyBehaviour>();
                if (keyBehavour && keyBehavour.keyColor == lockColor)
                {
                    key.GetComponent<PickupBehaviour>().onPickUp.AddListener(KeyCollected);
                }
            }
        }

        if (!raiseBehaviour.isRaised)
        {
            Color c = keyholeRenderer.material.color;
            c.a = Mathf.Lerp(1.0f, 0.2f, raiseBehaviour.raiseDeltaT);
            keyholeRenderer.material.color = c;
        }
	}

    public void KeyCollected(BallBehaviour ball)
    {
        raiseBehaviour.Lower();
        blockRenderer.material = unlockedMaterial;
    }
}
