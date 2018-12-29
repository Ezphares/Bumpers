using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBehaviour : MonoBehaviour {

    public enum GameProgress
    {
        Pregame,
        Running,
        Ended
    }

    public bool isPaused;
    public GameProgress gameProgress;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsRunning()
    {
        return (!isPaused && gameProgress == GameProgress.Running);
    }
}
