using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForGround : MonoBehaviour
{
	public bool isGround = false;
	
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 8) //if layer is ground
		{
			//Debug.Log("grounda basıyor");
			isGround = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 8)  //if layer is ground
		{
			//Debug	.Log("grounddan çıktı");
			isGround = false;
		}
	}
}
