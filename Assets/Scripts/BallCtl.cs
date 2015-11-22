using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BallCtl : NetworkBehaviour {
	struct RBInfo {
		public Vector3 position;
		public Vector3 vel;
	}
	Rigidbody rb;

	[SyncVar]
	RBInfo physState;

	// Use this for initialization
	void Start () {
		Debug.Log (isClient);
		Debug.Log (isServer);
		rb = GetComponent<Rigidbody>();
		InitState();
	}

	[Server]
	void InitState () {
		rb.isKinematic = false;
		rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, ((Random.value - 0.5f)<0)?1f:-1f) * 50f;
		physState.position = rb.position;
		physState.vel = rb.velocity;
	}
	
	// Update is called once per frame
	void Update () {
		TestRandMotion();
		SyncState();
	}

	
	void SyncState () {
		if (isServer) {
			physState.position = rb.position;
			physState.vel = rb.velocity;
		} else {
			rb.position = physState.position;
			rb.velocity = physState.vel;
		}
	}

	[Server]
	void TestRandMotion () {
		if (Random.value < 0.005f) {
			rb.velocity = new Vector3(Random.value, Random.value, Random.value) - Vector3.one * 0.5f;
		}
		rb.velocity = rb.velocity.normalized * 5f;
	}
}
