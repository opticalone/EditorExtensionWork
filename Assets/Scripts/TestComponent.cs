using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour {

    public Vector3 testone = Vector3.zero;
    public Vector3 testtwo
    {
        get { return testone; }
        set { testone = value; }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
