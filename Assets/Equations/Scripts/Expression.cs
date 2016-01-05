using UnityEngine;
using System.Collections.Generic;

abstract public class Expression : MonoBehaviour
{
	public Equations equations; // the main class for this blackboard (or whatever) instance

	public Expression Parent;
	public List<Expression> Children = new List<Expression> ();
	public Expression imaginaryChild = null; // child that it would have if dropped
	public int imaginaryChildIndex;

	public bool IsDraggingOver = false;
	public bool ExpressionSign = true;
	public bool ShowPositiveSign = false;
	public bool PlusSide = false; // false for LHS, true for RHS

	public bool IsDraggingUnder = false;
	public bool ReciprocalSide = false; // false for LHS, true for RHS

	public bool IsReciprocal = false; // only relevant once dropped


	public GameObject assumptionLeft = null; // only used for assumptions
	public GameObject assumptionRight = null;
	public int assumptionStep;

	// graphics
	Character signComponent;
	Reciprocal reciprocalComponent;
	Bracket bracketComponentTop;
	Bracket bracketComponentBottom;

	private bool showingSignOld;
	private bool allowsideways = true;
	
	// position expression would have as a child
	private float targetx;
	private float targety;
	private bool targetsign = false; // whether would show positive sign as a child

	// Use Start because equations class does Awake first
	public void Start ()
	{
		// create UI elements we need
		signComponent = Instantiate (equations.characterPrefab).GetComponent<Character> ();
		signComponent.equations = equations;
		signComponent.transform.SetParent (transform.parent,false);
		signComponent.transform.position = transform.position;

		reciprocalComponent = Instantiate (equations.reciprocalPrefab).GetComponent<Reciprocal> ();
		reciprocalComponent.equations = equations;
		reciprocalComponent.expression = this;
		reciprocalComponent.transform.SetParent (transform.parent,false);
		reciprocalComponent.transform.position = transform.position;

		bracketComponentTop = Instantiate (equations.bracketMiddle).GetComponent<Bracket> ();
		bracketComponentTop.equations = equations;
		bracketComponentTop.expression = this;
		bracketComponentTop.Top = true;
		bracketComponentTop.transform.SetParent (transform.parent,false);
		bracketComponentTop.gameObject.SetActive (false);

		bracketComponentBottom = Instantiate (equations.bracketMiddle).GetComponent<Bracket> ();
		bracketComponentBottom.equations = equations;
		bracketComponentBottom.expression = this;
		bracketComponentBottom.Top = false;
		bracketComponentBottom.transform.SetParent (transform.parent,false);
		bracketComponentBottom.gameObject.SetActive (false);
	}

