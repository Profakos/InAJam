using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;

	private Rigidbody2D rigidBody;

	private bool acceptInput = false;

	private bool dialogOpened;

	public GameObject talkBox;
	private GameObject nameBox;
	private GameObject choiceBox;
	private GameObject imageBox;

	public Inventory inventory;

	private UnityEngine.UI.Text talkBoxText;
	private UnityEngine.UI.Text nameBoxText;
	private UnityEngine.UI.Text choiceBoxText;

	private UnityEngine.UI.Image talkImage;

	private bool fireNextLine = true;
	private bool lineCompleted = false;
	private Coroutine currentWordByWord = null;
	private int choiceIndex = 0;

	public List<GameObject> reactionActiavateables = new List<GameObject>();

	public List<ReactionCollection> cutscenesCollection = new List<ReactionCollection>();
	private List<Reaction> queuedReactions = new List<Reaction>();

	private Dictionary<string, string> gameState = new Dictionary<string, string> (); 

	public Direction facing = Direction.West;

	public Sprite spriteWest;
	public Sprite spriteEast;
	public Sprite spriteNorth;
	public Sprite spriteSouth;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	void Awake ()
	{
		Cursor.visible = false;

		spriteRenderer = GetComponent<SpriteRenderer> ();

		rigidBody = GetComponent<Rigidbody2D>();

		if (talkBox == null)
		{
			return;
		}

		var textImageTransform = talkBox.transform.Find("TextImage");

		if (textImageTransform != null)
		{		
			imageBox = textImageTransform.gameObject;
			talkImage = textImageTransform.GetComponent<Image> ();
		}

		var talkBoxTextTransform = talkBox.transform.Find("TalkBoxText");

		if (talkBoxTextTransform != null)
		{		
			talkBoxText = talkBoxTextTransform.GetComponent<Text> ();
		}

		var nameBoxTransform = talkBox.transform.Find ("NameBox");

		nameBox = nameBoxTransform.gameObject;

		var nameBoxTextTransform = nameBox.transform.Find("NameBoxText");

		if (nameBoxTextTransform != null)
		{
			nameBoxText = nameBoxTextTransform.GetComponent<Text> ();
		}


		var choiceBoxTransform = talkBox.transform.Find ("ChoiceBox");

		choiceBox = choiceBoxTransform.gameObject;

		var choiceBoxTextTransform = choiceBox.transform.Find("ChoiceBoxText");

		if (choiceBoxTextTransform != null)
		{
			choiceBoxText = choiceBoxTextTransform.GetComponent<Text> ();
		}

		var inventoryComponent = transform.GetComponent<Inventory> ();

		if (inventoryComponent != null) 
		{
			inventory = inventoryComponent;
		}
	}

	void Start()
	{
		InsertCutscene ("START", 0);
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

	public void InsertCutscene(string name, int insertLocation)
	{
		var cutScene = cutscenesCollection.Find (m => m.reactionCollectionName == name);

		if (cutScene != null)
		{
			queuedReactions.InsertRange (insertLocation, cutScene.reactions);
			StartCoroutine (OpenDialog ());
		} else
		{
			acceptInput = false;
		}
	}

	public void HandleDialog()
	{
		if (fireNextLine && queuedReactions.Count > 0)
		{
			fireNextLine = false;
			currentWordByWord = StartCoroutine (WordByWord (queuedReactions [0].text));

			if (!string.IsNullOrEmpty (queuedReactions [0].displayName))
			{
				nameBox.SetActive (true);
				nameBoxText.text = queuedReactions [0].displayName;
			} else
			{
				nameBox.SetActive (false);
			}

			if (queuedReactions [0].sprite != null)
			{
				imageBox.SetActive (true);
				talkImage.sprite = queuedReactions [0].sprite;
			} 
			else
			{
				imageBox.SetActive (false);
			}

			if (queuedReactions [0].objectsToDisable.Count > 0) 
			{
				foreach (string name in queuedReactions[0].objectsToDisable) 
				{
					var gO = reactionActiavateables.Find (r => r.name == name);

					if (gO == null)
					{
						continue;
					}

					gO.SetActive (false);
				}
			}

			if (queuedReactions [0].objectsToEnable.Count > 0) 
			{
				foreach (string name in queuedReactions[0].objectsToEnable) 
				{
					var gO = reactionActiavateables.Find (r => r.name == name);

					if (gO == null)
					{
						continue;
					}

					gO.SetActive (true);
				}
			}

			if (queuedReactions [0].setGameState.Count != 0) 
			{
				foreach (var gS in queuedReactions [0].setGameState) 
				{
					if (gameState.ContainsKey (gS.name)) 
					{
						gameState [gS.name] = gS.value;
					} else 
					{
						gameState.Add (gS.name, gS.value);
					}
				}			
			}

			if (queuedReactions [0].addItem.Count != 0) 
			{
				foreach (var item in queuedReactions[0].addItem) 
				{
					inventory.AddItem (item);
				}
			}

			if (queuedReactions [0].removeItem.Count != 0) 
			{
				foreach (var item in queuedReactions[0].removeItem) 
				{
					inventory.RemoveItem (item);
				}
			}

			return;
		}

		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			if (queuedReactions [0].options.Count != 0)
			{
				choiceIndex--;

				if (choiceIndex < 0)
				{
					choiceIndex = queuedReactions [0].options.Count - 1;
				}

				DisplayOptions ();
			}
			return;
		}

		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			if (queuedReactions [0].options.Count != 0)
			{
				choiceIndex++;

				if (choiceIndex >= queuedReactions [0].options.Count)
				{
					choiceIndex = 0;
				}

				DisplayOptions ();
			}
			return;
		}

   
		if(Input.GetKeyDown("space"))
		{   	
			if (lineCompleted) 
			{
				if (queuedReactions [0].options.Count > 0)
				{
					choiceBox.SetActive (false);
					var selectedOption = queuedReactions [0].options [choiceIndex];
					InsertCutscene (selectedOption, 1);
				}

				queuedReactions.RemoveAt (0);
				fireNextLine = true;
				if (queuedReactions.Count == 0) 
				{
					queuedReactions.Clear ();
					StartCoroutine (CloseDialog ());
					return;
				}
			} 
			else 
			{
				StopCoroutine (currentWordByWord);
				talkBoxText.text = queuedReactions [0].text;
				FinishLine ();
			}

		}		
	}

	public void FinishLine()
	{
		var options = queuedReactions [0].options;

		if (options != null && options.Count > 0)
		{
			choiceIndex = 0;
			DisplayOptions ();
		}

		lineCompleted = true;
	}

	public void DisplayOptions()
	{
		choiceBox.SetActive (true);

		var currentLine = 0;

		var displayText = string.Empty;

		foreach (var choice in queuedReactions[0].options)
		{ 
			if (choiceIndex == currentLine)
			{
				displayText += ">";
			}

			displayText += choice + "\n";
			currentLine++;
		}

		choiceBoxText.text = displayText;
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

		nameBox.SetActive (false);

		choiceBox.SetActive (false);

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

		ReactionCollection reactionCollection = interactable.Interact (gameState);

		if (reactionCollection.reactions.Count == 0) 
		{
			return false;
		}

		acceptInput = false;

		queuedReactions.Clear (); 

		queuedReactions.AddRange (reactionCollection.reactions);

		StartCoroutine(OpenDialog());
		rigidBody.velocity = Vector2.zero;		

		return true;
	}

	public IEnumerator CloseDialog()
	{
		nameBox.SetActive (false);
		choiceBox.SetActive (false);
		imageBox.SetActive (false);

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

		FinishLine ();
	}

}
