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
	public GameObject scoreprefab;
	public Material RedMaterial;
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	public float monsterMinZ;
	public float monsterMaxZ;

	public GameObject[] allMonsters;

	PaddleState state;

	[SyncVar]
	float statex,statey,statez;
	// Use this for initialization
	void Start () {
		cam = GameObject.Find("CardboardMain").transform.FindChild("Head").gameObject;
		if((isServer && isLocalPlayer) || (!isServer && !isLocalPlayer)) {
			collide = GameObject.Find("PaddlePlaneGreen");


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
			StartCoroutine(SendPosCoroutine(0.1f));
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
			transform.position = new Vector3(statex,statey,statez);
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

			bool spawnADude = false;
			if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1
			    	&& Input.GetTouch(0).phase == TouchPhase.Began)) {
				spawnADude = true;
			}
			
			//Debug.Log (hits.Length);
			Vector3 posToGo = transform.position;
			
			foreach (RaycastHit hit in hits) {
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
				} else if (spawnADude && hit.collider.name.StartsWith("Plane")) { // Walls
					CmdSpawnDude(hit.point, hit.normal);
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
		GameObject scoreboard = Instantiate(scoreprefab);
		NetworkServer.Spawn(scoreboard);

		GameObject ball = Instantiate<GameObject>(ballPrefab);
		ball.GetComponent<BallCtl> ().scores = scoreboard;
		NetworkServer.Spawn(ball);
	}

	[Command]
	void CmdSpawnDude (Vector3 pos, Vector3 normal) {
		Debug.Log("Spawning a dude! " + pos + " " + normal);
		int monsterInd = Mathf.FloorToInt(Random.value * allMonsters.Length);
		if (allMonsters.Length > 0 && pos.z < monsterMaxZ && pos.z > monsterMinZ) {
			GameObject mons = Instantiate<GameObject>(allMonsters[monsterInd]);
			mons.transform.position = pos;
			if (normal == Vector3.left) {
				mons.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
			} else if (normal == Vector3.down) {
				mons.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			} else if (normal == Vector3.right) {
				mons.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			}
			NetworkServer.Spawn(mons);
		}
	}
	
	IEnumerator SendPosCoroutine (float interval) {
		while (true) {
			yield return new WaitForSeconds(interval);
			CmdRaycastNewPos(transform.position);
		}
	}

	void OnPaddleStateChange(PaddleState ps) {
		Debug.Log (ps);
		state = ps;
	}
}
