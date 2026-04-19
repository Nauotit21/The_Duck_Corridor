using UnityEngine;

public class ButtonConfirmQuitBehaviour : MonoBehaviour
{
    public void OnButtonClick()
    {
        Application.Quit();
    }
}
