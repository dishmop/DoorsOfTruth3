// EQUATIONS by Jeremy Parker

// This class links everything together and provides globals

// Other main class in abstract class Expression
// Various expression types inherit from Expression and have a heirarchy, ultimately leading to
// EquationSide or null if being dragged

// There are two main interactions, called dragging over and dragging under

using UnityEngine;

public class Equations : MonoBehaviour
{

	// our actual globals - static members cannot be assigned in editor

	// things to be linked
	public static EquationSide LHS;
	public static EquationSide RHS;
	public static Undo TopUndo;
	public static GameObject HelpText;
	public static GameObject ObscuringBox;

	// prefabs
	public static GameObject CharacterPrefab;
	public static GameObject ReciprocalPrefab;
	public static GameObject ProductPrefab;
	public static GameObject SumPrefab;
	public static GameObject OnePrefab;
	public static GameObject ZeroPrefab;
	public static GameObject VariablePrefab;
	public static GameObject AssumptionRight;
	public static GameObject AssumptionLeft;
	public static GameObject BracketMiddle;
	public static GameObject BracketEnd;

	// constants
	public static readonly float SignWidth = 35f; // width allowed for + and - symbol
	public static readonly float SideOffset = 40f; // distance of equation sides from centre
	public static readonly bool ShowAssumptions = false; // whether to display list of assumptions


	// global variables
	public static int NumAssumptions = 0;
	public static int Step = 0;
	public static Vector3 MouseDelta;
	public static Vector3 OldMousePos = new Vector3 ();
	public static Vector3 MousePos = new Vector3 ();
	public static bool Dragging = false;
	public static Color DefaultTextColour;

	// singleton
	public static GameObject Canvas = null;


	// so we can pass the prefabs in the editor
	[SerializeField]
	GameObject characterPrefab;
	[SerializeField]
	GameObject reciprocalPrefab;
	[SerializeField]
	EquationSide lhs;
	[SerializeField]
	EquationSide rhs;
	[SerializeField]
	GameObject sumPrefab;
	[SerializeField]
	GameObject productPrefab;
	[SerializeField]
	GameObject onePrefab;
	[SerializeField]
	GameObject zeroPrefab;
	[SerializeField]
	GameObject variablePrefab;
	[SerializeField]
	Undo topUndo;
	[SerializeField]
	GameObject assumptionLeft;
	[SerializeField]
	GameObject assumptionRight;
	[SerializeField]
	GameObject helpText;
	[SerializeField]
	GameObject obscuringBox;
	[SerializeField]
	GameObject bracketMiddle;
	[SerializeField]
	GameObject bracketEnd;
	[SerializeField]
	Color textColour;
	[SerializeField]
	Color backgroundColour;

	public string ArrangedFor; // which symbol is on LHS/RHS
	
	public void Awake ()
	{
		// set up static variables
		if (Canvas != null) {
			throw new UnityException ("Globals must have only instance");
		}
		Canvas = gameObject;

		CharacterPrefab = characterPrefab;
		ReciprocalPrefab = reciprocalPrefab;

		LHS = lhs;
		RHS = rhs;

		SumPrefab = sumPrefab;
		ProductPrefab = productPrefab;

		ZeroPrefab = zeroPrefab;
		OnePrefab = onePrefab;

		VariablePrefab = variablePrefab;

		AssumptionLeft = assumptionLeft;
		AssumptionRight = assumptionRight;	

		TopUndo = topUndo;

		HelpText = helpText;

		ObscuringBox = obscuringBox;

		BracketEnd = bracketEnd;
		BracketMiddle = bracketMiddle;

		DefaultTextColour = textColour;

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


		OldMousePos = MousePos;
		if (GetComponent<Canvas> ().renderMode == RenderMode.WorldSpace) {

			// transform mouse position into canvas space
			Vector2 mousepos2;
			RectTransformUtility.ScreenPointToLocalPointInRectangle ((RectTransform)transform, Input.mousePosition, Camera.main, out mousepos2);
			MousePos = new Vector3 (mousepos2.x, mousepos2.y, 0);
		} else {
			MousePos = Input.mousePosition - new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		}

		// Work out change in mouse position
		MouseDelta = MousePos - OldMousePos;

		// LHS has precedence over RHS (in practice there should never be a conflict)
		if (LHS.Children [0].GetType () == typeof(Variable)) {
			ArrangedFor = ((Variable)LHS.Children[0]).Symbol;
		} else if (RHS.Children [0].GetType () == typeof(Variable)) {
			ArrangedFor = ((Variable)RHS.Children[0]).Symbol;
		} else {
			ArrangedFor = null;
		}

	}

	public void Undo ()
	{
		TopUndo.next.Restore ();
		HelpText.SetActive (false);
	}
}
