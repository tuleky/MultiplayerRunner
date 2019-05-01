using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CharacterMovement : NetworkBehaviour
{
	public float movementSpeed = 1f;
	public float jumpPower;
	public LayerMask whatIsGround;
	private bool isGrounded = true;

	[SyncVar]
	bool isWinned = false;

	private Vector2 moveVector;

	private Rigidbody2D rb2d;
	private Animator animator;
	public GameObject canvas;

	public GameObject playerCamera;

	public CheckForGround checkForGround;

	private ServerManager serverManager;	

	void Start()
    {
		rb2d = gameObject.GetComponent<Rigidbody2D>();
		animator = gameObject.GetComponent<Animator>();
		if (isLocalPlayer)
		{
			playerCamera.SetActive(true);
		}
		else
		{
			playerCamera.SetActive(false);
		}
	}

	public override void OnStartServer()
	{
		serverManager = GameObject.FindWithTag("Manager").GetComponent<ServerManager>();
	}


	bool canControl;

	private IEnumerator OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Win") && isServer)
		{
			isWinned = true;	
		}

		if (collision.gameObject.CompareTag("Mace"))  //if its colliding with mace then apply upper force, wait for a little then apply horizontal force and player cant control
		{
			rb2d.velocity = new Vector2(0f, 16f);
			yield return new WaitForSeconds(0.05f);
			rb2d.velocity = new Vector2(-15f, rb2d.velocity.y);
			canControl = false;
		}

		if (collision.gameObject.CompareTag("JumpUpgrade"))  //if its colliding with jumpupgrade then increase jumpPower
		{
			jumpPower = 2300f;
		}
	}

	[Command]
	void CmdWin(bool winState)
	{
		isWinned = winState;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("JumpUpgrade"))
		{
			jumpPower = 1500f;
		}
	}

	bool canJump = true;

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Slowdown"))
		{
			animator.speed = 0.6f;
			movementSpeed = 3f;
			canJump = false;
		}

		if (collision.gameObject.CompareTag("Slippery"))
		{
			animator.speed = 1.5f;
			movementSpeed = 11f;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Slowdown"))
		{
			animator.speed = 1f;
			movementSpeed = 8f;
			canJump = true;
		}

		if (collision.gameObject.CompareTag("Slippery"))
		{
			animator.speed = 1f;
			movementSpeed = 8f;
		}
	}

	void Jump()
	{
		if (Input.GetButtonDown("Jump") && isGrounded && canJump) //when he presses jumps and on the ground
		{
			//jump
			rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);    //stops character on y axis
			rb2d.AddForce(new Vector2(0f, jumpPower));      //adds jump force
			isGrounded = false;                             //make his state inAir
		}
	}

	void AnimatorController()
	{
		if (!isGrounded)
		{
			animator.SetBool("isJumping", true);    //change the animation state to jump
		}
		else
		{
			animator.SetBool("isJumping", false);
		}


		if (Mathf.Floor(rb2d.velocity.x) != 0)
		{
			animator.SetFloat("velocity", rb2d.velocity.x);     //change the animation state to run
		}
		else
		{
			animator.SetFloat("velocity", 0f);     //change the animation state to idle
		}
	}

	bool CheckForGround()
	{
		if (checkForGround.isGround) //if its grounded
		{
			//can jump
			isGrounded = true;
		}
		else
		{
			//if raycast cant find any object within whatIsGround layer, grounded turned to false
			isGrounded = false;
		}
		return isGrounded;
	}

	void GetInputsForMoveVector()
	{
		float xAxis = Input.GetAxisRaw("Horizontal");       //getting inputs
		moveVector = new Vector2(xAxis * movementSpeed, rb2d.velocity.y); //assigning inputs to a variable
	}

	void ApplyMovement()
	{
		rb2d.velocity = moveVector; //adding movement to our character via moveVector
	}

	[Command]
	public void CmdYenidenBasla()
	{
		serverManager.RpcRestartGame();
	}


	//void CheckForWinning()
	//{
	//	if (isWinned) //win alanına girdiyse
	//	{
	//		//servera kazandın ata
	//		//CmdDoWinning(isWinned);
	//		syncWinning = isWinned;
	//	}
	//}

	//[Command]
	//void CmdDoWinning(bool isWin)
	//{
	//	//RpcUpdateWinning(isWin);
	//	syncWinning = isWin;
	//}

	//[ClientRpc]
	//void RpcUpdateWinning(bool isWin)
	//{
	//	syncWinning = isWin;
	//}


	void ShowWin()
	{
		if (isWinned)
		{
			canvas.SetActive(true);
		}
	}

	void Update()
	{
		Debug.Log("is winned: " + GetComponent<NetworkIdentity>().netId  + isWinned);

		if (!isLocalPlayer) //if this game client is not a local player then dont execute these codes. 
			return;         //this line of code can help to play two clients in same pc without worrying about controlling both of the clients.

		if (CheckForGround())  //we check for our characters grounded state, if it is then set bool isGrounded to true, when its not grounded set its isGrounded to false
		{
			canControl = true;
		}
		Jump();
		AnimatorController();


		//ShowWin();
	}

	private void FixedUpdate()
	{
		ShowWin();

		if (isLocalPlayer) //explained in line 31
		{
			GetInputsForMoveVector();
			if (canControl)
			{
				ApplyMovement();
			}
		}
	}
}