	// Use LateUpdate because equations class does Update first
	public void LateUpdate ()
	{
		signComponent.transform.localPosition = new Vector3 (Positionx - 10f, Positiony + Height / 2 - 25f, 0);

		if (ExpressionSign) {
			if (ShowPositiveSign) {
				signComponent.character = "+";
			} else {
				signComponent.character = "";
			}
		} else {
			signComponent.character = "−";
		}

		// if we're currently dragging, either continue to drag or end
		if (IsDraggingOver) {
			if (Input.GetMouseButton (0) && Positiony >= targety) {
				DragOver ();
			} else {
				DropOver ();
				equations.topUndo.Capture ();
				IsDraggingOver = false;
			}
		} else if (IsDraggingUnder) {
			if (Input.GetMouseButton (0) && Positiony <= targety) {
				DragUnder ();
			} else {
				DropUnder ();
				equations.topUndo.Capture ();
				IsDraggingUnder = false;
			}
		} else {
			// if we're clicking on the expression and nothing else is being dragged
			if (ScreenRect.Contains (equations.mousePos) && Input.GetMouseButton (0) && Parent != null && !equations.dragging) {

				// if the cursor is moving up and we can add/subtract
				if (Parent.CanDragOver (this) && equations.mouseDelta.y > 0) {
					Parent.RemoveOver (this);
					ShowPositiveSign = true;
					IsDraggingOver = true;
					PlusSide = ExpressionSign ^ (transform.localPosition.x < 0); // exclusive or
					equations.dragging = true;
					allowsideways = false;

					targetx = Positionx;
					targety = Positiony;
					targetsign = false;
				} else

				// if the cursor is moving down and we can divide/multiply
				if (Parent.CanDragUnder (this) && equations.mouseDelta.y < 0) {
					Parent.RemoveUnder (this);
					IsDraggingUnder = true;
					ReciprocalSide = IsReciprocal ^ (transform.localPosition.x < 0); // exclusive or
					equations.dragging = true;
					allowsideways = false;

					targetx = Positionx;
					targety = Positiony;
					targetsign = false;
				}
			
			}

			// if not floating, always snap to position in parent
			if (Parent != null) {
				transform.localPosition = new Vector3 (Parent.ChildPosx (this), Parent.ChildPosy (this), 0);
			}
		}

		// reset dragging flag only when mouse is released, even if we have already dropped
		if (!Input.GetMouseButton (0)) {
			equations.dragging = false;
		}

		if (Parent != null) {
			if (ScreenRect.Contains (equations.mousePos)) {
				if (Parent.CanDragOver (this)) {
					bracketComponentBottom.gameObject.SetActive (true);
				} else {
					bracketComponentBottom.gameObject.SetActive (false);
				}
				if (Parent.CanDragUnder (this)) {
					bracketComponentTop.gameObject.SetActive (true);
				} else {
					bracketComponentTop.gameObject.SetActive (false);
				}
			} else {
				bracketComponentBottom.gameObject.SetActive (false);
				bracketComponentTop.gameObject.SetActive (false);
			}
		}
	}

	public void OnDestroy ()
	{
		if (signComponent != null) {
			Destroy (signComponent.gameObject);
		}
		
		if (reciprocalComponent != null) {
			Destroy (reciprocalComponent.gameObject);
		}

		if (assumptionLeft != null) {
			Destroy (assumptionLeft);
		}
		if (assumptionRight != null) {
			Destroy (assumptionRight);
		}

		if (bracketComponentBottom != null) {
			Destroy (bracketComponentBottom.gameObject);
			Destroy (bracketComponentBottom.gameObject);
		}
		
		if (GetComponent<Undo> () == null) { // only if this is not a saved undo
			foreach (var child in Children) {
				if (child != null) {
					Destroy (child.gameObject);
				}
			}
		}
	}

	public void DropOver ()
	{
		transform.parent.parent.GetComponent<BlackBoard>().PlayWritingSound();

		equations.obscuringBox.SetActive (false);

		//get rid of any old imaginary children
		foreach (var expression in equations.GetComponentsInChildren<Expression>()) {
			expression.imaginaryChild = null;
		}

		cumulativesideways = 0;

		//Globals.Dragging = false;

		EquationSide side;

		if (ScreenRect.xMin + equations.signWidth / 2 > 0) {
			side = equations.rhs;
			ExpressionSign = PlusSide;
		} else {
			side = equations.lhs;
			ExpressionSign = !PlusSide;
		}

		if (side.Children [0].GetType () == typeof(Sum)) {
			// find correct location for insertion
			side.Children [0].imaginaryChild = this;


			int index = 0;
			while (index < side.Children[0].Children.Count && ScreenRect.center.x > side.Children[0].Children[index].ScreenRect.center.x) {
				index++;
			}

			side.Children [0].imaginaryChild = null;
			// already a sum, so just add another term
			((Sum)side.Children [0]).Insert (index, this);
		} else if (side.Children [0].GetType () == typeof(Zero)) {
			// replace with expression
			Destroy (side.Children [0].gameObject);
			side.Children [0] = this;
			Parent = side;
		} else {
			// must create a new sum
			Sum newSum = Instantiate (equations.sumPrefab).GetComponent<Sum> ();
			newSum.equations= equations;
			newSum.Parent = side;
			newSum.transform.SetParent (transform.parent,false);

			if (ScreenRect.center.x > side.Children [0].ScreenRect.center.x) {
				newSum.Insert (0, side.Children [0]);
				newSum.Insert (1, this);						//insert onto right

			} else {
				newSum.Insert (0, this);						// insert onto left
				newSum.Insert (1, side.Children [0]);
			}

			side.Children [0] = newSum;

			newSum.transform.localPosition = new Vector3 (side.ChildPosx (newSum), side.ChildPosy (newSum), 0);
		}
	}
	
