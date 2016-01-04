using UnityEngine;
using System.Collections;

public class Undo : MonoBehaviour {
	public Undo next = null;
	public Undo prev = null;
	int step;

	EquationSide LHS;
	EquationSide RHS;

	public void Update() {
		if (next != null) {
			next.prev = this;
		}
	}

	public void Capture() { //saves current state

		foreach(var expression in Equations.Canvas.GetComponentsInChildren<Expression>()){
			expression.Reduce();
		}

		// find last state
		Undo last = this;
		while (last.next!=null) {
			last = last.next;
		}

		//move states along
		while(last!=this){

			if(last.prev.LHS != null) {
				last.LHS = (EquationSide)last.prev.LHS.Store (last);
				last.RHS = (EquationSide)last.prev.RHS.Store (last);
				last.step = last.prev.step;

				foreach(var expression in last.prev.GetComponents<Expression>()){
					Destroy (expression);
				}
			}

			last = last.prev;
		}

		// store current scene
		LHS = (EquationSide)Equations.LHS.Store (this);
		RHS = (EquationSide)Equations.RHS.Store (this);
		Equations.Step++;
		step = Equations.Step;
	}

	public void Restore() {
		if (LHS == null)
			return;

		// clear scene
		foreach (var expression in Equations.Canvas.GetComponentsInChildren<Expression>()) {
			if(expression!=Equations.LHS && expression!=Equations.RHS && expression.GetComponent<Undo>()==null){
				Expression topparent = expression;

				while(topparent.Parent!=null) {
					topparent = topparent.Parent;
				}

				if(topparent.assumptionLeft != null && topparent.assumptionStep < step) {
					continue;
				}
				Destroy(expression.gameObject);
			}
		}


		// copy all saved members to scene
		Equations.LHS.Children [0] = LHS.Children [0].Create (); // recursively creates everything
		Equations.LHS.Children [0].Parent = Equations.LHS;

		Equations.RHS.Children [0] = RHS.Children [0].Create ();
		Equations.RHS.Children [0].Parent = Equations.RHS;

		Equations.Step = step;

		
		// now move state back

		// find last state
		Undo last = this;
		while (last.next!=null) {
			last = last.next;
		}

		// change which is top undo
		Equations.TopUndo = this;

		// clear the state
		foreach(var expression in prev.GetComponents<Expression>()){
			Destroy (expression);
		}
		
		prev.next = null;
		prev.LHS = null;
		prev.RHS = null;

		// set this as new last state
		last.next = prev;

		prev = null;
	}
}
