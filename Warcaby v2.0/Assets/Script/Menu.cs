using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    public Client client1;
    private string u1;
    private string p1;
    private string u2;
    private string p2;
    public void Start() {
    }

    public void PlayGame() {
        GameObject panel = GameObject.Find("Menu").FindObject("ChooseGamemodePanel");
        panel.gameObject.SetActive(true);
    }

    public void gotologowanie() {
        SceneManager.LoadScene(3);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void Readlogin1Input(string s) {
        u1 = s;
    }
    public void Readpassword1Input(string s) {
        p1 = s;
    }
    public void Readlogin2Input(string s) {
        u2 = s;
    }
    public void Readpassword2Input(string s) {
        p2 = s;
    }
    public void calllogin() { 
    }
    public void callregister() {
    }
    public void Hotseat() {
        SceneManager.LoadScene(2);
    }
    public void Multiplayer() {
        SceneManager.LoadScene(4);
    }
}
