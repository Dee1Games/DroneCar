using UnityEngine;

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

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        foreach (UIScreen screen in Screens)
        {
            screen.Init();
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
}