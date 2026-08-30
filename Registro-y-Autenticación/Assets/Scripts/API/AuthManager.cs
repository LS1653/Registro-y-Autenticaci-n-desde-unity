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
}