using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;

	private Rigidbody2D rigidBody;

	private bool acceptInput = true;

	private bool dialogOpened;

	public GameObject talkBox;

	private UnityEngine.UI.Text talkBoxText;

	private bool fireNextLine = true;
	private bool lineCompleted = false;
	private Coroutine currentWordByWord = null;

	private List<Reaction> reactions = new List<Reaction>();


	public Direction facing = Direction.West;

	public Sprite spriteWest;
	public Sprite spriteEast;
	public Sprite spriteNorth;
	public Sprite spriteSouth;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();

		rigidBody = GetComponent<Rigidbody2D>();

		if (talkBox == null)
		{
			return;
		}

		var talkBoxTextTransform = talkBox.transform.Find("TalkBoxText");

		if (talkBoxTextTransform == null)
		{
			return;
		}

		talkBoxText = talkBoxTextTransform.GetComponent<Text> ();
 
	}

	void Update ()
	{

		if (Input.GetKeyDown ("escape")) 
		{
			Application.Quit ();
		}

		if (!acceptInput)
		{
			return;
		}

		if (dialogOpened)
		{
			HandleDialog ();
		} 
		else
		{
			HandleMove ();
		}
	}

	public void HandleDialog()
	{
		if (fireNextLine && reactions.Count > 0)
		{
			fireNextLine = false;
			currentWordByWord = StartCoroutine (WordByWord (reactions [0].displayedText));
			return;
		}
   
		if(Input.GetKeyDown("space"))
		{   	
			if (lineCompleted) 
			{
				reactions.RemoveAt (0);
				fireNextLine = true;
				if (reactions.Count == 0) 
				{
					reactions.Clear ();
					StartCoroutine (CloseDialog ());
					return;
				}
			} 
			else 
			{
				StopCoroutine (currentWordByWord);
				talkBoxText.text = reactions [0].displayedText;
				lineCompleted = true;
			}

		}		
	}

	public void HandleMove()
	{
		if(Input.GetKeyDown("space"))
		{ 
			if (ExamineFront ())
			{
				return;
			}
		}

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);

		rigidBody.velocity = movement * speed;

		ChangeFacing (movement.normalized);
 
	}

	public void ChangeFacing(Vector2 newDirection)
	{

		if (newDirection.x == 0 && newDirection.y == 0)
		{
			return;
		}

		if (newDirection.x == 1 && newDirection.y == 1)
		{
			switch (facing)
			{
				case Direction.South:
					facing = Direction.North;
					break;
				case Direction.West:
					facing = Direction.East;
					break;
			}
		}

		if (newDirection.x == -1 && newDirection.y == -1)
		{
			switch (facing)
			{
				case Direction.East:
					facing = Direction.West;
					break;
				case Direction.North:
					facing = Direction.South;
					break;
			}
		}

		if (newDirection.x == 0)
		{
			if (newDirection.y == 1)
			{
				facing = Direction.North;
			} 
			else
			{
				facing = Direction.South;
			}
		} 

		if (newDirection.y == 0)
		{
			if (newDirection.x == 1)
			{
				facing = Direction.East;
			} 
			else
			{
				facing = Direction.West;
			}
		} 

		switch (facing)
		{
			case Direction.North:
				spriteRenderer.sprite = spriteNorth;
				break;
			case Direction.East:
				spriteRenderer.sprite = spriteEast;
				break;
			case Direction.South:
				spriteRenderer.sprite = spriteSouth;
				break;
			case Direction.West:
				spriteRenderer.sprite = spriteWest;
				break;
		}
	}


	public IEnumerator OpenDialog()
	{	
		talkBoxText.text = string.Empty;

		talkBox.SetActive (true); 

		while (talkBox.transform.localScale.x < 1)
		{
			if (talkBox.transform.localScale.x >= 0.9)
			{
				talkBox.transform.localScale = new Vector2(1f , 1f);
				break;
			}

			talkBox.transform.localScale = new Vector2(talkBox.transform.localScale.x + 0.1f , 1f);

			yield return new WaitForSeconds(0.01f);
		}
		 
		acceptInput = true;
		dialogOpened = true;
	}

	public bool ExamineFront()
	{

		Vector2 fwd = Vector2.zero;

		switch (facing)
		{

			case(Direction.North):
				fwd = new Vector2 (0, 1);
				break;

			case(Direction.East):
				fwd = new Vector2 (1, 0);
				break;

			case(Direction.South):
				fwd = new Vector2 (0, -1);
				break;

			case(Direction.West):
				fwd = new Vector2 (-1, 0);
				break;

		}

		var layerMask = 1 << LayerMask.NameToLayer("Interactable");

		RaycastHit2D hit = Physics2D.Raycast (transform.position, fwd, 2, layerMask);  

		if (!hit)
		{
			return false;
		}

		Interactable interactable = hit.transform.gameObject.GetComponent<Interactable> ();

		if (interactable == null) 
		{
			return false;
		}

		acceptInput = false;


		ReactionCollection reactionCollection = interactable.Interact ();

		reactions.Clear (); 

		reactions.AddRange (reactionCollection.reactions);

		StartCoroutine(OpenDialog());
		rigidBody.velocity = Vector2.zero;		

		return true;
	}

	public IEnumerator CloseDialog()
	{

		while (talkBox.transform.localScale.x > 0)
		{

			if (talkBox.transform.localScale.x <= 0.1)
			{
				talkBox.transform.localScale = new Vector2(0f , 1f);
				break;
			}
				
			talkBox.transform.localScale = new Vector2(talkBox.transform.localScale.x - 0.1f , 1f);

			yield return new WaitForSeconds(0.01f);
		} 

		dialogOpened = false; 
		talkBox.SetActive (false);
		acceptInput = true;
	}

	public IEnumerator WordByWord(string line)
	{
		lineCompleted = false;

		var words = line.Split (' ');

		for (int i = 0; i < words.Length; i++)
		{
			if (i == 0)
			{
				talkBoxText.text = words [i];
			} 
			else
			{
				talkBoxText.text += (" " + words [i]);
			}

			yield return new WaitForSeconds(0.1f);
		}

		lineCompleted = true;
	}

}
