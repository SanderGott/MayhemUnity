// 1/2/2026 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;

    private void Start()
    {
        // Ensure buttons are assigned
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(() => SwitchScene("SampleScene"));
        }

        if (clientButton != null)
        {
            clientButton.onClick.AddListener(() => SwitchScene("SampleScene"));
        }
    }

    private void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}