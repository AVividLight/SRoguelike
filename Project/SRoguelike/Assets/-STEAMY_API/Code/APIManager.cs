using System;
using RestSharp;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
//Written by Michael Bethke
public class User
{
	
	public APIKey apikey = new APIKey ();
	public UserAccount userAccount = new UserAccount ();
}


public class APIKey
{
	
	public string id { get; set; }
	public string secret { get; set; }
}


public class UserAccount
{
	
	public int accessFailedCount { get; set; }
	public string email { get; set; }
	public bool emailConfirmed { get; set; }
	public string id { get; set; }
	public bool lockoutEnabled { get; set; }
	//public byte lockoutEndDateUtc { get; set; } //Might be DateTime, not implemented in API yet.
	public string passwordHash { get; set; }
	//public byte phoneNumber { get; set; } //Might be string, not implemented in API yet.
	public bool phoneNumberConfirmed { get; set; }
	public List<String> roles { get; set; }
	public bool twoFactorEnabled { get; set; }
	public string userName { get; set; }
	public string status { get; set; }
}


public class APIManager : MonoBehaviour
{	
	
	public User UserLogin ( string username, string password )
	{
		
		User user = new User ();
		
		RestClient client = new RestClient ( "https://kimochi.co:443" );
		
		user.apikey = GetKey ( client, username, password );
		user.userAccount = GetUserAccount ( client, username, password );
		
		//UnityEngine.Debug.Log ( GetFrontPage ( client, user ));
		//UnityEngine.Debug.Log ( GetDownloadHealth ( client, user ));
		
		return user;
	}
	
	
	private APIKey GetKey ( RestClient client, string username, string password )
	{
		
		APIKey newKey = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( username, password );

		RestRequest request = new RestRequest ( "api/ApiKey", Method.POST );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		newKey = JsonConvert.DeserializeObject<APIKey> ( response.Content );
		
		if ( String.IsNullOrEmpty ( newKey.id ) == true || String.IsNullOrEmpty ( newKey.secret ) == true )
		{
			
			UnityEngine.Debug.Log ( "Unable to POST for APIKey, check Username and Password" );
			return null;
		}
		
		return newKey;
	}
	
	
	private UserAccount GetUserAccount ( RestClient client, string username, string password )
	{
		
		UserAccount newUserAccount = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( username, password );
		
		RestRequest request = new RestRequest ( "api/Account", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		newUserAccount = JsonConvert.DeserializeObject<UserAccount> ( response.Content );
		
		if ( String.IsNullOrEmpty ( newUserAccount.userName ) == true ) //Not the best way to check for a failure, this is old.
		{
			
			UnityEngine.Debug.Log ( "Unable to GET for Me, this account could be corrupted!" );
			return null;
		}
		
		return newUserAccount;
	}
	
	
	private string GetFrontPage ( RestClient client, User user )
	{
		
		string frontPage = null;
		
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		
		RestRequest request = new RestRequest ( "api/Frontpage", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		
		frontPage = response.Content;
		
		if ( String.IsNullOrEmpty ( frontPage ) == true )
		{
		
			UnityEngine.Debug.Log ( "Unable to GET for FrontPage, something went wrong!" );
			return null;
		}
		
		return frontPage;
	}
	
	
	private string GetDownloadHealth ( RestClient client, User user )
	{
			
		string downloadHealth = null;
		    
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		    
		RestRequest request = new RestRequest ( "api/download/health", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		    
		downloadHealth = response.Content;
		    
		if ( String.IsNullOrEmpty ( downloadHealth ) == true )
		{
		    
			UnityEngine.Debug.Log ( "Unable to GET for Download Health, something went wrong!" );
			return null;
		}
		    
		return downloadHealth;
	}
	
	
	private string GetAPIVersion ( RestClient client, User user )
	{
			
		string apiVersion = null;
		    
		client.Authenticator = new HttpBasicAuthenticator ( user.apikey.id, user.apikey.secret );
		    
		RestRequest request = new RestRequest ( "api/Version", Method.GET );
		request.RequestFormat = DataFormat.Json;
		IRestResponse response = client.Execute ( request );
		    
		apiVersion = response.Content;
		    
		if ( String.IsNullOrEmpty ( apiVersion ) == true )
		{
		    
			UnityEngine.Debug.Log ( "Unable to GET for API Version, something went wrong!" );
			return null;
		}
		    
		return apiVersion;
	}
}
