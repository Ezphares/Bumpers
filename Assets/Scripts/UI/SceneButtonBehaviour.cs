using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class SceneButtonBehaviour : MonoBehaviour {

    public enum SceneTarget
    {
        Custom,
        Retry,
        Menu,
        Next
    }

    public SceneTarget target;
    public string customSceneName;
    public Text text;

    [HideInInspector] [SerializeField] Button button;

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        string sceneName = "";

        switch (target)
        {
            case SceneTarget.Retry:
                sceneName = SceneManager.GetActiveScene().name;
                break;

            case SceneTarget.Menu:
                sceneName = "MenuScene";
                break;

            case SceneTarget.Next:
                string[] split = SceneManager.GetActiveScene().name.Split(new char[]{'_'});
                int num = int.Parse(split[1]);
                sceneName = split[0] + "_" + (num + 1).ToString();

                if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    gameObject.SetActive(false);
                }

                break;

            default:
                sceneName = customSceneName;
                break;
        }

        button.onClick.AddListener(() => {
            SceneManager.LoadScene(sceneName);
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
