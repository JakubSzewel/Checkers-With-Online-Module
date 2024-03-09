using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class tableButton : MonoBehaviour {
    public TextMeshProUGUI text;
    void Start() {
        GameObject Panel = GameObject.Find("TablesPanel");
        gameObject.transform.parent = Panel.transform;
    }
    public void changeText(string t)
    {
        text.text = t;
    }
    public void SelectTable() {
        GameObject panel = GameObject.Find("Canvas").FindObject("EnterPasswordPopup");
        panel.gameObject.SetActive(true);
        GameObject.Find("TableNameText").GetComponent<TextMeshProUGUI>().text = text.text;
    }
}
