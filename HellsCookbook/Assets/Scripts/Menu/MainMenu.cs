using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform directionalLight;
    [SerializeField] private float lightSpeed = 5f;
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Update()
    {
        directionalLight.Rotate(lightSpeed * Time.deltaTime, 0, 0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void About()
    {
        aboutPanel.SetActive(true);
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        if (Application.isPlaying)
        {
            Application.Quit();
        }
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }
}
