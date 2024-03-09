using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public static class Objects {
    public static GameObject FindObject(this GameObject parent, string name) { // Funkcja dodana do biblioteki Unity, który znajdujê nieaktywny obiekt, ¿eby potem mo¿na go by³o aktywowaæ
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs) {
            if (t.name == name) {
                return t.gameObject;
            }
        }
        return null;
    }
}

public class LoginScene : MonoBehaviour {
    public void OnClickCreateAccount() {
        string Login = GameObject.Find("CreateLogin").GetComponent<TMP_InputField>().text;
        string Password = GameObject.Find("CreatePassword").GetComponent<TMP_InputField>().text;
        Client.Instance.SendCreateAccount(Login, Password);
    }

    public void OnClickCreateRequest() {
        string Login = GameObject.Find("InputLogin").GetComponent<TMP_InputField>().text;
        string Password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;
        Client.Instance.SendLoginRequest(Login, Password);
    }
    public void QuitGame() {
        Application.Quit();
    }
    public void ExitSUPupup() {
        GameObject.Find("SameUsernamePupup").SetActive(false);
    }
    public void ExitUDEPupup() {
        GameObject.Find("UsernameDoesntExistPupup").SetActive(false);
    }
    public void ExitWPPupup() {
        GameObject.Find("WrongPasswordPupup").SetActive(false);
    }
    public void PlayOffline() {
        SceneManager.LoadScene(1);
    }
}
