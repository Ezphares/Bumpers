using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSpecialRuleBehaviour : SpecialRuleBehaviour {

    public int ticksTotal;
    public int ticksLeft;
    public TextMesh text;

    // Use this for initialization
    void Start () {
        ticksLeft = ticksTotal;
        graphicsTransform.localScale = Vector3.one * (GameplayConstants.tileEdgeSize - GameplayConstants.ballRadius * 2.2f);
	}
	
	// Update is called once per frame
	void Update () {

        float timeInSeconds = ((float)ticksLeft) * tickTime;
        int intSeconds = (int)timeInSeconds;
        int minutes = intSeconds / 60;
        intSeconds -= minutes * 60;

        text.text = string.Format("{0}:{1:D2}", minutes, intSeconds);

        // Flash display when less than three seconds
        if (intSeconds <= 3)
        {
            float dec = timeInSeconds - Mathf.Floor(timeInSeconds);

            text.gameObject.SetActive(dec < 0.75f);
        }
	}

    public override bool HasCausedDefeat()
    {
        return ticksLeft <= 0;
    }

    public override void OnTick()
    {
        ticksLeft -= 1;
    }
}
