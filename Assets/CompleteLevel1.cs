using UnityEngine;

public class CompleteLevel1 : MonoBehaviour
{
    // This method should be called when Level 1 is completed
    public void MarkLevel1Complete()
    {
        PlayerPrefs.SetInt("Level1Completed", 1);
        PlayerPrefs.Save();
    }
}