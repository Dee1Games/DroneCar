using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public UIScreen[] Screens;

    private UIScreen currentScreen;

    public Image giantGunTimer;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void SetGiantGunTimer(float value)
    {
        if (giantGunTimer)
        {
            giantGunTimer.fillAmount = value;
        }
    }

    public void Init()
    {
        foreach (UIScreen screen in Screens)
        {
            screen.Init();
            screen.Hide();
        }
    }

    public void ShowScreen(UIScreenID screenID)
    {
        if (currentScreen != null)
        {
            currentScreen.Hide();
        }

        foreach (UIScreen screen in Screens)
        {
            if (screen.ID == screenID)
            {
                currentScreen = screen;
                currentScreen.Show();
                return;
            }
        }
    }

    public void Refresh()
    {
        currentScreen.Show();
    }
}