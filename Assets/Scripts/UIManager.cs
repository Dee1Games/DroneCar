using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UIScreen[] Screens;

    private UIScreen currentScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        TutorialManager.Instance.Hide();
        TutorialManager.Instance.HideHand();
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