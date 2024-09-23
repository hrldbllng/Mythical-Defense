using UnityEngine;

public class OpenURLOnClick : MonoBehaviour
{
    public string url = "https://docs.google.com/forms/d/e/1FAIpQLSdZsWuC2ZpxyeMYTQWgW3en2ji2LnWarKN_Qw2MQlDdt4RIPg/viewform?pli=1";

    // Function to open the specified URL
    public void OpenURL()
    {
        Application.OpenURL(url);
    }
}
