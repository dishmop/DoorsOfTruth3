using UnityEngine;
using System.Collections;

public class Variable : Expression
{
	public string Symbol;
	Character character;

	public Color colour;

	new public void Start ()
	{
		base.Start ();
	}

	new public void LateUpdate ()
	{
		if (character == null) {
			character = Instantiate (equations.characterPrefab).GetComponent<Character> ();
			character.equations = equations;
			character.transform.SetParent (transform.parent,false);	
			character.transform.position = transform.position;
			character.character = Symbol;
			character.SetColour(colour);
		}

		character.transform.localPosition = new Vector3(Positionx,Positiony,0)+ new Vector3 ((ShowingSign ? equations.signWidth	 : 0),0,0);

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
			return (ShowPositiveSign || !ExpressionSign) ? 50f + equations.signWidth : 50f;
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
		ret1.colour = colour;
	}

	protected override Expression ActualInstantiate ()
	{
		Expression ret = Instantiate (equations.variablePrefab).GetComponent<Expression> ();
		ret.transform.SetParent (equations.transform,false);
		((Variable)ret).Symbol = Symbol;
		((Variable)ret).colour = colour;
		return ret;
	}

	public override void BringToFront ()
	{
		character.transform.SetAsLastSibling ();
		base.BringToFront ();
	}
}
