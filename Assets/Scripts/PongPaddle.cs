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
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	[SyncVar]
	PaddleState state;
	// Use this for initialization
	void Start () {
		cam = GameObject.Find("CardboardMain").transform.FindChild("Head").gameObject;
		if(isServer) {
			collide = GameObject.Find("PaddlePlaneGreen");

			GameObject ball = Instantiate(ballPrefab);
			NetworkServer.Spawn(ball);

		} else {
			collide = GameObject.Find("PaddlePlaneRed");
		}

		InitState();
	}

	[Server]
	void InitState () {
		state.pos.x = 0f;
		state.pos.y = 0f;
		state.pos.z = collide.transform.position.z;
	}

	void SyncState () {
		transform.position = state.pos;
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			RaycastHit[] hits;
			hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 100.0F);
			
			//Debug.Log (hits.Length);
			Vector3 posToGo = state.pos;
			
			for (int i = 0; i < hits.Length; i++) {
				RaycastHit hit = hits [i];
				
				if(hit.collider.gameObject == collide) {
					Debug.Log(hit.collider.name + " X: " + state.pos.x + " Y: " + state.pos.y);
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
			CmdRaycastNewPos(posToGo);
		}
		SyncState();
	}

	[Command]
	void CmdRaycastNewPos (Vector3 posToSet) {
		state.pos = posToSet;
	}
}
