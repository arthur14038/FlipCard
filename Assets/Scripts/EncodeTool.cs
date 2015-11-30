using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;

public class EncodeTool{
	public static string GetEncodedBase64(string source, string key, string iv)
	{
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();
		byte[] Key = Encoding.ASCII.GetBytes(key);
		byte[] IV = Encoding.ASCII.GetBytes(iv);
		byte[] dataByteArray = Encoding.UTF8.GetBytes(source);
		
		des.Key = Key;
		des.IV = IV;
		string encrypt = "";
		using (MemoryStream ms = new MemoryStream())
			using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
		{
			cs.Write(dataByteArray, 0, dataByteArray.Length);
			cs.FlushFinalBlock();
			encrypt = Convert.ToBase64String(ms.ToArray());
		}
		return encrypt;
	}	
	
	public static string GetDecodedBase64(string encodedData, string key, string iv)
	{
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();
		byte[] Key = Encoding.ASCII.GetBytes(key);
		byte[] IV = Encoding.ASCII.GetBytes(iv);
		des.Key = Key;
		des.IV = IV;
		
		byte[] dataByteArray = Convert.FromBase64String(encodedData);
		using (MemoryStream ms = new MemoryStream())
		{
			using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
			{
				cs.Write(dataByteArray, 0, dataByteArray.Length);
				cs.FlushFinalBlock();
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}
	}
}
