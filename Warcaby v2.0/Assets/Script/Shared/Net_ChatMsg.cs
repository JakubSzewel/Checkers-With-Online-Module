[System.Serializable]
public class Net_ChatMsg : NetMsg {
    public Net_ChatMsg() {
        OP = NetOP.ChatMsg;
    }

    public string Msg { set; get; }
    public int TableID { set; get; }
    public string Username { set; get; }
}