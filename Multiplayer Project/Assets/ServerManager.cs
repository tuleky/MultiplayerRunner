using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour
{
	[ClientRpc]
   public void RpcRestartGame()
	{
		NetworkManager.singleton.ServerChangeScene("SampleScene");
	}
}
