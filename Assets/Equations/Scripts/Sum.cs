using UnityEngine;
using System.Collections.Generic;

public class Sum : Expression
{
	Character leftBracket;
	Character rightBracket;

	new public void Start ()
	{
		leftBracket = Instantiate (equations.characterPrefab).GetComponent<Character> ();
		leftBracket.equations = equations;
		leftBracket.transform.SetParent (transform.parent,false);	

		rightBracket = Instantiate (equations.characterPrefab).GetComponent<Character> ();
		rightBracket.equations = equations;
		rightBracket.transform.SetParent (transform.parent,false);	
		
		base.Start ();
	}

	bool showBrackets = false;

	new public void LateUpdate ()
	{
		foreach (var child in Children) {
			child.ShowPositiveSign = Children.IndexOf (child) != 0;
		}

		if (imaginaryChild != null && imaginaryChildIndex == 0) {
			Children [0].ShowPositiveSign = true;
		}
		
		showBrackets = (Parent != null && Parent.GetType () == typeof(Product) && ((IsReciprocal && ((Product)Parent).NumBottom > 1) || (!IsReciprocal && ((Product)Parent).NumTop > 1))) || !ExpressionSign;

		if (showBrackets) {
			leftBracket.character = "(";
			rightBracket.character = ")";
		} else {
			leftBracket.character = "";
			rightBracket.character = "";
		}
		
		if (!ShowingSign) {
			leftBracket.transform.localPosition = new Vector3 (ScreenRect.xMin - 10f, ScreenRect.center.y - 25f, 0);
		} else {
			leftBracket.transform.localPosition = new Vector3 (ScreenRect.xMin - 10f + equations.signWidth, ScreenRect.center.y - 25f, 0);

		}
		rightBracket.transform.localPosition = new Vector3 (ScreenRect.xMax - 40f, ScreenRect.center.y - 25f, 0);
		
		base.LateUpdate ();
	}

	new public void OnDestroy ()
	{
		if (leftBracket != null)
			Destroy (leftBracket.gameObject);
		if (rightBracket != null)
			Destroy (rightBracket.gameObject);

		base.OnDestroy ();
	}

	public void Insert (int index, Expression expression)
	{
		Children.Insert (index, expression);
		expression.Parent = this;
	}

	public override void RemoveOver (Expression child)
	{
		imaginaryChild = child;
		imaginaryChildIndex = Children.IndexOf (child);

		if (!ExpressionSign) {
			child.ExpressionSign = !child.ExpressionSign;
		}

		Children.Remove (child);
		child.Parent = null;
	}

	public override void Reduce ()
	{
		if (Children.Count == 1) { // if only one item in sum, remove sum and replace with only element
			Parent.Children [Parent.Children.IndexOf (this)] = Children [0];
			Children [0].Parent = Parent;
			
			if (!ExpressionSign) {
				Children [0].ExpressionSign = !Children [0].ExpressionSign;
			}
			
			Children.RemoveAt (0);
			
			Destroy (gameObject);
		}
	}

	public override bool CanDragOver (Expression child)
	{
		if (Parent == null) {
			return false;
		}

		if (!Children.Contains (child)) {
			return false;
		}

		if (Parent.GetType () == typeof(EquationSide)) { // top level sum, so we can drag any child over
			return true;
		} else {
			return false;
		}
	}

	public override bool CanDragUnder (Expression child)
	{
		return false;
	}

	public override void RemoveUnder (Expression child)
	{
		// can't, so irrelevant
	}

	public override float ChildPosx (Expression expression)
	{
		if (expression == null) {
			throw new UnityException ();
		}

		int index;

		if (expression == imaginaryChild) {
			index = imaginaryChildIndex;
		} else {
			index = Children.IndexOf (expression);
		}


		if (index == imaginaryChildIndex && expression != imaginaryChild && imaginaryChild != null) {
			return ChildPosx (imaginaryChild) + ImaginaryChildWidth ();
		} else {
			if (index == 0) {
				return (ShowingSign ? equations.signWidth + Positionx : Positionx) + (showBrackets ? 20f : 0f);
			} else {
				return ChildPosx (Children [index - 1]) + Children [index - 1].Width;
			}
		}
	}

	public override float ChildPosy (Expression expression)
	{
		if (expression == null) {
			throw new UnityException ();
		}

		float extraheight = 0;
		if (expression.Height < Height) {
			extraheight = (Height - expression.Height) / 2;
		}

		return Positiony + extraheight;
	}

	public override float Width {
		get {
			float width = ShowingSign ? 35f : 0;

			for (int i =0; i<Children.Count; i++) {
				width += Children [i].Width;
			}

			if (imaginaryChild != null) {
				width += ImaginaryChildWidth ();
			}

			if (showBrackets)
				width += 40f;

			return width;
		}
	}

	public override float Height {
		get {
			float height = 0;
			
			for (int i =0; i<Children.Count; i++) {
				if (Children [i].Height > height) {
					height = Children [i].Height;
				}
			}

			if (imaginaryChild != null && imaginaryChild.Height > height) {
				height = imaginaryChild.Height;
			}
			
			return height;
		}
	}

	protected override Expression ActualInstantiate ()
	{
		Expression ret = Instantiate (equations.sumPrefab).GetComponent<Expression> ();
		ret.equations = equations;
		ret.transform.SetParent (equations.transform,false);
		return ret;

	}

	protected override float ImaginaryChildWidth ()
	{
		if (imaginaryChild == null)
			return 0;

		float imaginaryChildWith = imaginaryChild.Width;

		if (imaginaryChild.ExpressionSign && imaginaryChildIndex == 0) {
			imaginaryChildWith -= equations.signWidth;
		}

		float diff = imaginaryChild.Positiony - ChildPosy (imaginaryChild);

		if (diff > imaginaryChild.Height) {
			return 10f;
		} else {
			return imaginaryChildWith;
		}
	}

	public override void BringToFront ()
	{
		leftBracket.transform.SetAsLastSibling ();
		rightBracket.transform.SetAsLastSibling ();

		base.BringToFront ();
	}
}
