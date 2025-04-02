using System;
using UnityEngine;

public static class JsonDataManager<T>
{
    // Json -> <T>
    // <T> -> Json

    public static string ToJson(T data)
    {
        string json = JsonUtility.ToJson(data);

        if(string.IsNullOrEmpty(json))
        {
            Debug.LogError("Not Serializable Data");
            return null;
        }

        return AesEncryption.Encrypt(json);
    }

    public static T FromJson(string json)
    {
        string decryptedJson = AesEncryption.Decrypt(json);

        return JsonUtility.FromJson<T>(decryptedJson);
    }
}