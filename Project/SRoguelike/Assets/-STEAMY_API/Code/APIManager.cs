using System;
using RestSharp;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
//Written by Michael Bethke
public class User
{
	
	public APIKey apikey = new APIKey ();
	public Me me = new Me ();
}


public class APIKey
{
	
	public string id { get; set; }
	public string secret { get; set; }
}


public class Me
{
	
	public string userName { get; set; }
}


public class APIManager : MonoBehaviour
{
	
	internal APIKey apiKey;
	internal Me me;
	
	
	public User UserLogin ( string username, string password )
	{
		
		User user = new User ();
		
		RestClient client = new RestClient ( "https://steamy.io:443" );
		client.Authenticator = new HttpBasicAuthenticator ( username, password );

		RestRequest apikeyRequest = new RestRequest ( "api/ApiKey", Method.POST );
		apikeyRequest.RequestFormat = DataFormat.Json;
		IRestResponse apikeyResponse = client.Execute ( apikeyRequest );
		
		apiKey = JsonConvert.DeserializeObject<APIKey> ( apikeyResponse.Content );
		
		if ( String.IsNullOrEmpty ( apiKey.id ) == true || String.IsNullOrEmpty ( apiKey.secret ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to POST for APIKey, check Username and Password" );
			return null;
		}
		
		user.apikey = apiKey;
		
		client.Authenticator = new HttpBasicAuthenticator ( apiKey.id, apiKey.secret );
		
		RestRequest meRequest = new RestRequest ( "api/Me", Method.GET );
		meRequest.RequestFormat = DataFormat.Json;
		IRestResponse meResponse = client.Execute ( meRequest );
		
		me = JsonConvert.DeserializeObject<Me> ( meResponse.Content );
		
		if ( String.IsNullOrEmpty ( me.userName ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to GET for Me, this account could be corrupted!" );
			return null;
		}
		
		user.me = me;
		
		return user;
	}
}
