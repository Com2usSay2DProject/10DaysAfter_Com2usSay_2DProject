using System;
using UnityEngine;

public static class JsonDataManager // 문의 : 수민
{
    // Json -> <T>
    // <T> -> Json

    public static string ToJson<T>(T data)
    {
        try
        {
            string json = JsonUtility.ToJson(data);

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Serialization failed: Data is not serializable.");
                return default;
            }

            return AesEncryption.Encrypt(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"ToJson Error: {ex.Message}");
            return default;
        }
    }

    public static T FromJson<T>(string json)
    {
        try
        {
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogError("Decryption failed: Empty JSON string.");
                return default;
            }

            string decryptedJson = AesEncryption.Decrypt(json);

            if (string.IsNullOrEmpty(decryptedJson))
            {
                Debug.LogError("Decryption failed: Resulting JSON is empty.");
                return default;
            }

            return JsonUtility.FromJson<T>(decryptedJson);
        }
        catch (Exception ex)
        {
            Debug.LogError($"FromJson Error: {ex.Message}");
            return default;
        }
    }
}
