using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButtonBehaviour : MonoBehaviour {

    public GameObject toClose;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Close()
    {
        toClose.SetActive(false);
    }
}
