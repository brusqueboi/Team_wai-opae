using UnityEngine;
using System.Collections;

public class CursorMovement : MonoBehaviour {

    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;
    private Vector3 movementVector;
    private CharacterController player;
    private float movementSpeed = 8;
    public int playerNumber;

	// Use this for initialization
	void Start ()
    {
        player = this.GetComponent<CharacterController>();
        checkForPlayers();
    }
	
	// Update is called once per frame
	void Update () {
        
        // check player number and move object accordingly
        if(playerNumber == 1)
        {
            movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P1") * movementSpeed;
            movementVector.y = Input.GetAxis("LeftJoystickVertical_P1") * movementSpeed;
        }

        else if(playerNumber == 2)
        {
            movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P2") * movementSpeed;
            movementVector.y = Input.GetAxis("LeftJoystickVertical_P2") * movementSpeed;
        }

        else if(playerNumber == 3)
        {
            movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P3") * movementSpeed;
            movementVector.y = Input.GetAxis("LeftJoystickVertical_P3") * movementSpeed;
        }

        else if(playerNumber == 4)
        {
            movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P4") * movementSpeed;
            movementVector.y = Input.GetAxis("LeftJoystickVertical_P4") * movementSpeed;
        }

        player.Move(movementVector * Time.deltaTime);
	}

    void checkForPlayers()
    {
        if(playerNumber == 1)
        {
            if(GameObject.Find("Player 1 Cursor") != null)
            {
                Physics.IgnoreCollision(player2.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player3.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player4.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
            }
        }

        if (playerNumber == 2)
        {
            if (GameObject.Find("Player 2 Cursor") != null)
            {
                Physics.IgnoreCollision(player1.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player3.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player4.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
            }
        }

        if (playerNumber == 3)
        {
            if (GameObject.Find("Player 3 Cursor") != null)
            {
                Physics.IgnoreCollision(player1.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player2.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player4.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
            }
        }

        if (playerNumber == 4)
        {
            if (GameObject.Find("Player 4 Cursor") != null)
            {
                Physics.IgnoreCollision(player1.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player2.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
                Physics.IgnoreCollision(player3.GetComponent<CharacterController>(), this.GetComponent<CharacterController>());
            }
        }
    }
}
