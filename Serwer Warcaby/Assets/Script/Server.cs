using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {
    
    private const int MAX_USER = 10;
    private const int PORT = 6201;
    private const int BYTE_SIZE = 1024;

    private byte reliableChannel;
    private int hostID;

    private bool isStarted;
    private byte error;

    List<Table> tables = new List<Table>();
    int numOfTables = 0;

    #region Monobehaviour
    private void Start() {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update() {
        UpdateMessagePump();
    }
    #endregion

    public void Init() {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        //

        hostID = NetworkTransport.AddHost(topo, PORT, null);

        isStarted = true;
    }
    public void Shutdown() {
        isStarted = false;
        NetworkTransport.Shutdown();
    }
    public void UpdateMessagePump() {
        if (!isStarted)
            return;

        int connectionID;
        int channelID;

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out hostID, out connectionID, out channelID, recBuffer, BYTE_SIZE, out dataSize, out error);
        switch (type) {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("User {0} has connected!", connectionID));
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User {0} has disconnected!", connectionID));
                break;
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);
                OnData(connectionID, channelID, hostID, msg);
                break;
            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("Unexpected network event type");
                break;
        } 
    }

    #region OnData
    private void OnData(int cnnID, int chID, int hID, NetMsg msg) {
        //Debug.Log("Recieved a msg of type " + msg.OP);
        switch (msg.OP) {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.CreateAccount:
                CreateAccount(cnnID, chID, hID, (Net_CreateAccount)msg);
                break;
            case NetOP.LoginRequest:
                LoginRequest(cnnID, chID, hID, (Net_LoginRequest)msg);
                break;
            case NetOP.CreateTable:
                CreateTable(cnnID, chID, hID, (Net_CreateTable)msg);
                break;
            case NetOP.JoinTable:
                JoinTable(cnnID, chID, hID, (Net_JoinTable)msg);
                break;
            case NetOP.ScanTables:
                ScanTables(cnnID, chID, hID, (Net_ScanTables)msg);
                break;
            case NetOP.SideChoice:
                SideChoice(cnnID, chID, hID, (Net_SideChoice)msg);
                break;
            case NetOP.MakeMove:
                MakeMove(cnnID, chID, hID, (Net_MakeMove)msg);
                break;
            case NetOP.ChangePassword:
                ChangePassword(cnnID, chID, hID, (Net_ChangePassword)msg);
                break;
            case NetOP.ChangeUsername:
                ChangeUsername(cnnID, chID, hID, (Net_ChangeUsername)msg);
                break;
            case NetOP.DeleteAccount:
                DeleteAccount(cnnID, chID, hID, (Net_DeleteAccount)msg);
                break;
            case NetOP.ChatMsg:
                ChatMsg(cnnID, chID, hID, (Net_ChatMsg)msg);
                break;
        }

    }
    private void CreateAccount(int cnnID, int chID, int hID, Net_CreateAccount ca) {
        string path = @"c:\Users\kubas\Serwer Warcaby\Assets\DataBase\Dane Logowania.txt";
        Net_OnCreateAccount oca = new Net_OnCreateAccount();

        foreach (string line in File.ReadLines(path)) {
            string[] words = line.Split(' ');
            if (words[0] == ca.Username) { // Czy dana nazwa u¿ytkownika ju¿ istnieje
                oca.MsgID = 1;
                break;
            }
        }
        if (oca.MsgID != 1) {
            using (StreamWriter w = File.AppendText(path))
                w.WriteLine(ca.Username + " " + ca.Password);
            oca.MsgID = 0;
        }
        SendClient(cnnID, oca);

    }
    private void LoginRequest(int cnnID, int chID, int hID, Net_LoginRequest lr) {
        string path = @"c:\Users\kubas\Serwer Warcaby\Assets\DataBase\Dane Logowania.txt";
        Net_OnLoginRequest olr = new Net_OnLoginRequest();

        foreach (string line in File.ReadLines(path)) {
            string[] words = line.Split(' ');
            olr.MsgID = 1;
            if (words[0] == lr.Username && words[1] == lr.Password) { // Czy dana nazwa u¿ytkownika ju¿ istnieje
                olr.MsgID = 0;
                break;
            }
            else if (words[0] == lr.Username && words[1] != lr.Password) {
                olr.MsgID = 2;
                break;
            }
        }
        SendClient(cnnID, olr);
    }
    private void CreateTable(int cnnID, int chID, int hID, Net_CreateTable ct) {
        Net_OnCreateTable oct = new Net_OnCreateTable();
        for (int i = 0; i < tables.Count; i++) {
            if(tables[i].Name == ct.TableName) {
                oct.MsgID = 1;
                break;
            }
        }
        if (oct.MsgID != 1) {
            numOfTables++;
            tables.Add(new Table());
            tables[numOfTables - 1].Name = ct.TableName;
            tables[numOfTables - 1].Password = ct.Password;
            tables[numOfTables - 1].HostID = cnnID;
            tables[numOfTables - 1].HostUsername = ct.Username;
            tables[numOfTables - 1].ID = numOfTables - 1;
            tables[numOfTables - 1].Occupied = false;
            oct.TableID = tables[numOfTables - 1].ID;
            oct.MsgID = 0;
        }
        SendClient(cnnID, oct);
    }
    private void JoinTable(int cnnID, int chID, int hID, Net_JoinTable jt) {
        Net_OnJoinTable ojt = new Net_OnJoinTable();
        Net_OnOpponentJoin ooj = new Net_OnOpponentJoin();
        ojt.MsgID = 1;
        for (int i = 0; i < tables.Count; i++) {
            if (tables[i].Name == jt.TableName) {
                if (tables[i].Password == jt.Password) {
                    tables[i].GuestID = cnnID;
                    tables[i].GuestUsername = jt.Username;
                    tables[i].Occupied = true;
                    ojt.TableID = tables[i].ID;
                    ojt.MsgID = 0;
                    ooj.OpponentName = jt.Username;
                    SendClient(tables[i].HostID, ooj);
                }
                break;
            }
        }
            
        SendClient(cnnID, ojt);
    }
    private void ScanTables(int cnnID, int chID, int hID, Net_ScanTables st) {
        Net_OnScanTables ost = new Net_OnScanTables();
        List<string> AvailableTables = new List<string>();
        for (int i = 0; i < tables.Count; i++) {
            if (tables[i].Occupied == false)
                AvailableTables.Add(tables[i].Name);
        }
        ost.Tables = AvailableTables;
        SendClient(cnnID, ost);
    }
    private void SideChoice(int cnnID, int chID, int hID, Net_SideChoice sc) {
        Net_OnSideChoice osc = new Net_OnSideChoice();
        osc.TimePlay = sc.TimePlay;
        osc.RoundTime = sc.RoundTime;
        for (int i = 0; i < tables.Count; i++) {
            if (sc.TableID == tables[i].ID) {
                if (cnnID == tables[i].HostID) {
                    if (sc.Color == true) {
                        tables[i].HostColor = true;
                        osc.Color = false;
                        SendClient(tables[i].GuestID, osc);
                        break;
                    }
                    else {
                        tables[i].HostColor = false;
                        osc.Color = true;
                        SendClient(tables[i].GuestID, osc);
                        break;
                    }
                }
                else if (cnnID == tables[i].GuestID) {
                    if (sc.Color == true) {
                        tables[i].HostColor = false;
                        osc.Color = false;
                        SendClient(tables[i].HostID, osc);
                        break;
                    }
                    else {
                        tables[i].HostColor = true;
                        osc.Color = true;
                        SendClient(tables[i].HostID, osc);
                        break;
                    }
                }
            }
        }
    }
    private void MakeMove(int cnnID, int chID, int hID, Net_MakeMove mm) {
        Net_MakeMove omm = new Net_MakeMove();
        omm.bX = mm.bX;
        omm.bY = mm.bY;
        omm.x = mm.x;
        omm.y = mm.y;
        omm.ifKilled = mm.ifKilled;
        omm.kX = mm.kX;
        omm.kY = mm.kY;
        omm.turnEnd = mm.turnEnd;
        for (int i = 0; i < tables.Count; i++) {
            if (mm.TableID == tables[i].ID) {
                if (cnnID == tables[i].HostID)
                    SendClient(tables[i].GuestID, omm);
                else if (cnnID == tables[i].GuestID)
                    SendClient(tables[i].HostID, omm);
                break;
            }
        }
    }
    private void ChangePassword(int cnnID, int chID, int hID, Net_ChangePassword cp) {
        string path = @"c:\Users\kubas\Serwer Warcaby\Assets\DataBase\Dane Logowania.txt";
        Net_OnChangePassword ocp = new Net_OnChangePassword();
        string text = cp.Username + " " + cp.OldPassword;
        string[] arrLine = File.ReadAllLines(path);
        ocp.MsgID = 1;
        for (int i = 0; i < arrLine.Length; i++) {
            if (arrLine[i] == text) {
                arrLine[i] = cp.Username + " " + cp.NewPassword;
                File.WriteAllLines(path, arrLine);
                ocp.MsgID = 0;
                break;
            }
        }
        SendClient(cnnID, ocp);
    }
    private void ChangeUsername(int cnnID, int chID, int hID, Net_ChangeUsername cu) {
        string path = @"c:\Users\kubas\Serwer Warcaby\Assets\DataBase\Dane Logowania.txt";
        Net_OnChangeUsername ocu = new Net_OnChangeUsername();
        string text = cu.Username + " " + cu.Password;
        string[] arrLine = File.ReadAllLines(path);
        ocu.MsgID = 1;
        for (int i = 0; i < arrLine.Length; i++) {
            if (arrLine[i] == text) {
                arrLine[i] = cu.NewUsername + " " + cu.Password;
                File.WriteAllLines(path, arrLine);
                ocu.MsgID = 0;
                ocu.NewUsername = cu.NewUsername;
                break;
            }
        }
        SendClient(cnnID, ocu);
    }
    private void DeleteAccount(int cnnID, int chID, int hID, Net_DeleteAccount da) {
        string path = @"c:\Users\kubas\Serwer Warcaby\Assets\DataBase\Dane Logowania.txt";
        Net_OnDeleteAccount oda = new Net_OnDeleteAccount();
        string text = da.Username + " " + da.Password;
        string[] arrLine = File.ReadAllLines(path);
        oda.MsgID = 1;
        for (int i = 0; i < arrLine.Length; i++) {
            if (arrLine[i] == text) {
                arrLine[i] = "";
                File.WriteAllLines(path, arrLine);
                oda.MsgID = 0;
                break;
            }
        }
        SendClient(cnnID, oda);
    }
    private void ChatMsg(int cnnID, int chID, int hID, Net_ChatMsg cm) {
        Net_OnChatMsg ocm = new Net_OnChatMsg();
        ocm.Msg = cm.Msg;
        ocm.Username = cm.Username;
        for (int i = 0; i < tables.Count; i++) {
            if (cm.TableID == tables[i].ID) {
                SendClient(tables[i].GuestID, ocm);
                SendClient(tables[i].HostID, ocm);
                break;
            }
        }
    }
    #endregion

    #region Send
    public void SendClient(int cnnID, NetMsg msg) {
        // Tu przechowywane s¹ dane
        byte[] buffer = new byte[BYTE_SIZE];

        // Zamiana danych na byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostID, cnnID, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion
}

public class Table {
    public int ID;
    public string Name;
    public string Password;
    public int HostID;
    public string HostUsername;
    public int GuestID;
    public string GuestUsername;
    public bool Occupied;
    public bool HostColor;
}
