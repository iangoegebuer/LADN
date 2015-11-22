using UnityEngine;
using System.Collections;

public class BallCtl : MonoBehaviour {
	Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * 10f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
