using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameObject AchievementsPanel;
    [SerializeField] private GameObject SettingsPanel;
    private void Start()
    {
        AchievementsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenAchievements()
    {
        AchievementsPanel.SetActive(true);
    }
    public void OpenSettings()
    {
        SettingsPanel.SetActive(true);
    }

}
