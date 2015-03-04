using UnityEngine;
using System.Collections;
//Written by Michael Bethke
public class PlayerMovement : MonoBehaviour
{
	
	internal bool canMove = false;
	
	
	private void Update ()
	{
		
		if ( canMove == true )
		{
			
			if ( Input.GetKey ( KeyCode.W ) || Input.GetKey ( KeyCode.UpArrow ))
			{
				
				AttemptMove ( 0 );
			}
			
			if ( Input.GetKey ( KeyCode.S ) || Input.GetKey ( KeyCode.DownArrow ))
			{
				
				AttemptMove ( 1 );
			}
			
			if ( Input.GetKey ( KeyCode.A ) || Input.GetKey ( KeyCode.LeftArrow ))
			{
				
				AttemptMove ( 2 );
			}
			
			if ( Input.GetKey ( KeyCode.D ) || Input.GetKey ( KeyCode.RightArrow ))
			{
				
				AttemptMove ( 3 );
			}
		}
	}
	
	
	private void AttemptMove ( int direction )
	{
		
		UnityEngine.Debug.Log ( direction );
	}
}
