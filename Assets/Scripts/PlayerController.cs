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

	private bool writingInProgress = false;
	private List<string> lines = new List<string>();

	void Awake ()
	{
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
		if (!writingInProgress && lines.Count > 0)
		{
			writingInProgress = true;
			StartCoroutine (WordByWord (lines[0]));
			return;
		}
   
		if(Input.GetKeyDown("space"))
		{   
			writingInProgress = false;
			talkBoxText.text = lines [0]; 
			lines.RemoveAt (0);

			if (lines.Count == 0)
			{
				lines.Clear ();
				StartCoroutine (CloseDialog ());
				return;
			}

		}		
	}

	public void HandleMove()
	{
		if(Input.GetKeyDown("space"))
		{ 
			acceptInput = false;

			lines.Clear ();
			lines.Add ("Lorem ipsum aaaa dolor amat crocodilian telegraphic Panka");
			lines.Add ("Crocodilum ipsum aaa bbb ccc ddd eee");
			lines.Add ("Trallalalaaa laaa");

			StartCoroutine(StartDialog());
			 
			rigidBody.velocity = Vector2.zero;
			return;
		}

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);

		rigidBody.velocity = movement * speed;
	}


	public IEnumerator StartDialog()
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
	}

}
