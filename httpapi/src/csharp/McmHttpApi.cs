/*
 * Copyright (c) 2011 Global Mouth AB
 * Need to add System.Web to References in order to compile! (Needed for HttpUtility)
 */
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace MCM
{
	public class McmHttpApi
	{
		char[] hexMap = {'0','1','2','3','4','5','6','7','8','9','a','b','c','d','e','f' };
		
		public static void Main(string[] args)
		{
			McmHttpApi api = new McmHttpApi();
			api.sendMessage("myusername", "mypassword", "myoriginator", "mymsisdn", "mybody" );
		}
		
		public void sendMessage( string UserName, string Password, string Originator, string Msisdn, string Body ) {
			sendMessage( UserName, Password, Originator, Msisdn, Body, false, null );
		}
		
		public void sendMessage( string UserName, string Password, string Originator, string Msisdn, string Body, bool DeliveryReport, string Reference )
		{
			string Parameters = "?";
			Parameters += "username=" + HttpUtility.UrlEncode( UserName ) + "&";
			Parameters += "body=" + HttpUtility.UrlEncode( Body ) + "&";
			Parameters += "msisdn=" + HttpUtility.UrlEncode( Msisdn ) + "&";
			Parameters += "dlr=" + HttpUtility.UrlEncode( Convert.ToString( DeliveryReport ) ) + "&";
			if( DeliveryReport ) 
			{
				Parameters += "ref=" + HttpUtility.UrlEncode( Reference ) + "&";
			}
			if( Originator != null && !"".Equals( Originator ) )
			{
				Parameters += "originator=" + HttpUtility.UrlEncode( Originator ) + "&";
			}
			Parameters += "hash=" + getMD5Hash( UserName, Password, new string[] { Body, Originator, Msisdn } );
			
			HttpWebRequest Request = (HttpWebRequest)WebRequest.Create( "http://mcm.globalmouth.com:8080/api/mcm" + Parameters );
			HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
		}
		
		private string getMD5Hash( string UserName, string Password, string[] values ) 
		{
			string Hash = UserName;
			for( int i = 0; i < values.Length; i++ )
			{
				if( values[i] != null ) Hash += values[i];
			}
			string pwHash = ConvertToMD5Hex( UserName + ":" + Password );
			return ConvertToMD5Hex( Hash + pwHash );
		}
		
		private string ConvertToMD5Hex( string str ) 
		{
			MD5 Md5 = MD5.Create();
			byte[] hashedBytes = Encoding.UTF8.GetBytes( str );
			hashedBytes = Encoding.Convert( Encoding.UTF8, Encoding.GetEncoding( "ISO-8859-1" ), hashedBytes );
			
			hashedBytes = Md5.ComputeHash( hashedBytes );
			
			StringBuilder sb = new StringBuilder();
			
			for( int i = 0; i < hashedBytes.Length; i++ ) {
				sb.Append( hexMap[(hashedBytes[i]&0xF0) >> 4] );
				sb.Append( hexMap[hashedBytes[i]&0x0F] );
			}
						
			return sb.ToString();
		}
	}
}
