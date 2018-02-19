using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public Image[] itemImages = new Image[numItemSlots];
	public Item[] items = new Item[numItemSlots];
	public const int numItemSlots = 4;

	public void Awake()
	{
		var inventoryItem = GameObject.Find ("Inventory");

		if (inventoryItem == null) 
		{
			return;
		}

		FindImageObject(inventoryItem.transform, "Slot0/Image", 0);
		FindImageObject(inventoryItem.transform, "Slot1/Image", 1);
		FindImageObject(inventoryItem.transform, "Slot2/Image", 2);
		FindImageObject(inventoryItem.transform, "Slot3/Image", 3);

		items [0] = null;
		items [1] = null;
		items [2] = null;
		items [3] = null;

	}

	public void FindImageObject(Transform target, string name, int index)
	{		
		var slotTransform = target.Find (name);

		if (slotTransform) 
		{
			var imageScript = slotTransform.GetComponent<Image> ();

			if (imageScript != null) 
			{
				itemImages [index] = imageScript;
				itemImages [index].enabled = false;
			}
		}	
	}

	public void AddItem(Item itemToAdd)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] == null)
			{
				items[i] = itemToAdd;
				itemImages[i].sprite = itemToAdd.sprite;
				itemImages[i].enabled = true;
				return;
			}
		}
	}
	public void RemoveItem (string itemToRemove)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items [i] == null) 
			{
				continue;
			}

			if (items[i].name == itemToRemove)
			{
				items[i] = null;
				itemImages[i].sprite = null;
				itemImages[i].enabled = false;
				return;
			}
		}
	} 

}

[System.Serializable]
public class Item
{
	public Sprite sprite;
	public string name;
}
 
