using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerCube : NetworkBehaviour {
	public float SPEED_AROUND = 0.05f;
	struct CubeState {
		public float x;
		public float y;
	}
	
	[SyncVar]
	CubeState state;

	void Awake () {
		InitState();
	}

	[Server]
	void InitState () {
		state.x = 0f;
		state.y = 0f;
	}

	void SyncState () {
		transform.position = new Vector3(state.x, state.y, 0f);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			HandleInput();
		}
		SyncState();
	}

	void HandleInput ()
	{
		float dx = 0;
		float dy = 0;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			dx -= SPEED_AROUND;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			dx += SPEED_AROUND;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			dy -= SPEED_AROUND;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			dy += SPEED_AROUND;
		}
		CmdMoveState(dx, dy);
	}

	[Command]
	void CmdMoveState (float dx, float dy) {
		state.x += dx;
		state.y += dy;
	}
}
