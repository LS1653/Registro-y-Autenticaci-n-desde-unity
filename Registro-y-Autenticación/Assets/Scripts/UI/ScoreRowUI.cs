using TMPro;
using UnityEngine;

public class ScoreRowUI : MonoBehaviour
{
    public TMP_Text positionText;
    public TMP_Text usernameText;
    public TMP_Text scoreText;

    public void SetData(
        int position,
        string username,
        int score)
    {
        positionText.text = position.ToString();
        usernameText.text = username;
        scoreText.text = score.ToString();
    }
}