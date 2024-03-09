using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BoardScene : MonoBehaviour {
    float currentTimeWhite; // Obecny czas bia³ego gracza
    float currentTimeBlack; // Obecny czas czarnego gracza
    float startingTime = 600f; // Wybrany czas rozgrywki
    public Text BlackTimeText; // Tekst wyœwietlaj¹cy czas czarnego gracza
    public Text WhiteTimeText; // Tekst wyœwietlaj¹cy czas bia³ego gracza
    public Text VictoriousSideText; // Tekst wyœwietlaj¹cy stronê która wygra³a

    private void Start() {
        if (Client.Instance.MultiplayerEnabled == false) {
            GameObject.Find("ChatMsgField").SetActive(false);
            GameObject.Find("ChatPanel").SetActive(false);
            GameObject.Find("SendButton").SetActive(false);
        }
        if(Client.Instance.TimePlay == true) {
            GameObject.Find("Canvas").FindObject("WhiteTimer").SetActive(true);
            GameObject.Find("Canvas").FindObject("BlackTimer").SetActive(true);
            if (Client.Instance.RoundTime == 10)
                startingTime = 600f;
            else if (Client.Instance.RoundTime == 5)
                startingTime = 300f;
            else if (Client.Instance.RoundTime == 3)
                startingTime = 180f;
            currentTimeBlack = startingTime;
            currentTimeWhite = startingTime;
        }
    }
    private void Update() {
        if (Client.Instance.TimePlay == true) {
            if (currentTimeBlack > 0 && currentTimeWhite > 0) {
                if (Logic.Instance.turn == false) {
                    currentTimeBlack -= 1 * Time.deltaTime;
                    BlackTimeText.text = currentTimeBlack.ToString("0");
                }
                else {
                    currentTimeWhite -= 1 * Time.deltaTime;
                    WhiteTimeText.text = currentTimeWhite.ToString("0");
                }
            }
            else if (currentTimeBlack <= 0 || Logic.Instance.NumOfBlack <= 0) {
                WhiteWinsPopup();
            }
            else if (currentTimeWhite <= 0 || Logic.Instance.NumOfWhite <= 0) {
                BlackWinsPopup();
            }
        }
    }

    public void SendMsgButton() {
        string m = GameObject.Find("ChatMsgField").GetComponent<TMP_InputField>().text;
        Client.Instance.SendChatMsg(m);
    }

    public void WhiteWinsPopup() {
        GameObject.Find("Canvas").FindObject("EndGamePanel").SetActive(true);
        VictoriousSideText.text = "White Wins!";
    }

    public void BlackWinsPopup() {
        GameObject.Find("Canvas").FindObject("EndGamePanel").SetActive(true);
        VictoriousSideText.text = "Black Wins!";
    }

    public void BackToMenuButton() {
        SceneManager.LoadScene(1);
    }
}
