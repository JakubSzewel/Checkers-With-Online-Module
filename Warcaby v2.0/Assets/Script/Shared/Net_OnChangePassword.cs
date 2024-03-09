[System.Serializable]
public class Net_OnChangePassword : NetMsg {
    public Net_OnChangePassword() {
        OP = NetOP.OnChangePassword;
    }

    public byte MsgID { set; get; }
}
