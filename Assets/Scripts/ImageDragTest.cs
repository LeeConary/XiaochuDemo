using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDragTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mousePos = Input.mousePosition;
        if (Input.GetMouseButton(0) && Vector3.Distance(mousePos, transform.position) <= 100f)
        {
            transform.position = mousePos;
        }
    }
}
