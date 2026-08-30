using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private const string Url = "https://sid-restapi.onrender.com";

    private string token = "";
    private string username = "";

    [Header("Login")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;

    public void RegisterButtonClick()
    {
      StartCoroutine(RegisterUser());
    }


    private IEnumerator RegisterUser()
    {
        AuthData authData = new AuthData();
    
        authData.username = usernameInput.text;
        authData.password = passwordInput.text;
    
        string jsonData = JsonUtility.ToJson(authData);
    
        Debug.Log("Sending JSON data: " + jsonData);
    
        UnityWebRequest request =
            UnityWebRequest.Post(
                Url + "/api/usuarios",
                jsonData,
                "application/json"
            );
    
        yield return request.SendWebRequest();
    
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
    
            SetStatus("No se pudo registrar el usuario.");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
    
            UserResponse userResponse =
                JsonUtility.FromJson<UserResponse>(
                    request.downloadHandler.text
                );
    
            Debug.Log(
                "User registered: " +
                userResponse.usuario.username
            );
    
            SetStatus("Usuario registrado correctamente.");
        }
    }


    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}

