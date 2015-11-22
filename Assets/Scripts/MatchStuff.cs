using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

public class MatchStuff : MonoBehaviour {
	NetworkMatch netMatch;
	NetworkManager netMgr;
	NetworkClient netClient;

	public GameObject hudToDeactivate;

	public GameObject redStart;
	public GameObject greenStart;
	public GameObject theCamera;

	bool canPress = true;
	bool alreadyLoaded = false;

	// Use this for initialization
	void Start () {
		netMatch = gameObject.AddComponent<NetworkMatch>();
		netMgr = GetComponent<NetworkManager>();
		netMgr.matchMaker = netMatch;

		netMatch.SetProgramAppID((AppID)520001);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PressCreate () {
		if (!canPress) {
			return;
		}
		canPress = false;

		CreateMatchRequest create = new CreateMatchRequest();
		create.name = "TheRoom" + Mathf.RoundToInt(100f * Random.value);
		create.size = 2;
		create.advertise = true;
		create.password = "";

		netMatch.CreateMatch(create, OnMatchCreate);
	}

	public void PressJoin () {
		if (!canPress) {
			return;
		}
		canPress = false;

		netMatch.ListMatches(0, 20, "", OnMatchList);
	}

	void OnMatchCreate (CreateMatchResponse cmr) {
		netMgr.OnMatchCreate(cmr);


		if (cmr.success) {
			/*Debug.Log("Create match succeeded");
			//matchCreated = true;
			Utility.SetAccessTokenForNetwork(cmr.networkId, new NetworkAccessToken(cmr.accessTokenString));
			NetworkServer.Listen(new MatchInfo(cmr), 9000);

			NetworkClient netClient = new NetworkClient();
			netClient.RegisterHandler(MsgType.Connect, OnConnected);
			netClient.Connect("127.0.0.1",9000);*/

			//netClient = ClientScene.ConnectLocalServer();
			//netClient.RegisterHandler(MsgType.Connect, OnConnected);


			if (!alreadyLoaded) {
				alreadyLoaded = true;
				hudToDeactivate.SetActive(false);
				theCamera.transform.position = greenStart.transform.position;
				theCamera.transform.rotation = greenStart.transform.rotation;
				greenStart.SetActive(false);
			} else {
				Debug.LogError("Level already loaded?");
			}
		} else {
			Debug.LogError ("Create match failed");
			canPress = true;
		}
	}
	public void OnMatchList(ListMatchResponse matchListResponse)
	{
		if (matchListResponse.success && matchListResponse.matches != null
		    	&& matchListResponse.matches.Count > 0) {
			int randomChoice = Mathf.FloorToInt(Random.value * matchListResponse.matches.Count);
			netMatch.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
		} else {
			Debug.LogError ("Unable to join any matches.");
		}
	}
	
	public void OnMatchJoined(JoinMatchResponse matchJoin)
	{
		netMgr.OnMatchJoined(matchJoin);
		if (matchJoin.success) {
			Debug.Log("Join match succeeded");
			/*if (matchCreated) {
				Debug.LogWarning("Match already set up, aborting...");
				canPress = true;
				return;
			}*/
			//Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
			//NetworkClient netClient = new NetworkClient();
			//netClient.RegisterHandler(MsgType.Connect, OnConnected);
			//netClient.Connect(new MatchInfo(matchJoin));
			if (!alreadyLoaded)  {
				alreadyLoaded = true;
				hudToDeactivate.SetActive(false);
				theCamera.transform.position = redStart.transform.position;
				theCamera.transform.rotation = redStart.transform.rotation;
				redStart.SetActive(false);
				NetworkServer.SpawnObjects();
			} else {
				Debug.LogError("Holy shit already loaded");
			}
		}
		else
		{
			Debug.LogError("Join match failed");
			canPress = true;
		}
	}
}
