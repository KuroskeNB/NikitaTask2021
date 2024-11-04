using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    // Ключи для сохранения значений
    private const string finishedLevelsKey = "FinishedLevels";
    private const string inRowRewardsKey = "inRowRewards";

     private void Awake()
    {
        // check if there is no duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveValues(int finishedLevels, int inRowRewards)
    {
        PlayerPrefs.SetInt(finishedLevelsKey, finishedLevels);
        PlayerPrefs.SetInt(inRowRewardsKey, inRowRewards);
        PlayerPrefs.Save(); // save on disk
        Debug.Log("Values saved: finishedLevels = " + finishedLevels + ", inRowRewards = " + inRowRewards);
    }
    
    public void ClearSaves()
    {
        Debug.Log("clear saves");
        PlayerPrefs.DeleteAll();
    }
    public int LoadFinishedLevels()
    {
        return PlayerPrefs.GetInt(finishedLevelsKey, 0);
    }

    public int LoadInRowRewards()
    {
        return PlayerPrefs.GetInt(inRowRewardsKey, 0);
    }
}


