using UnityEngine;
using System.Collections.Generic;

public class Product : Expression
{
	
	new public void Start ()
	{
		base.Start ();
	}

	new public void LateUpdate ()
	{
		foreach (var child in Children) {
			child.ShowPositiveSign = false;

			if (!child.ExpressionSign) { // absorb minus signs into overall product
				child.ExpressionSign = true;
				ExpressionSign = !ExpressionSign;
			}
		}

		base.LateUpdate ();
	}

	new public void OnDestroy ()
	{
		base.OnDestroy ();
	}
	
	public void Insert (int index, Expression expression)
	{
		if (!expression.IsReciprocal && Children.Count > 0 && Children [0].GetType () == typeof(One)) {
			Destroy (Children [0].gameObject);
			Children [0] = expression;
		} else {
			Children.Insert (index, expression);
		}
		expression.Parent = this;
	}

	public override void RemoveOver (Expression child)
	{
		// can't, so irrelevant
	}
	
	public override bool CanDragOver (Expression child)
	{
		return false;
	}
	
	public override bool CanDragUnder (Expression child)
	{
		if (Parent == null) {
			return false;
		}
		
		if (!Children.Contains (child)) {
			return false;
		}
		
		if (Parent.GetType () == typeof(EquationSide) && child.GetType () != typeof(One)) { // top level product, so we can drag any child under
			return true;
		} else {
			return false;
		}
	}
	
	public override void RemoveUnder (Expression child)
	{
		imaginaryChild = child;
		imaginaryChildIndex = Children.IndexOf (child);

		Children.Remove (child);
		child.Parent = null;

		if (NumTop == 0) {
			// create a new one to go on top
			One newone = Instantiate (Equations.OnePrefab).GetComponent <One> ();
			newone.Parent = this;
			newone.transform.SetParent (transform.parent,false);
			Children.Insert (0, newone);
			
			
			
		} 
	}

	public override void Reduce ()
	{
		if (Children.Count == 1 && Children [0].IsReciprocal == false) { // if only one item on top, remove product and replace with only element
			Parent.Children [Parent.Children.IndexOf (this)] = Children [0];
			Children [0].Parent = Parent;
			
			if (!ExpressionSign) {
				Children [0].ExpressionSign = !Children [0].ExpressionSign;
			}
			
			Children.RemoveAt (0);
			
			Destroy (gameObject);
		} else if (NumTop == 0) {
			// create a new one to go on top
			One newone = Instantiate (Equations.OnePrefab).GetComponent <One> ();
			newone.Parent = this;
			newone.transform.SetParent (transform.parent,false);
			Children.Insert (0, newone);

		}
	}

	public int NumTop { // number of non-reciprocal elements
		get {
			int num = 0;
			foreach (var child in Children) {
				if (!child.IsReciprocal) {
					num++;
				}
			}

			return num;
		}
	}

	public int NumBottom {
		get { return Children.Count - NumTop; }
	}

	public float WidthTop {
		get {
			float width = 0;
			
			for (int i =0; i<Children.Count; i++) {
				if (!Children [i].IsReciprocal) {
					width += Children [i].Width;
				}
			}
			
			if (imaginaryChild != null) {
				if (!imaginaryChild.IsReciprocal) {
					width += ImaginaryChildWidth ();
				}
			}
			
			return width;
		}
	}

	public float WidthBottom {
		get {
			float width = 0;
			
			for (int i =0; i<Children.Count; i++) {
				if (Children [i].IsReciprocal) {
					width += Children [i].Width;
				}
			}
			
			if (imaginaryChild != null) {
				if (imaginaryChild.IsReciprocal) {
					width += ImaginaryChildWidth ();
				}
			}
			
			return width;
		}
	}

	public float HeightTop {
		get {
			float height = 0;
			
			for (int i =0; i<Children.Count; i++) {
				if (Children [i].Height > height && !Children [i].IsReciprocal) {
					height = Children [i].Height;
				}
			}
			
			return height;
		}
	}

	public float HeightBottom {
		get {
			float height = 0;
			
			for (int i =0; i<Children.Count; i++) {
				if (Children [i].Height > height && Children [i].IsReciprocal) {
					height = Children [i].Height;
				}
			}
			
			return height;
		}
	}

	public override float Height {
		get {
			return HeightTop + HeightBottom;
		}
	}

	public override float Width {
		get {
			return Mathf.Max (WidthTop, WidthBottom) + ((ShowPositiveSign || !ExpressionSign) ? Equations.SignWidth : 0f);
		}
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

		float topstart;
		float bottomstart;

		if (WidthBottom > WidthTop) {
			bottomstart = 0f;
			topstart = WidthBottom / 2 - WidthTop / 2;
		} else {
			topstart = 0f;
			bottomstart = WidthTop / 2 - WidthBottom / 2;
		}

		if (ShowingSign) {
			topstart += Equations.SignWidth;
			bottomstart += Equations.SignWidth;
		}

		if (!expression.IsReciprocal) {
			if (index == imaginaryChildIndex && expression != imaginaryChild && imaginaryChild != null) {
				return ChildPosx (imaginaryChild) + ImaginaryChildWidth ();
			} else {
				if (index == 0) {
					return topstart + Positionx;
				} else {
					return ChildPosx (Children [index - 1]) + Children [index - 1].Width;
				}
			}
		} else {
			if (index == imaginaryChildIndex && expression != imaginaryChild && imaginaryChild != null && imaginaryChild.IsReciprocal) {
				return ChildPosx (imaginaryChild) + ImaginaryChildWidth ();
			} else {
				if (index == NumTop) {
					return bottomstart + Positionx;
				} else {
					return ChildPosx (Children [index - 1]) + Children [index - 1].Width;
				}
			}
		}

	}

	public override float ChildPosy (Expression expression)
	{
		if (expression == null) {
			throw new UnityException ();
		}
		
		if (!expression.IsReciprocal) { // on top
			float extraheight = 0;
			if (expression.Height < HeightTop) {
				extraheight = (HeightTop - expression.Height) / 2;
			}

			return HeightBottom + extraheight + Positiony;
		} else {
			float extraheight = 0;
			if (expression.Height < HeightBottom) {
				extraheight = (HeightBottom - expression.Height) / 2;
			}

			return  extraheight + Positiony;
		}
		
	}

	protected override Expression ActualInstantiate ()
	{
		Expression ret = Instantiate (Equations.ProductPrefab).GetComponent<Expression> ();
		ret.transform.SetParent (Equations.Canvas.transform,false);
		return ret;

	}

	protected override float ImaginaryChildWidth ()
	{
		if (imaginaryChild == null)
			return 0;

		float imaginaryChildWith = imaginaryChild.Width;
		

		float diff = imaginaryChild.Positiony - ChildPosy (imaginaryChild);

		if (diff < -imaginaryChild.Height) {
			return 10f;
		} else {
			return imaginaryChildWith;
		}
	}
}