	public void DragOver ()
	{
		equations.obscuringBox.SetActive (true);
		equations.obscuringBox.transform.SetAsLastSibling ();
		((RectTransform)equations.obscuringBox.transform).sizeDelta = new Vector2 (ScreenRect.width, ScreenRect.height);
		equations.obscuringBox.transform.localPosition = transform.localPosition;
		BringToFront ();

		if (Positiony - targety > Height) {
			allowsideways = true;
		} else {
			Vector3 newPosition = new Vector3 (targetx - (targetsign || !ExpressionSign ? 0 : equations.signWidth), transform.localPosition.y, 0);
			float sidewaysmove = newPosition.x - transform.localPosition.x;
			transform.localPosition = newPosition;
			allowsideways = false;
			Move(new Vector3(-sidewaysmove,0,0)); // store the movement so we can snap back afterwards
		}

		float oldPos = ScreenRect.xMin + equations.signWidth / 2;
		Move (equations.mouseDelta);
		float newPos = ScreenRect.xMin + equations.signWidth / 2;

		if (oldPos < 0 != newPos < 0) {
			ExpressionSign = !ExpressionSign;
		}

		//get rid of any old imaginary children
		foreach (var expression in equations.GetComponentsInChildren<Expression>()) {
			expression.imaginaryChild = null;
		}

		EquationSide side;
		
		if (ScreenRect.xMin + equations.signWidth / 2 > 0) {
			side = equations.rhs;
		} else {
			side = equations.lhs;
		}
		
		if (side.Children [0].GetType () == typeof(Sum)) {
			side.Children [0].imaginaryChild = this;


			// find correct location for insertion
			int index = 0;
			while (index < side.Children[0].Children.Count && ScreenRect.center.x > side.Children[0].Children[index].ScreenRect.center.x) {
				index++;
			}
			
			// already a sum, so just add another term
			side.Children [0].imaginaryChildIndex = index;
			targetx = side.Children [0].ChildPosx (this);
			targety = side.Children [0].ChildPosy (this);
			targetsign = index != 0;
		} else if (side.Children [0].GetType () == typeof(Zero)) {
			targetx = side.ChildPosx (this);
			targety = side.ChildPosy (this);
			targetsign = false;
		} else {
			// must create a new sum
			Sum newSum = Instantiate (equations.sumPrefab).GetComponent<Sum> ();
			newSum.equations = equations;
			newSum.Parent = side;
			newSum.transform.SetParent (transform.parent,false);
			
			if (ScreenRect.center.x > side.Children [0].ScreenRect.center.x) {
				newSum.Insert (0, side.Children [0]);
				//newSum.Insert(1, this);						//insert onto right
				newSum.imaginaryChild = this;
				newSum.imaginaryChildIndex = 1;
				
			} else {
				//newSum.Insert(0, this);						// insert onto left
				newSum.Insert (0, side.Children [0]);
				newSum.imaginaryChild = this;
				newSum.imaginaryChildIndex = 0;

			}
			
			side.Children [0] = newSum;

			targetx = newSum.ChildPosx (this);
			targety = newSum.ChildPosy (this);
			targetsign = newSum.imaginaryChildIndex != 0;
			
			newSum.transform.localPosition = new Vector3 (side.ChildPosx (newSum), side.ChildPosy (newSum), 0);
		}

	}

