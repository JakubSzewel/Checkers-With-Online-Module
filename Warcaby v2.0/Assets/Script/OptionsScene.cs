using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class OptionsScene : MonoBehaviour {

    public void back() {
        SceneManager.LoadScene(1);
    }

    public void openPCPopup() {
        GameObject panel1 = GameObject.Find("Canvas").FindObject("PasswordChangePopup");
        panel1.gameObject.SetActive(true);
    }
    public void confirmPCPopup() {
        string op = GameObject.Find("OldPasswordField").GetComponent<TMP_InputField>().text;
        string np = GameObject.Find("NewPasswordField").GetComponent<TMP_InputField>().text;
        Client.Instance.SendChangePassword(op, np);
        GameObject.Find("PasswordChangePopup").SetActive(false);
    }

    public void openUCPopup() {
        GameObject panel2 = GameObject.Find("Canvas").FindObject("UsernameChangePopup");
        panel2.gameObject.SetActive(true);
    }
    public void confirmUCPopup() {
        string p = GameObject.Find("PasswordField").GetComponent<TMP_InputField>().text;
        string nu = GameObject.Find("NewUsernameField").GetComponent<TMP_InputField>().text;
        Client.Instance.SendChangeUsername(nu, p);
        GameObject.Find("UsernameChangePopup").SetActive(false);
    }

    public void openADPopup() {
        GameObject panel3 = GameObject.Find("Canvas").FindObject("DeleteAccountPopup");
        panel3.gameObject.SetActive(true);
    }
    public void confirmADPopup() {
        string u = GameObject.Find("UsernameDeleteField").GetComponent<TMP_InputField>().text;
        string p = GameObject.Find("PasswordDeleteField").GetComponent<TMP_InputField>().text;
        GameObject.Find("DeleteAccountPopup").SetActive(false);
        Client.Instance.SendDeleteAccount(u, p);
    }

    public void openRCPopup() {
        GameObject panel4 = GameObject.Find("Canvas").FindObject("ResolutionChangePopup");
        panel4.gameObject.SetActive(true);
    }
    public void confirmRCPopup() {
        GameObject.Find("ResolutionChangePopup").SetActive(false);
    }

    
    

    public void SetScreenRes() {
        string index = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        switch (index) {
            case "1152x648Button":
                Screen.SetResolution(1152, 648, true);
                break;
            case "1280x720Button":
                Screen.SetResolution(1280, 720, true);
                break;
            case "1360x764Button":
                Screen.SetResolution(1360, 764, true);
                break;
            case "1920x1080Button":
                Screen.SetResolution(1920, 1080, true);
                break;
        }
    }
}
