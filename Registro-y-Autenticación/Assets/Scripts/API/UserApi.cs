using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UserApi : MonoBehaviour
{
    private const string Url =
        "https://sid-restapi.onrender.com/api/usuarios";

    public IEnumerator GetUsers(
        string token,
        System.Action<User[]> onSuccess)
    {
        using UnityWebRequest request =
            UnityWebRequest.Get(Url);

        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Usuarios recibidos:");
            Debug.Log(request.downloadHandler.text);

            UserListResponse response =
                JsonUtility.FromJson<UserListResponse>(
                    request.downloadHandler.text
                );

            onSuccess?.Invoke(response.usuarios);
        }
        else
        {
            Debug.LogError("Error obteniendo usuarios:");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }

    public IEnumerator UpdateScore(
    string token,
    string username,
    int score,
    System.Action<User> onSuccess)
    {
        UserUpdateData updateData = new UserUpdateData();
    
        updateData.username = username;
        updateData.data = new UserDat();
        updateData.data.score = score;
    
        string jsonData = JsonUtility.ToJson(updateData);
    
        Debug.Log("Updating score: " + jsonData);
    
        using UnityWebRequest request =
            new UnityWebRequest(Url, "PATCH");
    
        byte[] bodyRaw =
            System.Text.Encoding.UTF8.GetBytes(jsonData);
    
        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);
    
        request.downloadHandler =
            new DownloadHandlerBuffer();
    
        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );
    
        request.SetRequestHeader(
            "x-token",
            token
        );
    
        yield return request.SendWebRequest();
    
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score actualizado:");
            Debug.Log(request.downloadHandler.text);
        
            UserUpdateResponse response =
                JsonUtility.FromJson<UserUpdateResponse>(
                    request.downloadHandler.text
                );
        
            onSuccess?.Invoke(response.usuario);
        }
        else
        {
            Debug.LogError("Error actualizando score:");
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }
}