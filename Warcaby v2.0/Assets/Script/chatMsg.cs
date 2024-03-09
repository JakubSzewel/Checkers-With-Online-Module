using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chatMsg : MonoBehaviour {
    public Text text;
    void Start() {
        GameObject Panel = GameObject.Find("ChatContent");
        gameObject.transform.parent = Panel.transform;
    }
    public void changeText(string t) {
        text.text = t;
    }
}
