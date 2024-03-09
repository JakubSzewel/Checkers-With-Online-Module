using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSelectionScene : MonoBehaviour
{
    public string CurrentTableName;
    private void Start() {
        Client.Instance.SendScanTables();
    }

    public void CreateTable() {
        string TableName = GameObject.Find("CreateNameField").GetComponent<TMP_InputField>().text;
        string Password = GameObject.Find("CreatePasswordField").GetComponent<TMP_InputField>().text;
        Client.Instance.SendCreateTable(TableName, Password);
    }

    public void JoinTable() {
        string Password = GameObject.Find("JoinPasswordField").GetComponent<TMP_InputField>().text;
        string CurrentTableName = GameObject.Find("TableNameText").GetComponent<TextMeshProUGUI>().text;
        Client.Instance.SendJoinTable(CurrentTableName, Password);
    }
    public void ScanAgain() {
        GameObject Panel = GameObject.Find("TablesPanel");
        foreach (Transform child in Panel.transform)
            GameObject.Destroy(child.gameObject);
        Client.Instance.SendScanTables();
    }
    public void ExitEPPupup() {
        GameObject.Find("EnterPasswordPopup").SetActive(false);
    }
    public void ExitWPPupup() {
        GameObject.Find("WrongPasswordPopup").SetActive(false);
    }
    public void Back() {
        SceneManager.LoadScene(1);
    }
}
