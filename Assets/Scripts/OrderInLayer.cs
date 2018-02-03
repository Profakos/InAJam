using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInLayer : MonoBehaviour {

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	void Awake () 
	{

		spriteRenderer = GetComponent<SpriteRenderer> ();
	}


	//updates the draw order of the sprite
	void LateUpdate () 
	{
		spriteRenderer.sortingOrder = (int)(gameObject.transform.position.y * -10);
	}
}
