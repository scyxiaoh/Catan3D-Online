using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveGameSystem
{
	public static bool SaveGame(SaveGame saveGame, string name)
	{
//		BinaryFormatter formatter = new BinaryFormatter();
//		Debug.Log (GetSavePath(name));
//		using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
//		{
//			try
//			{
//				Debug.Log("Lets try");
//				formatter.Serialize(stream, saveGame);
//			}
//			catch (Exception)
//			{
//				return false;
//			}
//		}
//
//		return true;
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream;

		try
		{
			if (File.Exists(GetSavePath(name)))
			{
				File.WriteAllText(GetSavePath(name), string.Empty);
				fileStream = File.Open(GetSavePath(name), FileMode.Open);
			}
			else
			{
				fileStream = File.Create(GetSavePath(name));
			}

			Debug.Log("file is created");
			binaryFormatter.Serialize(fileStream, saveGame);
			fileStream.Close();

		}
		catch (Exception e)
		{
			Debug.Log (e);
			return false;
		}

		return true;
	}

	public static SaveGame LoadGame(string name)
	{
		if (!DoesSaveGameExist(name))
		{
			return null;
		}

		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
		{
			try
			{
				return formatter.Deserialize(stream) as SaveGame;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

	public static bool DeleteSaveGame(string name)
	{
		try
		{
			File.Delete(GetSavePath(name));
		}
		catch (Exception)
		{
			return false;
		}

		return true;
	}

	public static bool DoesSaveGameExist(string name)
	{
		return File.Exists(GetSavePath(name));
	}

	private static string GetSavePath(string name)
	{
		return Path.Combine(Application.persistentDataPath, name + ".sav");
	}
}