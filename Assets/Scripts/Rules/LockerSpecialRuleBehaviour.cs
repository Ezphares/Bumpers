using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerSpecialRuleBehaviour : SpecialRuleBehaviour {

	// Use this for initialization
	void Start () {
        graphicsTransform.localScale = Vector3.one * (GameplayConstants.tileEdgeSize * 0.75f);
    }

    // Update is called once per frame
    public override void OnInitialize(BoardBehaviour board)
    {

        foreach (BumperBehaviour bumper in board.GetBumpers())
        {
            bumper.raiseBehaviour.onRaise.AddListener((RaiseBehaviour b) =>
            {
                bumper.isLockedByRule = true;
            });
        }
    }
}
