using UnityEngine;
using System.Collections;
//Written by Michael Bethke
public class MouseMovement : MonoBehaviour
{

	private float mouseMovementSpeed = 5;

	
	private void Update ()
	{
		
		if ( Input.GetKey ( KeyCode.Mouse0 ))
		{

			gameObject.transform.Translate ( new Vector3 ( -1 * ( Input.GetAxis ( "Mouse X" ) * Time.deltaTime ) * mouseMovementSpeed, -1 * ( Input.GetAxis ( "Mouse Y" ) * Time.deltaTime ) * mouseMovementSpeed, 0 ));
		}
		
		if ( Input.GetAxis ( "Mouse ScrollWheel" ) != 0 )
		{
			
			//gameObject.transform.Translate ( new Vector3 ( 0, 0, Input.GetAxis ( "Mouse ScrollWheel" )));
			Camera.main.orthographicSize -= Input.GetAxis ( "Mouse ScrollWheel" );
		}
	}
}
