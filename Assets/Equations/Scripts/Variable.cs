using UnityEngine;
using System.Collections;

public class Variable : Expression
{
	public string Symbol;
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
			character.character = Symbol;
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

	override public void RemoveOver (Expression child)
	{
	}

	public override bool CanDragUnder (Expression child)
	{
		return false;
	}

	public override void RemoveUnder (Expression child)
	{
	}

	public override float ChildPosx (Expression expression)
	{
		return 0; // no children so irrelevant
	}

	public override float ChildPosy (Expression expression)
	{
		return 0;
	}

	public override float Width {
		get {
			return (ShowPositiveSign || !ExpressionSign) ? 50f + Equations.SignWidth : 50f;
		}
	}

	public override float Height {
		get {
			return 50f;
		}
	}

	protected override void Copy (Expression ret)
	{
		Variable ret1 = (Variable)ret;
		ret1.Symbol = Symbol;
	}

	protected override Expression ActualInstantiate ()
	{
		Expression ret = Instantiate (Equations.VariablePrefab).GetComponent<Expression> ();
		ret.transform.SetParent (Equations.Canvas.transform,false);
		((Variable)ret).Symbol = Symbol;
		return ret;
	}

	public override void BringToFront ()
	{
		character.transform.SetAsLastSibling ();
		base.BringToFront ();
	}
}