	public void DropUnder ()
	{
		// play a sound
		transform.parent.parent.GetComponent<BlackBoard>().PlayWritingSound();

		// hide the blakc box
		equations.obscuringBox.SetActive (false);

		// reset any unused sideways movement we've stored
		cumulativesideways = 0;

		
		//get rid of any old imaginary children
		foreach (var expression in equations.GetComponentsInChildren<Expression>()) {
			expression.imaginaryChild = null;
		}

		EquationSide side;
		
		if (ScreenRect.center.x > 0) {
			side = equations.rhs;
			IsReciprocal = ReciprocalSide;
		} else {
			side = equations.lhs;
			IsReciprocal = !ReciprocalSide;
		}

		if (IsReciprocal && equations.showAssumptions) {
			Expression assumpt = Create ();
			assumpt.transform.localPosition = new Vector3 (0 + 20, 300 - equations.numAssumptions * 50, 0);
			GameObject left = Instantiate (equations.assumptionLeft);
			GameObject right = Instantiate (equations.assumptionRight);

			left.transform.SetParent (transform.parent,false);
			right.transform.SetParent (transform.parent,false);

			left.transform.localPosition = new Vector3 (assumpt.ScreenRect.xMin, assumpt.ScreenRect.center.y);
			right.transform.localPosition = new Vector3 (assumpt.ScreenRect.xMax, assumpt.ScreenRect.center.y);

			assumpt.assumptionLeft = left;
			assumpt.assumptionRight = right;
			assumpt.assumptionStep = equations.step;

			equations.numAssumptions++;
		}

		if (side.Children [0].GetType () == typeof(One)) {
			if (IsReciprocal) {
				// must create a new product
				Product newProd = Instantiate (equations.productPrefab).GetComponent<Product> ();
				newProd.Parent = side;
				newProd.equations = equations;
				newProd.transform.SetParent (transform.parent,false);
					
				newProd.Insert (0, side.Children [0]);
				newProd.Insert (1, this);
					
				side.Children [0] = newProd;
					
				newProd.transform.localPosition = new Vector3 (side.ChildPosx (newProd), side.ChildPosy (newProd), 0);

			} else {
				if (!side.Children [0].ExpressionSign) {
					ExpressionSign = !ExpressionSign;
				}
				Destroy (side.Children [0].gameObject);
				side.Children [0] = this;
				Parent = side;
			}
		} else if (side.Children [0].GetType () == typeof(Zero)) {
			// drop new element
			Destroy (gameObject);
		} else {
			if (side.Children [0].GetType () == typeof(Product)) {
				// already have a product, so just need to insert into it
				Product prod = (Product)side.Children [0];
				prod.imaginaryChild = this;

				// find index to insert at
				int index;
				if (IsReciprocal) {
					index = prod.NumTop;
					while (index < prod.Children.Count && ScreenRect.center.x > prod.Children[index].ScreenRect.center.x) {
						index++;
					}
				} else {
					index = 0;
					while (index < prod.NumTop && ScreenRect.center.x > prod.Children[index].ScreenRect.center.x) {
						index++;
					}
				}

				prod.Insert (index, this);

				prod.imaginaryChild = null;


			} else {
				// must create a new sum
				Product newProd = Instantiate (equations.productPrefab).GetComponent<Product> ();
				newProd.equations = equations;
				newProd.Parent = side;
				newProd.transform.SetParent (transform.parent,false);

				if (side.Children [0].IsReciprocal == IsReciprocal) { // if both new and old elements are on same side of fraction
					if (ScreenRect.center.x > side.Children [0].ScreenRect.center.x) {
						newProd.Insert (0, side.Children [0]);
						newProd.Insert (1, this);						//insert onto right
						
					} else {
						newProd.Insert (0, this);						// insert onto left
						newProd.Insert (1, side.Children [0]);
					}
				} else {
					if (IsReciprocal) {
						newProd.Insert (0, side.Children [0]);
						newProd.Insert (1, this);
					} else {
						newProd.Insert (0, this);
						newProd.Insert (1, side.Children [0]);
					}
				}
				
				side.Children [0] = newProd;


				
				newProd.transform.localPosition = new Vector3 (side.ChildPosx (newProd), side.ChildPosy (newProd), 0);
			}
		}

		if ((equations.lhs.Children [0].GetType () == typeof(One) && equations.rhs.Children [0].GetType () == typeof(Zero)) || (equations.rhs.Children [0].GetType () == typeof(One) && equations.lhs.Children [0].GetType () == typeof(Zero))) {
			equations.helpText.SetActive (true);
		}
	}

