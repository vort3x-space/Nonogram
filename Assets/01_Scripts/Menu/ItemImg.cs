using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImg : MonoBehaviour {

    [HideInInspector]
    public Sprite sprite;
    private Image image;
    private Button button;
    [HideInInspector]
    public int value;


    void Awake()
    {
        image = this.GetComponentsInChildren<Image>()[1];
        button = this.GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Menu.getInstance().LoadGameQuestion(value);
        });
    }

    void Start()
    {
        image.sprite = this.sprite;
    }
}
