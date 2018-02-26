using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour {

	public GameObject battlePanelObject;

	public GameObject[] buttonSlots = new GameObject[numItemSlots]; 
	public Interactable[] buttonInteractables = new Interactable[numItemSlots];  
	public GameObject[] buttonCursors = new GameObject[numItemSlots]; 

	public const int numItemSlots = 4;

	public int cursorIndex = 0;

	public void Awake()
	{
		battlePanelObject = GameObject.Find ("Canvas/BattlePanel");

		if (battlePanelObject == null) 
		{
			return;
		}

		FindBattleButton(battlePanelObject.transform, "BattlePanelButton0", 0);
		FindBattleButton(battlePanelObject.transform, "BattlePanelButton1", 1);
		FindBattleButton(battlePanelObject.transform, "BattlePanelButton2", 2);
		FindBattleButton(battlePanelObject.transform, "BattlePanelButton3", 3);

		for (int i = 0; i < numItemSlots; i++) 
		{
			if (buttonCursors [i] == null) 
			{
				continue;
			}

			buttonCursors [i].SetActive (cursorIndex == i);
		}
	}

	public void FindBattleButton(Transform target, string name, int index)
	{		
		var interactableTransform = target.Find (name);

		if (interactableTransform) 
		{
			buttonSlots [index] = interactableTransform.gameObject;
			var interactableScript = interactableTransform.GetComponent<Interactable> ();

			if (interactableScript != null) 
			{
				buttonInteractables [index] = interactableScript;
			}

			var cursorTransform = interactableTransform.Find ("BattleCursor");

			if (cursorTransform != null) 
			{
				buttonCursors [index] = cursorTransform.gameObject;
			}
 
		}	
	}

	public void EnableUI(bool enable)
	{
		battlePanelObject.SetActive (enable);
	}

	public void ShiftCursor(int shift)
	{
		buttonCursors [cursorIndex].SetActive (false);

		cursorIndex += shift;

		if (cursorIndex < 0) 
		{
			cursorIndex = numItemSlots - 1;
		}
		else if(cursorIndex >= numItemSlots) 
		{
			cursorIndex = 0;
		}

		buttonCursors [cursorIndex].SetActive (true);
	}

	public ReactionCollection Interact(Dictionary<string, string> gameState)
	{
		if (buttonInteractables [cursorIndex] == null) 
		{
			return null;
		}
 
		return buttonInteractables [cursorIndex].Interact(gameState);
	}
}