	public void DragUnder ()
	{
		equations.obscuringBox.SetActive (true);
		equations.obscuringBox.transform.SetAsLastSibling ();
		((RectTransform)equations.obscuringBox.transform).sizeDelta = new Vector2 (ScreenRect.width, ScreenRect.height);
		equations.obscuringBox.transform.localPosition = transform.localPosition;
		BringToFront ();

		if (targety - Positiony > Height) {
			allowsideways = true;
		} else {
			Vector3 newPosition = new Vector3 (targetx, transform.localPosition.y, 0);
			float sidewaysmove = newPosition.x - transform.localPosition.x;
			transform.localPosition = newPosition;
			allowsideways = false;
			Move(new Vector3(-sidewaysmove,0,0)); // store the movement so we can snap back afterwards

		}

		Move (equations.mouseDelta);

		//get rid of any old imaginary children
		foreach (var expression in equations.GetComponentsInChildren<Expression>()) {
			expression.imaginaryChild = null;
		}

		EquationSide side;
		
		if (ScreenRect.center.x > 0) {
			side = equations.rhs;
			IsReciprocal = ReciprocalSide;
		} else {
			side = equations.lhs;
			IsReciprocal = !ReciprocalSide;
		}

		if (side.Children [0].GetType () == typeof(Product)) {
			// already have a product, so just need to insert into it
			Product prod = (Product)side.Children [0];
			prod.imaginaryChild = this;

			
			// find index to insert at
			int index;
			if (IsReciprocal) {
				index = prod.NumTop;
				while (index < prod.Children.Count && ScreenRect.center.x > prod.Children[index].ScreenRect.center.x) {
					index++;
				}
			} else {
				index = 0;
				while (index < prod.NumTop && ScreenRect.center.x > prod.Children[index].ScreenRect.center.x) {
					index++;
				}
			}
			
			prod.imaginaryChildIndex = index;
			targetx = prod.ChildPosx (this);
			targety = prod.ChildPosy (this);

			
				
		} else if (side.Children [0].GetType () != typeof(One) && side.Children [0].GetType () != typeof(Zero)) {         // no movement in these cases

			// must create a new sum
			Product newProd = Instantiate (equations.productPrefab).GetComponent<Product> ();
			newProd.Parent = side;
			newProd.equations = equations;
			newProd.transform.SetParent (transform.parent,false);
			
			if (side.Children [0].IsReciprocal == IsReciprocal) { // if both new and old elements are on same side of fraction
				if (ScreenRect.center.x > side.Children [0].ScreenRect.center.x) {
					newProd.Insert (0, side.Children [0]);
					//newProd.Insert (1, this);						//insert onto right
					newProd.imaginaryChild = this;
					newProd.imaginaryChildIndex = 1;
					
				} else {
					//newProd.Insert (0, this);						// insert onto left
					newProd.Insert (0, side.Children [0]);
					newProd.imaginaryChild = this;
					newProd.imaginaryChildIndex = 0;

				}
			} else {
				if (IsReciprocal) {
					newProd.Insert (0, side.Children [0]);
					//newProd.Insert (1, this);
					newProd.imaginaryChild = this;
					newProd.imaginaryChildIndex = 1;

				} else {
					//newProd.Insert (0, this);
					newProd.Insert (0, side.Children [0]);
					newProd.imaginaryChild = this;
					newProd.imaginaryChildIndex = 0;

				}
			}

			targetx = newProd.ChildPosx (this);
			targety = newProd.ChildPosy (this);
			side.Children [0] = newProd;
			
			newProd.transform.localPosition = new Vector3 (side.ChildPosx (newProd), side.ChildPosy (newProd), 0);
		} else {
			targetx = side.ChildPosx (this);
			targety = side.ChildPosy (this);
			targetsign = false;
		}

	}

