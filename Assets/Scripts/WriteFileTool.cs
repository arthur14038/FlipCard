using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class WriteFileTool {
	public static void WriteFile(string filePath, string content)
	{		
//		#if UNITY_EDITOR 
//		Debug.Log(string.Format("WriteFile -- filePath: {0} \ncontent: {1}", filePath, content));
//		#endif
		StreamWriter sw;
		FileInfo fi = new FileInfo(filePath);
		if(fi.Exists)
		{			
			DeleteFile(filePath);
		}
		sw = fi.CreateText();
		sw.WriteLine(content);
		sw.Close();
		sw.Dispose();
	}

	public static string LoadFile(string filePath)
	{
		StreamReader sr = null;
		try{
			sr = File.OpenText(filePath);
		}catch
		{
			return null;
		}
		string content = sr.ReadLine();
		sr.Close();
		sr.Dispose();
		
//		#if UNITY_EDITOR 
//		Debug.Log(string.Format("LoadFile -- filePath: {0} \ncontent: {1}", filePath, content));
//		#endif
		return content;
	}

	public static void DeleteFile(string filePath)
	{
		try
		{
			File.Delete(filePath);
		} catch
		{
		}
		//		#if UNITY_EDITOR 
		//		Debug.Log(string.Format("DeleteFile -- filePath: {0}", filePath));
		//		#endif
	}
}
