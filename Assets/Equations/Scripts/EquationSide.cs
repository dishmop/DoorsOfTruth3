using UnityEngine;
using System.Collections;

public class EquationSide : Expression
{
	public bool LHS;

	new public void LateUpdate ()
	{
		if (Children.Count > 0) {
			Children [0].ShowPositiveSign = false;
		}

		base.LateUpdate ();
	}

	public override bool CanDragOver (Expression child) // anything but a sum or be dragged over (which must be done term by term)
	{
		if (child.GetType () == typeof(Sum) || child.GetType () == typeof(Zero)) { // can't drag over zero as that's pointless
			return false;
		} else {
			return true;
		}
	}

	override public void RemoveOver (Expression child)
	{
		Children.Remove (child);
		child.Parent = null;

		Zero newzero = Instantiate (equations.zeroPrefab).GetComponent <Zero> ();
		newzero.equations = equations;
		newzero.Parent = this;
		newzero.transform.SetParent (transform.parent,false);
		Children.Add (newzero);
		newzero.transform.localPosition = new Vector3 (ChildPosx (newzero), ChildPosy (newzero), 0);
	}

	public override bool CanDragUnder (Expression child)
	{
		if (child.GetType () == typeof(Product) || child.GetType () == typeof(Zero) || child.GetType () == typeof(One)) { // can't divide by zero. dividing by 1 is pointless
			return false;
		} else {
			return true;
		}
	}

	public override void RemoveUnder (Expression child)
	{
		Children.Remove (child);
		child.Parent = null;


		One newone = Instantiate (equations.onePrefab).GetComponent <One> ();
		newone.equations = equations;
		newone.Parent = this;
		newone.transform.SetParent (transform.parent,false);
		Children.Add (newone);
	}

	public override float ChildPosx (Expression expression)
	{
		if (expression == null) {
			return transform.localPosition.x;
		}
		if (LHS) {
			return - equations.sideOffset - expression.Width + ((expression.ShowPositiveSign && expression.ExpressionSign) ? equations.signWidth : 0);
		} else { // RHS																							 						 
			return + equations.sideOffset;
		}
	}

	public override float ChildPosy (Expression expression)
	{
		if (expression == null) {
			return transform.localPosition.y;
		}

		return -expression.Height / 2;
	}

	public override float Height {
		get {
			return Children [0].Height;
		}
	}

	public override float Width {
		get {
			return Children [0].Width;
		}
	}

	protected override void Copy (Expression ret)
	{
		EquationSide ret1 = (EquationSide)ret;
		ret1.LHS = LHS;
	}

	protected override Expression ActualInstantiate ()
	{
		throw new UnityException ("Cannot create equation side on the fly");
	}
}
