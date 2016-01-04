using UnityEngine;
using System.Collections;

public class Zero : Expression
{

	Character character;
	
	new public void Start ()
	{
		base.Start ();
	}
	
	new public void LateUpdate ()
	{
		if (character == null) {
			character = Instantiate (Equations.CharacterPrefab).GetComponent<Character> ();
			character.transform.SetParent (transform.parent,false);	
			character.transform.position = transform.position;
			character.character = "0";
		}


		
		character.transform.localPosition = new Vector3(Positionx,Positiony,0)+ new Vector3 ((ShowingSign ? Equations.SignWidth : 0),0,0);

		base.LateUpdate ();
	}

	new public void OnDestroy ()
	{
		if (character != null)
			Destroy (character.gameObject);
		base.OnDestroy ();
	}

	public override bool CanDragOver (Expression child)
	{
		return false;
	}
	
	public override bool CanDragUnder (Expression child)
	{
		return false;
	}
	
	public override float ChildPosx (Expression expression)
	{
		return 0; // no children so irrelevant
	}
	
	public override float ChildPosy (Expression expression)
	{
		return 0;
	}
	
	public override float Height {
		get {
			return 50f;
		}
	}
	
	public override float Width {
		get {
			return (ShowPositiveSign || !ExpressionSign) ? 50f + Equations.SignWidth : 50f;
		}
	}
	
	public override void RemoveOver (Expression child)
	{
	}
	
	public override void RemoveUnder (Expression child)
	{
	}

	protected override Expression ActualInstantiate ()
	{
		Expression ret = Instantiate (Equations.ZeroPrefab).GetComponent<Expression> ();
		ret.transform.SetParent (Equations.Canvas.transform,false);
		return ret;

	}

	public override void BringToFront ()
	{
		character.transform.SetAsLastSibling ();
		base.BringToFront ();

	}

}