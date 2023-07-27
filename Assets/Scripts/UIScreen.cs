using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public virtual UIScreenID ID { get; protected set; }

    public virtual void Init()
    {
        
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public  void Hide()
    {
        gameObject.SetActive(false);
    }
}
