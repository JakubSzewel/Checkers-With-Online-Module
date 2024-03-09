[System.Serializable]
public class Net_OnChangeUsername : NetMsg {
    public Net_OnChangeUsername() {
        OP = NetOP.OnChangeUsername;
    }

    public byte MsgID { set; get; }
    public string NewUsername { set; get; }
}
