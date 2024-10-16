using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagement : MonoBehaviour
{
    public TextMeshProUGUI Point; 

    void Update()
    {
        Point.text = Enemy.totalScore.ToString("F0");
    }
}
