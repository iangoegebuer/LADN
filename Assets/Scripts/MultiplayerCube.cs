using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MultiplayerCube : NetworkBehaviour {
	public float SPEED_AROUND = 0.05f;
	struct CubeState {
		public float x;
		public float y;
	}
	
	[SyncVar] CubeState state;

	void Awake () {
		InitState();
	}

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
	}

	void HandleInput ()
	{
		if (Input.GetKey (KeyCode.LeftArrow)) {
			state.x -= SPEED_AROUND;
			SyncState ();
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			state.x += SPEED_AROUND;
			SyncState ();
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			state.y -= SPEED_AROUND;
			SyncState ();
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			state.y += SPEED_AROUND;
			SyncState ();
		}
	}
}
