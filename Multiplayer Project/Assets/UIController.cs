using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class UIController : NetworkBehaviour
{
	private ServerManager serverManager;

	private void Awake()
	{
		serverManager = GameObject.FindWithTag("Manager").GetComponent<ServerManager>();
	}

	[Command]
	public void CmdYenidenBasla()
	{
		serverManager.RpcRestartGame();
		//SceneManager.LoadScene(0);
		Debug.Log("Yeniden başla");
	}
}
