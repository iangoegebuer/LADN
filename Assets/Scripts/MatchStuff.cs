using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

public class MatchStuff : MonoBehaviour {
	NetworkMatch netMatch;
	NetworkManager netMgr;
	bool canPress = true;

	// Use this for initialization
	void Start () {
		netMatch = gameObject.AddComponent<NetworkMatch>();
		netMgr = GetComponent<NetworkManager>();

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
		if (cmr.success) {
			Debug.Log("Create match succeeded");
			//matchCreated = true;
			Utility.SetAccessTokenForNetwork(cmr.networkId, new NetworkAccessToken(cmr.accessTokenString));
			NetworkServer.Listen(new MatchInfo(cmr), 9000);
			Application.LoadLevel("pongz");
		} else {
			Debug.LogError ("Create match failed");
			canPress = true;
		}
	}
	public void OnMatchList(ListMatchResponse matchListResponse)
	{
		if (matchListResponse.success && matchListResponse.matches != null) {
			int randomChoice = Mathf.FloorToInt(Random.value * matchListResponse.matches.Count);
			netMatch.JoinMatch(matchListResponse.matches[0].networkId, "", OnMatchJoined);
		}
	}
	
	public void OnMatchJoined(JoinMatchResponse matchJoin)
	{
		if (matchJoin.success) {
			Debug.Log("Join match succeeded");
			/*if (matchCreated) {
				Debug.LogWarning("Match already set up, aborting...");
				canPress = true;
				return;
			}*/
			Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
			NetworkClient myClient = new NetworkClient();
			myClient.RegisterHandler(MsgType.Connect, OnConnected);
			myClient.Connect(new MatchInfo(matchJoin));
		}
		else
		{
			Debug.LogError("Join match failed");
			canPress = true;
		}
	}
	
	public void OnConnected(NetworkMessage msg)
	{
		Debug.Log("Connected!");
	}
}
