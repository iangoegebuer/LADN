using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PongPaddle : NetworkBehaviour {
	struct PaddleState {
		public Vector3 pos;
	}

	public GameObject cam;
	public GameObject collide;
	public GameObject ballPrefab;
	public Material RedMaterial;
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	PaddleState state;

	[SyncVar]
	int statex,statey,statez;
	// Use this for initialization
	void Start () {
		cam = GameObject.Find("CardboardMain").transform.FindChild("Head").gameObject;
		if((isServer && isLocalPlayer) || (!isServer && !isLocalPlayer)) {
			collide = GameObject.Find("PaddlePlaneGreen");

			if (isServer) {
				CmdSpawnBall();
			}
		} else {
			collide = GameObject.Find("PaddlePlaneRed");
			GetComponent<MeshRenderer>().material = RedMaterial;
			if (isServer) {
				CmdSpawnBall();
			}
		}

		if (isServer) {
			InitState();
		}

		if (isLocalPlayer) {
			StartCoroutine(SendPosCoroutine(0.5f));
		}
	}

	[Server]
	void InitState () {
		state.pos.x = 0f;
		state.pos.y = 0f;
		state.pos.z = collide.transform.position.z;
	}

	void SyncState () {
		if (!isLocalPlayer) {
			transform.position = state.pos;
		}
		Vector3 vec = transform.position;
		vec.z = collide.transform.position.z;
		transform.position = vec;
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			RaycastHit[] hits;
			hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 100.0F);
			
			//Debug.Log (hits.Length);
			Vector3 posToGo = transform.position;
			
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hit = hits [i];
				
				if(hit.collider.gameObject == collide) {
					//Debug.Log(hit.collider.name + " X: " + state.pos.x + " Y: " + state.pos.y);
					if(hit.point.x > minX && hit.point.x < maxX)
						posToGo.x = hit.point.x;
					else if( hit.point.x < minX)
						posToGo.x = minX;
					else posToGo.x = maxX;
					if(hit.point.y > minY && hit.point.y < maxY)
						posToGo.y = hit.point.y;
					else if( hit.point.y < minY)
						posToGo.y = minY;
					else posToGo.y = maxY;
				}
			}
			Debug.DrawLine(cam.transform.position, posToGo);
			transform.position = posToGo;
		}
		SyncState();
	}

	[Command]
	void CmdRaycastNewPos (Vector3 posToSet) {
		state.pos = posToSet;
		statex = state.pos.x;
		statey = state.pos.y;
		statez = state.pos.z;
	}

	[Command]
	void CmdSpawnBall () {
		GameObject ball = Instantiate(ballPrefab);
		ball.GetComponent<BallCtl>().scores = GameObject.Find("Scoreboard");
		NetworkServer.Spawn(ball);
	}
	
	IEnumerator SendPosCoroutine (float interval) {
		while (true) {
			yield return new WaitForSeconds(interval);
			CmdRaycastNewPos(transform.position);
		}
	}
}
