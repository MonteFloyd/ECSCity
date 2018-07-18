using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

    private Vector3 centerPos;

	// Use this for initialization
	void Start () {
        centerPos = new Vector3(0, 0, 0);
	}

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 150.0f;

        //trfoansrm.Rotate(0, x, 0);
        transform.Translate(x, 0, z);
        transform.LookAt(centerPos);
    }
}
