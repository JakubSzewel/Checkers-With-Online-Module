using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyScene : MonoBehaviour {
    public GameObject toggler;
    public GameObject MinToggler3;
    public GameObject MinToggle5;
    public GameObject MinToggler10;
    private void Start() {
        if (Client.Instance.GuestFlag == true) {
            GameObject.Find("WaitingPanel").SetActive(false);
            GameObject panel = GameObject.Find("Canvas").FindObject("ColorChoicePanel");
            panel.gameObject.SetActive(true);
            string text = "You have connected!";
            GameObject.Find("OpponentJoinMsg").GetComponent<TextMeshProUGUI>().text = text;
            Client.Instance.GuestFlag = false;
        }

    }
    public void ChooseWhite() {
        Client.Instance.SendSideChoice(true);
        SceneManager.LoadScene(2);
    }
    public void ChooseBlack() {
        Client.Instance.SendSideChoice(false);
        SceneManager.LoadScene(2);
    }
    public void EnableTimePlayToggle() {
        bool tog = toggler.GetComponent<Toggle>().isOn;
        if (tog == true) {
            GameObject.Find("ColorChoicePanel").FindObject("TimerPanel").SetActive(true);
            Client.Instance.TimePlay = true;
        }
        else {
            GameObject.Find("ColorChoicePanel").FindObject("TimerPanel").SetActive(false);
            Client.Instance.TimePlay = false;
        }
    }
    public void ThreeMinuteTimer() {
        Client.Instance.RoundTime = 3;
    }
    public void FiveMinuteTimer() {
        Client.Instance.RoundTime = 5;
    }
    public void TenMinuteTimer() {
        Client.Instance.RoundTime = 10;
    }

}
