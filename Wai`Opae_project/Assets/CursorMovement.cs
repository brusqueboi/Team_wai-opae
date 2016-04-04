using UnityEngine;
using System.Collections;

public class CursorMovement : MonoBehaviour {

    private Vector3 movementVector;
    private CharacterController characterController;
    private float movementSpeed = 8;

	// Use this for initialization
	void Start () {
        characterController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        movementVector.x = Input.GetAxis("LeftJoystickHorizontal") * movementSpeed;
        movementVector.z = Input.GetAxis("LeftJoystickVertical") * movementSpeed;

        characterController.Move(movementVector * Time.deltaTime);
	}
}
