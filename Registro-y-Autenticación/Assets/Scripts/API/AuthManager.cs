using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private const string Url = "https://sid-restapi.onrender.com";

    private string token = "";
    private string username = "";

    [Header("Register")]
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    
    [Header("Login")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;

    private void Start()
    {
        token = PlayerPrefs.GetString("token", "");
        username = PlayerPrefs.GetString("username", "");
    
        if (!string.IsNullOrEmpty(token) &&
            !string.IsNullOrEmpty(username))
        {
            StartCoroutine(GetProfile());
        }
        else
        {
            ShowLogin();
        }
    }

    public void RegisterButtonClick()
    {
      StartCoroutine(RegisterUser());
    }

    public void LoginButtonClick()
    {
      StartCoroutine(Login());
    }


    private IEnumerator RegisterUser()
    {
        AuthData authData = new AuthData();
    
        authData.username = registerUsernameInput.text;
        authData.password = registerPasswordInput.text;
    
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


    private IEnumerator Login()
    {
        AuthData authData = new AuthData();
    
        authData.username = loginUsernameInput.text;
        authData.password = loginPasswordInput.text;
    
        string jsonData = JsonUtility.ToJson(authData);
    
        Debug.Log("Sending login data: " + jsonData);
    
        UnityWebRequest request =
            UnityWebRequest.Post(
                Url + "/api/auth/login",
                jsonData,
                "application/json"
            );
    
        yield return request.SendWebRequest();
    
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
    
            SetStatus("Usuario o contraseña incorrectos.");
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
    
            UserResponse userResponse =
                JsonUtility.FromJson<UserResponse>(
                    request.downloadHandler.text
                );
    
            token = userResponse.token;
            username = userResponse.usuario.username;
            
            // Guardamos la sesión
            PlayerPrefs.SetString("token", token);
            PlayerPrefs.SetString("username", username);
            PlayerPrefs.Save();
            
            Debug.Log("Login correcto.");
            Debug.Log("Usuario: " + username);
            Debug.Log("Token guardado correctamente.");
    
            SetStatus("Inicio de sesión correcto.");
        }
    }


    private IEnumerator GetProfile()
    {
        UnityWebRequest request =
            UnityWebRequest.Get(
                Url + "/api/usuarios/" + username
            );
    
        request.SetRequestHeader("x-token", token);
    
        yield return request.SendWebRequest();
    
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            Debug.LogError(request.downloadHandler.text);
    
            LogoutButtonClick();
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
    
            UserResponse userResponse =
                JsonUtility.FromJson<UserResponse>(
                    request.downloadHandler.text
                );
    
            Debug.Log(
                "Token válido. Usuario: " +
                userResponse.usuario.username
            );
    
            ShowProfile(userResponse.usuario.username);
        }
    }

    

    public void LogoutButtonClick()
    {
        token = "";
        username = "";
    
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.Save();
    
        ShowLogin();
    }   

    private void ShowLogin()
    {
        Debug.Log("Mostrando pantalla de Login.");
    }
    
    private void ShowProfile(string displayName)
    {
        Debug.Log("Usuario autenticado: " + displayName);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}

