using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public virtual UIScreenID ID { get; protected set; }

    public virtual void Init()
    {
        
    }
    
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
