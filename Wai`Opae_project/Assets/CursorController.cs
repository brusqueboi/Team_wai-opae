using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour {

	public int playerIndex = 1;

	public float yPosition = 1.87f;

    private float movementSpeed = 8;

	protected bool visible = true;
	public bool Visible
	{
		get { return visible; }
		set
		{
			if(value != visible)
			{
				visible = value;
				gameObject.GetComponent<Renderer>().enabled = visible;
			}
		}
	}
	
	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update () {

        // quit the .exe by pressing esc key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

		if (Visible)
		{
			Vector2 moveDirection = 
				GameModel.Model.GetPlayer(playerIndex).Controller.LeftAnalog * movementSpeed * Time.deltaTime;
			gameObject.GetComponent<CharacterController>().Move(
				new Vector3(moveDirection.x, 0.0f, moveDirection.y));
			Vector3 updatedPosition = gameObject.transform.position;
			updatedPosition.y = yPosition;
			gameObject.transform.position = updatedPosition;
		}
	}
}
