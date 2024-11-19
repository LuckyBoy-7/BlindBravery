using Lucky.Framework;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

namespace Lucky.Kits.QuickScripts
{
    public class QuickMenu : ManagedBehaviour
    {
        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}