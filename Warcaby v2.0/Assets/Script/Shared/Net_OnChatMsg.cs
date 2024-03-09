[System.Serializable]
public class Net_OnChatMsg : NetMsg {
    public Net_OnChatMsg() {
        OP = NetOP.OnChatMsg;
    }

    public string Msg { set; get; }
    public string Username { set; get; }
}