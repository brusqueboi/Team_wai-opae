using UnityEngine;
using System.Collections;

public class CursorMovement : MonoBehaviour {

    public Transform player1;
    public Transform player2;
    public Transform player3;
    public Transform player4;
    private Vector3 movementVector;
    private CharacterController player;
    private Renderer visible1;
    private Renderer visible2;
    private Renderer visible3;
    private Renderer visible4;
    private float movementSpeed = 8;
    public int playerNumber;
    private float x, z;
    private const float y = 1.87f;

	// Use this for initialization
	void Start ()
    {
        player = this.GetComponent<CharacterController>();
        setVisibility();
        checkForPlayers();
    }
	
	// Update is called once per frame
	void Update () {

        // quit the .exe by pressing esc key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // get x and z axis of current player cursor. y is constant
        x = player.transform.position.x;
        z = player.transform.position.z;

        // check player number and move object accordingly
        // player 1 movement
        if (playerNumber == 1)
        {
            // toggles player 1 visibility
            if (Input.GetButtonDown("StartButton_P1"))
            {
                if (visible1.enabled == false)
                {
                    visible1.enabled = true;
                    //Debug.Log("Player 1 Enabled");
                }

                else
                {
                    visible1.enabled = false;
                    //Debug.Log("Player 1 Disabled");
                }
            }

            if (visible1.isVisible)
            {
                // makes cursor stay on a constant y axix position. player can only move on x, z axis
                player.transform.position = new Vector3(x, y, z);

                movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P1") * movementSpeed;
                movementVector.z = Input.GetAxis("LeftJoystickVertical_P1") * movementSpeed/2;
                player.Move(movementVector * Time.deltaTime);
            }

            /*if (Input.GetButtonDown("BackButton_P1"))
            {
                playerNumber++;
            }*/
        }

        // player 2 movement
        else if(playerNumber == 2)
        {
            // toggles player 2 visibility
            if (Input.GetButtonDown("StartButton_P2"))
            {
                if (visible2.enabled == false)
                {
                    visible2.enabled = true;
                    //Debug.Log("Player 2 Enabled");
                }

                else
                {
                    visible2.enabled = false;
                    //Debug.Log("Player 2 Disabled");
                }
            }

            if (visible2.isVisible)
            {
                // makes cursor stay on a constant y axix position. player can only move on x, z axis
                player.transform.position = new Vector3(x, y, z);

                movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P2") * movementSpeed;
                movementVector.y = Input.GetAxis("LeftJoystickVertical_P2") * movementSpeed;
                player.Move(movementVector * Time.deltaTime);
            }

            /*if (Input.GetButtonDown("BackButton_P1"))
            {
                playerNumber++;
            }*/
        }

        // player 3 movement
        else if(playerNumber == 3)
        {
            // toggles player 3 visibility
            if (Input.GetButtonDown("StartButton_P3"))
            {
                if (visible3.enabled == false)
                {
                    visible3.enabled = true;
                    //Debug.Log("Player 3 Enabled");
                }

                else
                {
                    visible3.enabled = false;
                    //Debug.Log("Player 3 Disabled");
                }
            }

            if (visible3.isVisible)
            {
                // makes cursor stay on a constant y axix position. player can only move on x, z axis
                player.transform.position = new Vector3(x, y, z);

                movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P3") * movementSpeed;
                movementVector.y = Input.GetAxis("LeftJoystickVertical_P3") * movementSpeed;
                player.Move(movementVector * Time.deltaTime);
            }

            /*if (Input.GetButtonDown("BackButton_P1"))
            {
                playerNumber++;
            }*/
        }

        // player 4 movement
        else if(playerNumber == 4)
        {
            // toggle player 4 visibility
            if (Input.GetButtonDown("StartButton_P4"))
            {
                if (visible4.enabled == false)
                {
                    visible4.enabled = true;
                    //Debug.Log("Player 4 Enabled");
                }

                else
                {
                    visible4.enabled = false;
                    //Debug.Log("Player 4 Disabled");
                }
            }

            if (visible4.isVisible)
            {
                // makes cursor stay on a constant y axix position. player can only move on x, z axis
                player.transform.position = new Vector3(x, y, z);

                movementVector.x = Input.GetAxis("LeftJoystickHorizontal_P4") * movementSpeed;
                movementVector.y = Input.GetAxis("LeftJoystickVertical_P4") * movementSpeed;
                player.Move(movementVector * Time.deltaTime);
            }

            /*if (Input.GetButtonDown("BackButton_P1"))
            {
                playerNumber = 1;
            }*/
        }
	}

    // sets visibility of player cursor at start. start with one player
    void setVisibility()
    {
        visible1 = player1.GetComponent<Renderer>();
        visible2 = player2.GetComponent<Renderer>();
        visible3 = player3.GetComponent<Renderer>();
        visible4 = player4.GetComponent<Renderer>();

        visible1.enabled = true;
        visible2.enabled = true;
        visible3.enabled = true;
        visible4.enabled = true;
    }

    // ignores the cursor collision of other players
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
