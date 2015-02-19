using System;
using System.IO;
using UnityEngine;
using System.Collections;
//Written by Michael Bethke
public static class Read
{
	
	public static string XMLFile ( string path )
	{
		
		StreamReader streamReader = new StreamReader ( path );
		string text = streamReader.ReadToEnd();
		streamReader.Close();

		return text;
	}
}


public class ExternalInformation : MonoBehaviour
{


}
