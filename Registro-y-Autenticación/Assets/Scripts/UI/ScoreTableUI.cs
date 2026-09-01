using System.Linq;
using UnityEngine;

public class ScoreTableUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject scoreRowPrefab;

    public void ShowUsers(User[] users)
    {
        ClearTable();

        User[] sortedUsers =
            users
            .OrderByDescending(user => user.data.score)
            .ToArray();

        for (int i = 0; i < sortedUsers.Length; i++)
        {
            GameObject rowObject =
                Instantiate(
                    scoreRowPrefab,
                    content
                );

            ScoreRowUI row =
                rowObject.GetComponent<ScoreRowUI>();

            row.SetData(
                i + 1,
                sortedUsers[i].username,
                sortedUsers[i].data.score
            );
        }
    }

    private void ClearTable()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }
}