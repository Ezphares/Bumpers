using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakerSpecialRuleBehaviour : SpecialRuleBehaviour {

	// Use this for initialization
	void Start () {
        graphicsTransform.localScale = Vector3.one * (GameplayConstants.tileEdgeSize * 0.75f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnInitialize(BoardBehaviour board)
    {
        
        foreach (BumperBehaviour bumper in board.GetBumpers())
        {
            bumper.raiseBehaviour.onLower.AddListener((RaiseBehaviour b) =>
            {
                bumper.Kill();
            });
        }
    }
}
