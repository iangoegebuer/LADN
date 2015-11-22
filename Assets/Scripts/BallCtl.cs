using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BallCtl : NetworkBehaviour {
	struct RBInfo {
		public Vector3 position;
		public Vector3 vel;
	}
	public GameObject scores;
	Rigidbody rb;
	Vector3 startPos;

	[SyncVar]
	RBInfo physState;

	// Use this for initialization  ((Random.value - 0.5f)<0)?1f:
	void Start () {
		Debug.Log (isClient);
		Debug.Log (isServer);
		rb = GetComponent<Rigidbody>();
		if (isServer) {
			InitState();
			StartCoroutine(SendPosCoroutine(0.1f));
		}
	}

	[Server]
	void InitState () {
		rb.isKinematic = false;
		startPos = transform.position;
		rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, ((Random.value - 0.5f)<0)?1f:-1f) * 50f;
		physState.position = rb.position;
		physState.vel = rb.velocity;
	}

	static IEnumerator ResetTrail(TrailRenderer trail)
	{
		var trailTime = trail.time;
		trail.time = -1;
		yield return new WaitForEndOfFrame();
		trail.time = trailTime;
	}  
	
	// Update is called once per frame
	void Update () {
		//TestRandMotion();
		if (isServer) {
			HandleAllMotion();
		}
		SyncState();
	}

	
	void SyncState () {
		if (!isServer) {
			rb.position = physState.position;
			rb.velocity = physState.vel;
		}
	}

	[Server]
	void HandleAllMotion () {
		if (transform.position.z < -25f) {
			transform.position = startPos;
			rb = GetComponent<Rigidbody>();
			
			rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 1f) * 50f;
			scores.GetComponent<ScoreHandler>().UpdateRed(1);
			TrailRenderer TR = GetComponent<TrailRenderer>();
			ResetTrail(TR);
			
		}
		if (transform.position.z > 180f) {
			transform.position = startPos;
			rb = GetComponent<Rigidbody>();
			
			rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, -1f) * 50f;
			scores.GetComponent<ScoreHandler>().UpdateGreen(1);
			TrailRenderer TR = GetComponent<TrailRenderer>();
			ResetTrail(TR);
		}
	}

	[Server]
	void TestRandMotion () {
		if (Random.value < 0.005f) {
			rb.velocity = new Vector3(Random.value, Random.value, Random.value) - Vector3.one * 0.5f;
		}
		rb.velocity = rb.velocity.normalized * 5f;
	}

	[Server]
	IEnumerator SendPosCoroutine (float interval) {
		while (true) {
			//physState.position = rb.position;
			//physState.vel = rb.velocity;
			yield return new WaitForSeconds(interval);
		}
	}
}
