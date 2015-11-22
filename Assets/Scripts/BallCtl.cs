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
	RBInfo physState;

	public float expires = 0f;
	public Material altBallMtl;

	[SyncVar]
	float posx,posy,posz,velx,vely,velz;

	// Use this for initialization  ((Random.value - 0.5f)<0)?1f:
	void Start () {
		Debug.Log (isClient);
		Debug.Log (isServer);
		rb = GetComponent<Rigidbody>();
		if (isServer) {
			InitState();
			StartCoroutine(SendPosCoroutine(0.1f));

			if (expires > 0f) {
				GetComponent<MeshRenderer>().material = altBallMtl;
				StartCoroutine(KillMeIn(expires));
			}
		}
		rb.isKinematic = false;
	}

	[Server]
	void InitState () {
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
			if(physState.position != new Vector3(posx,posy,posz)) {
				physState.position = new Vector3(posx,posy,posz);
				physState.vel = new Vector3(velx,vely,velz);
				rb.position = physState.position;
				rb.velocity = physState.vel;
			}

		}
	}

	[Server]
	void HandleAllMotion () {
		if (transform.position.z < -25f) {
			transform.position = new Vector3(0f,0f,0f);
			rb = GetComponent<Rigidbody>();
			
			rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, 1f) * 50f;
			scores.GetComponent<ScoreHandler>().UpdateRed(1);
			TrailRenderer TR = GetComponent<TrailRenderer>();
			ResetTrail(TR);
			
		}
		if (transform.position.z > 180f) {
			transform.position = new Vector3(0f,0f,155f);
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
		while (enabled) {
			physState.position = rb.position;
			
			posx = physState.position.x;
			posy = physState.position.y;
			posz = physState.position.z;

			physState.vel = rb.velocity;
			
			velx = physState.vel.x;
			vely = physState.vel.y;
			velz = physState.vel.z;

			yield return new WaitForSeconds(interval);
		}
	}

	IEnumerator KillMeIn (float time) {
		yield return new WaitForSeconds(time);
		this.enabled = false;
		if (isServer) {
			NetworkServer.UnSpawn(gameObject);
		}
		Destroy (gameObject);
	}
}
