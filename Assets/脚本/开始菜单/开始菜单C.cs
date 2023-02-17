using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 开始菜单C : MonoBehaviour
{
    [SerializeField] private Button 开始按钮;
    [SerializeField] private Button 提出按钮;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void 开始游戏()
    {
        SceneManager.LoadScene(1);
    }

    public void 提出游戏()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
