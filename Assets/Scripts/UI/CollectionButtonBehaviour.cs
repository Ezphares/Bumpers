using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CollectionButtonBehaviour : MonoBehaviour {

    public PuzzleCollection collection;
    public GameObject parentMenu;
    public GameObject targetMenu;
    public RectTransform targetMenuContent;
    public SceneButtonBehaviour buttonPrefab;
    public Text text;

    // Use this for initialization
    void Start() {
        text.text = collection.title;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            parentMenu.SetActive(false);
            targetMenu.SetActive(true);

            foreach (Transform child in targetMenuContent)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            while (true)
            {
                string sceneName = collection.prefix + (i + 1).ToString();

                if (!Application.CanStreamedLevelBeLoaded(sceneName))
                {
                    Debug.Log("Broke at " + sceneName);
                    break;
                }

                SceneButtonBehaviour newButton = Instantiate(buttonPrefab, targetMenuContent);
                RectTransform newRect = newButton.transform as RectTransform;

                newRect.anchorMin = new Vector2(0.0f, 1.0f);
                newRect.anchorMax = new Vector2(1.0f, 1.0f);
                newRect.pivot = new Vector2(0.5f, 1.0f);
                newRect.anchoredPosition = Vector3.down * 32.0f * i;

                newButton.target = SceneButtonBehaviour.SceneTarget.Custom;
                newButton.customSceneName = sceneName;

                i++;

                newButton.text.text = string.Format("{0} {1}", collection.title, i);

            }

            targetMenuContent.sizeDelta = new Vector2(targetMenuContent.sizeDelta.x, 32.0f * i);

        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
