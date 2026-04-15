using UnityEngine;

public class ButtonPlayGameBehaviour : MonoBehaviour
{
    public void OnButtonClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
