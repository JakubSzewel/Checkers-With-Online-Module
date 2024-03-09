using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class Client : MonoBehaviour {
    public static Client Instance { private set; get; } // Do odwo³ywania siê do zmiennych zawartych w klasie Client

    private const int MAX_USER = 10; // Maksymalna liczba u¿ytkowników
    private const int PORT = 6201; // Numer portu, za którego pomoc¹ ³¹czy siê z serwerem
    private const int BYTE_SIZE = 1024; // Wielkoœæ wiadomoœci przesy³anej miêdzy serwerem a klientem 
    private const string SERVER_IP = "127.0.0.1"; // Adres IP serwera
    
    private byte reliableChannel; // rodzaj po³¹czenia z serwerem
    private int connectionID; // Numer ID po³¹czenia klienta z serwerem
    private bool isStarted; // Czy nawi¹zane jest po³¹czenie
    private int hostID; // Rodzaj po³¹czenia klienta
    private byte error; // Kod b³êdu przy po³¹czeniu

    private string Username; // Nazwa u¿ytkownika po zalogowaniu
    public GameObject TableButtonPrefab; // Przycisk dodawany do listy dostêpnych sto³ów
    public GameObject ChatMsgPrefab; // wiadomoœæ wyœwietlana na czacie

    private int TableID; // Numer ID sto³u z którym u¿ytkownik jest po³¹czony

    public bool GuestFlag = false; // Czy do³¹czaj¹cy do lobby u¿ytkownik jest goœciem
    
    public bool MultiplayerEnabled = false; // Czy gra wieloosoboawa jest w³¹czona
    public bool YourColor; // Kolor pionków u¿ytkownika
    public Net_MakeMove OpMove; // Ruch przeciwnika
    public bool OpMadeAMove; // Czy przeciwnik wykona³ ruch

    public bool TimePlay = false; // Czy rozgrywka na czas jest w³¹czona
    public int RoundTime; // Czas w minutach jaki ma trwaæ rozgrywka jednej ze stron


    #region Monobehaviour
    private void Start() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update() {
        UpdateMessagePump();
    }
    #endregion

    public void Init() { // Ustawienie wartoœci pocz¹tkowych
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topo = new HostTopology(cc, MAX_USER);

        //

        hostID = NetworkTransport.AddHost(topo, 0);

        connectionID = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);
        isStarted = true;
    }
    public void Shutdown() { // Zerwanie po³¹czenia
        isStarted = false;
        NetworkTransport.Shutdown();
    }
    public void UpdateMessagePump() { // Nawi¹zanie po³¹czenia z serwerem
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
                Debug.Log("We have connected");
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("We have been disconnected");
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

    #region Send
    public void SendServer(NetMsg msg) {
        // Tu przechowywane s¹ dane
        byte[] buffer = new byte[BYTE_SIZE];

        // Zamiana danych na byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion

    #region OnData
    private void OnData(int cnnID, int chID, int hID, NetMsg msg) {
        //Debug.Log("Recieved a msg of type " + msg.OP);
        switch (msg.OP) {
            case NetOP.None:
                Debug.Log("Unexpected NETOP");
                break;
            case NetOP.OnCreateAccount:
                OnCreateAccount((Net_OnCreateAccount)msg);
                break;
            case NetOP.OnLoginRequest:
                OnLoginRequest((Net_OnLoginRequest)msg);
                break;
            case NetOP.OnScanTables:
                OnScanTables((Net_OnScanTables)msg);
                break;
            case NetOP.OnCreateTable:
                OnCreateTable((Net_OnCreateTable)msg);
                break;
            case NetOP.OnJoinTable:
                OnJoinTable((Net_OnJoinTable)msg);
                break;
            case NetOP.OnSideChoice:
                OnSideChoice((Net_OnSideChoice)msg);
                break;
            case NetOP.OnOpponentJoin:
                OnOpponentJoin((Net_OnOpponentJoin)msg);
                break;
            case NetOP.MakeMove:
                OnMakeMove((Net_MakeMove)msg);
                break;
            case NetOP.OnChangePassword:
                OnChangePassword((Net_OnChangePassword)msg);
                break;
            case NetOP.OnChangeUsername:
                OnChangeUsername((Net_OnChangeUsername)msg);
                break;
            case NetOP.OnDeleteAccount:
                OnDeleteAccount((Net_OnDeleteAccount)msg);
                break;
            case NetOP.OnChatMsg:
                OnChatMsg((Net_OnChatMsg)msg);
                break;
        }

    }
    public void SendCreateAccount(string u, string p) {
        if (u.Contains(' ') || p.Contains(' '))
            return;
        Net_CreateAccount ca = new Net_CreateAccount();
        ca.Username = u;
        ca.Password = p;
        SendServer(ca);
    }
    private void OnCreateAccount(Net_OnCreateAccount oca) {
        if (oca.MsgID == 1) {
            GameObject panel = GameObject.Find("Canvas").FindObject("SameUsernamePupup");
            panel.gameObject.SetActive(true);
        }
    }
    public void SendLoginRequest(string u, string p) {
        if (u.Contains(' ') || p.Contains(' '))
            return;
        Net_LoginRequest log = new Net_LoginRequest();
        log.Username = u;
        log.Password = p;
        SendServer(log);
    }
    private void OnLoginRequest(Net_OnLoginRequest olr) {
        if (olr.MsgID == 0) {
            Username = GameObject.Find("InputLogin").GetComponent<TMP_InputField>().text;
            SceneManager.LoadScene(1);
        }
        if(olr.MsgID == 1) {
            GameObject panel = GameObject.Find("Canvas").FindObject("UsernameDoesntExistPupup");
            panel.gameObject.SetActive(true);
        }
        if (olr.MsgID == 2) {
            GameObject panel = GameObject.Find("Canvas").FindObject("WrongPasswordPupup");
            panel.gameObject.SetActive(true);
        }
    }

    public void SendChangePassword(string op, string np) {
        if (np.Contains(' '))
            return;
        Net_ChangePassword cp = new Net_ChangePassword();
        cp.Username = Username;
        cp.OldPassword = op;
        cp.NewPassword = np;
        SendServer(cp);
    }
    private void OnChangePassword(Net_OnChangePassword ocp) {
        // Password succesfully changed popup
    }
    public void SendChangeUsername(string nu, string p) {
        if (nu.Contains(' '))
            return;
        Net_ChangeUsername cu = new Net_ChangeUsername();
        cu.NewUsername = nu;
        cu.Password = p;
        cu.Username = Username;
        SendServer(cu);
    }
    private void OnChangeUsername(Net_OnChangeUsername ocu) {
        if (ocu.MsgID == 0) {
            Username = ocu.NewUsername;
        }
    }
    public void SendDeleteAccount(string u, string p) {
        if (Username != u)
            return;
        Net_DeleteAccount da = new Net_DeleteAccount();
        da.Username = u;
        da.Password = p;
        SendServer(da);
    }
    private void OnDeleteAccount(Net_OnDeleteAccount oda) {
        if (oda.MsgID == 0) {
            SceneManager.LoadScene(0);
        }
    }

    public void SendScanTables() {
        Net_ScanTables st = new Net_ScanTables();
        SendServer(st);
    }
    private void OnScanTables(Net_OnScanTables ost) {
        for (int i = 0; i < ost.Tables.Count; i++) {
            GameObject go = Instantiate(TableButtonPrefab);
            tableButton t = go.GetComponent<tableButton>();
            t.changeText(ost.Tables[i]);
        }
    }
    public void SendCreateTable(string n, string p) {
        if (n.Contains(' ') || p.Contains(' '))
            return;
        Net_CreateTable ct = new Net_CreateTable();
        ct.TableName = n;
        ct.Password = p;
        ct.Username = Username;
        SendServer(ct);
    }
    private void OnCreateTable(Net_OnCreateTable oct) {
        if (oct.MsgID == 0) {
            SceneManager.LoadScene(5);
            TableID = oct.TableID;
        }
        //if (oct.MsgID == 1)
        // wyœwietl coœ, ¿e istnieje stó³ o takiej nazwie
    }
    public void SendJoinTable(string n, string p) {
        if (n.Contains(' ') || p.Contains(' '))
            return;
        Net_JoinTable jt = new Net_JoinTable();
        jt.TableName = n;
        jt.Password = p;
        jt.Username = Username;
        SendServer(jt);
    }
    private void OnJoinTable(Net_OnJoinTable ojt) {
        if (ojt.MsgID == 0) {
            SceneManager.LoadScene(5);
            TableID = ojt.TableID;
            GuestFlag = true;
        }
        if (ojt.MsgID == 1) {
            GameObject panel = GameObject.Find("Canvas").FindObject("WrongPasswordPopup");
            panel.gameObject.SetActive(true);
        }
    }

    public void SendSideChoice(bool c) {
        MultiplayerEnabled = true;
        Net_SideChoice sc = new Net_SideChoice();
        sc.Color = c;
        sc.TimePlay = TimePlay;
        sc.RoundTime = RoundTime;
        YourColor = c;
        SendServer(sc);
    }
    private void OnSideChoice(Net_OnSideChoice osc) {
        MultiplayerEnabled = true;
        YourColor = osc.Color;
        TimePlay = osc.TimePlay;
        RoundTime = osc.RoundTime;
        SceneManager.LoadScene(2);
    }
    private void OnOpponentJoin(Net_OnOpponentJoin ooj) {
        GameObject.Find("WaitingPanel").SetActive(false);
        GameObject panel = GameObject.Find("Canvas").FindObject("ColorChoicePanel");
        panel.gameObject.SetActive(true);
        string text = ooj.OpponentName + " have connected!";
        GameObject.Find("OpponentJoinMsg").GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SendMakeMove(int bX, int bY, int x, int y, bool ifKilled, int kX, int kY, bool turnEnd) {
        Net_MakeMove mm = new Net_MakeMove();
        mm.bX = bX;
        mm.bY = bY;
        mm.x = x;
        mm.y = y;
        mm.ifKilled = ifKilled;
        mm.kX = kX;
        mm.kY = kY;
        mm.turnEnd = turnEnd;
        mm.TableID = TableID;
        SendServer(mm);
    }
    private void OnMakeMove(Net_MakeMove omm) {
        Debug.Log("tak");
        OpMove = omm;
        OpMadeAMove = true;
    }

    public void SendChatMsg(string m) {
        Net_ChatMsg cm = new Net_ChatMsg();
        cm.Msg = m;
        cm.TableID = TableID;
        cm.Username = Username;
        SendServer(cm);
    }
    private void OnChatMsg(Net_OnChatMsg ocm) {
        GameObject go = Instantiate(ChatMsgPrefab);
        chatMsg c = go.GetComponent<chatMsg>();
        string text = ocm.Username + ": " + ocm.Msg;
        c.changeText(text);
    }
    #endregion
}
