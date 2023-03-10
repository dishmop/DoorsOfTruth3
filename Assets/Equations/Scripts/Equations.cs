// EQUATIONS by Jeremy Parker

// This class links everything together and provides globals

// The other main class is the abstract class Expression
// Various expression types inherit from Expression and have a heirarchy, ultimately leading to
// EquationSide or null if being dragged

// There are two main interactions, called dragging over and dragging under
// These correspond to adding/subtracting and multiplying/dividing
// A lot of the code for over and under is the same, but with subtle differences in handling
using UnityEngine;

public class Equations : MonoBehaviour
{
	// things to be assigned in the editor
	public GameObject characterPrefab;
	public GameObject reciprocalPrefab;
	public EquationSide lhs;
	public EquationSide rhs;
	public GameObject sumPrefab;
	public GameObject productPrefab;
	public GameObject onePrefab;
	public GameObject zeroPrefab;
	public GameObject variablePrefab;
	public Undo topUndo;
	public GameObject assumptionLeft;
	public GameObject assumptionRight;
	public GameObject helpText;
	public GameObject obscuringBox;
	public GameObject bracketMiddle;
	public GameObject bracketEnd;
	public Color backgroundColour;

	// globals
	public int numAssumptions = 0;
	public int step = 0;
	public Vector3 mouseDelta;
	public Vector3 oldMousePos = new Vector3 ();
	public Vector3 mousePos = new Vector3 ();
	public bool dragging = false;
	public Color defaultTextColour;
    public bool isInteractable = true;

	// constants
	public readonly float signWidth = 35f; // width allowed for + and - symbol
	public readonly float sideOffset = 40f; // distance of equation sides from centre
	public readonly bool showAssumptions = false; // whether to display list of assumptions

	public string ArrangedFor; // which symbol is on LHS/RHS
	
	public void Awake ()
	{
		obscuringBox.GetComponent<UnityEngine.UI.RawImage> ().color = backgroundColour;
	}

	// so we can run stuff on second frame
	private bool firstframe = true;
	private bool secondframe = false;

	public void Update ()
	{
		if (secondframe) {
			topUndo.Capture (); // capture initial state for undo
			
			secondframe = false;
		}
		if (firstframe) { 
			firstframe = false;
			secondframe = true;
		}


		oldMousePos = mousePos;
		if (GetComponent<Canvas> ().renderMode == RenderMode.WorldSpace) {

			// transform mouse position into canvas space
			Vector2 mousepos2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle ((RectTransform)transform, Input.mousePosition, Camera.main, out mousepos2);
			mousePos = new Vector3 (mousepos2.x, mousepos2.y, 0);
		} else {
			mousePos = Input.mousePosition - new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		}

		// Work out change in mouse position
		mouseDelta = mousePos - oldMousePos;

		// LHS has precedence over RHS (in practice there should never be a conflict)
		if (lhs.Children [0].GetType () == typeof(Variable)) {
			ArrangedFor = ((Variable)lhs.Children [0]).Symbol;
		} else if (rhs.Children [0].GetType () == typeof(Variable)) {
			ArrangedFor = ((Variable)rhs.Children [0]).Symbol;
		} else {
			ArrangedFor = null;
		}
	}

	public void Undo ()
	{
		if (topUndo.next.Restore ()) {
			// play sound if we actually erased
			transform.parent.GetComponent<BlackBoard>().PlayErasingSound();
		}
		helpText.SetActive (false);
	}
}
