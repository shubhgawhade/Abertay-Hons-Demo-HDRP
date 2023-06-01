using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public static class PersistentSave
{
    public static string Location = "dat.dat";
    
    public static void Save()
    {
        string path = Path.Combine(Application.persistentDataPath, Location);
        
        PlayerData data = new PlayerData();

        // Serialize game data into JSON string
        string jsonString = JsonUtility.ToJson(data);

        // Create FileStream for writing
        FileStream dataStream = new FileStream(path, FileMode.Create, FileAccess.Write);

        // Create Aes object for encryption
        using (Aes iAes = Aes.Create())
        {
            // TO HAVE ONLY 1 KEY PER PC
            if (!PlayerPrefs.HasKey("iv"))
            {
                string keyString = Convert.ToBase64String (iAes.Key);
                string ivString = Convert.ToBase64String (iAes.IV);
            
                // Debug.Log($"{keyString} : {ivString}");
                
                // Store them somewhere securely
                PlayerPrefs.SetString ("iv", keyString);
                PlayerPrefs.SetString ("key", ivString);
            }
            
            // Create CryptoStream for writing
            // CryptoStream iStream = new CryptoStream(dataStream, iAes.CreateEncryptor(iAes.Key, iAes.IV), CryptoStreamMode.Write);
            CryptoStream iStream = new CryptoStream(dataStream, iAes.CreateEncryptor(Convert.FromBase64String (PlayerPrefs.GetString("iv")), Convert.FromBase64String (PlayerPrefs.GetString("key"))), CryptoStreamMode.Write);

            // Create StreamWriter for writing
            StreamWriter sWriter = new StreamWriter(iStream);


            // Write JSON string to the innermost stream (which will encrypt it)
            sWriter.Write(jsonString);

            // Close all streams
            sWriter.Close();
            iStream.Close();
            dataStream.Close();
        }
    }

    public static PlayerData Load()
    {
        string path = Path.Combine(Application.persistentDataPath, Location);
        // Debug.Log(File.Exists(path));
        if (File.Exists(path))
        {
            // Debug.Log($"{PlayerPrefs.GetString("key")} : {PlayerPrefs.GetString("iv")}");
            byte[] keyBytes = Convert.FromBase64String (PlayerPrefs.GetString("iv"));
            byte[] ivBytes = Convert.FromBase64String (PlayerPrefs.GetString("key"));
            
            // Create FileStream for reading
            FileStream dataStream = new FileStream(path, FileMode.Open);

            // Create Aes object for decryption
            using (Aes iAes = Aes.Create())
            {
                // Create CryptoStream for reading
                CryptoStream iStream = new CryptoStream(dataStream, iAes.CreateDecryptor(keyBytes, ivBytes), CryptoStreamMode.Read);

                // Create StreamReader for reading
                StreamReader sReader = new StreamReader(iStream);

                // Read JSON string from the innermost stream (which will decrypt it)
                string jsonString = sReader.ReadToEnd();

                // Deserialize JSON string into game data object
                PlayerData data = JsonUtility.FromJson<PlayerData>(jsonString);

                // Close all streams
                sReader.Close();
                iStream.Close();
                dataStream.Close();
                
                GameManager.hasSave = true;
                return data;
            }
        }
        else
        {
            
            return null;
        }

    }
}