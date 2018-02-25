using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour {

	public GameObject battlePanelObject;

	public GameObject[] buttonSlots = new GameObject[numItemSlots]; 
	public Interactable[] buttonInteractables = new Interactable[numItemSlots]; 
	public const int numItemSlots = 4;

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

			//buttonSlots [index].SetActive(false);
		}	
	}


	public void ShiftCursor(int shift)
	{
		
	}
}