	// removes a child by dragging over the top
	abstract public void RemoveOver (Expression child);

	// removes a child by dragging under the bottom
	abstract public void RemoveUnder (Expression child);

	abstract public bool CanDragOver (Expression child);

	abstract public bool CanDragUnder (Expression child);

	abstract public float ChildPosx (Expression child);

	abstract public float ChildPosy (Expression child);
	
	public float Positionx {
		get {
			if (Parent != null) {
				return Parent.ChildPosx (this);
			} else {
				return transform.localPosition.x;
			}
		}
	}

	public float Positiony {
		get {
			if (Parent != null) {
				return Parent.ChildPosy (this);
			} else {
				return transform.localPosition.y;
			}
		}
	}

	// Rect on canvas (equals sign is the origin)
	public Rect ScreenRect {
		get {
			return new Rect (Positionx, Positiony, Width, Height);
		}
	}

	public bool ShowingSign {
		get{ return ShowPositiveSign || !ExpressionSign; }
	}

	abstract public float Width { get; }

	abstract public float Height { get; }

	float cumulativesideways = 0; // if we're not allowed to move sideways, store it anyway

	public void Move (Vector3 delta)
	{
		if (!allowsideways) {
			cumulativesideways += delta.x;
			delta.x = 0;
		} else {
			delta.x += cumulativesideways;
			cumulativesideways = 0;
		}


		transform.localPosition = transform.localPosition + delta; // move by the amount given
	}

	public Expression Store (Undo undo)
	{
		Expression ret = (Expression)undo.gameObject.AddComponent (GetType ());
		ret.equations = equations;
		ret.enabled = false;
		
		foreach (var child in Children) {
			var newchild = child.Store (undo);
			ret.Children.Add (newchild);
			newchild.Parent = ret;
		}

		ret.ExpressionSign = ExpressionSign;
		ret.IsReciprocal = IsReciprocal;

		Copy (ret);
		
		return ret;
	}

	// stores extra information
	protected virtual void Copy (Expression ret)
	{
	}

	// Used by Undo to create the component. calls ActualInstantiate
	public Expression Create ()
	{
		Expression ret = ActualInstantiate ();

		foreach (var child in Children) {
			var newchild = child.Create ();
			ret.Children.Add (newchild);
			newchild.Parent = ret;
		}

		ret.ExpressionSign = ExpressionSign;
		ret.IsReciprocal = IsReciprocal;
		ret.equations = equations;

		return ret;
	}

	protected abstract Expression ActualInstantiate ();

	public virtual void Reduce ()
	{
	}

	protected virtual float ImaginaryChildWidth ()
	{
		if (imaginaryChild != null) {
			return imaginaryChild.Width;
		}

		return 0;
	}

	// used when moving a component
	public virtual void BringToFront ()
	{
		foreach (var child in Children) {
			child.BringToFront ();
		}

		reciprocalComponent.transform.SetAsLastSibling ();
		signComponent.transform.SetAsLastSibling ();
		bracketComponentTop.BringToFront ();
		bracketComponentBottom.BringToFront ();
	}
}
