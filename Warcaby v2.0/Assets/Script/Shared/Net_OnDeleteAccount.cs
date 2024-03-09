[System.Serializable]
public class Net_OnDeleteAccount : NetMsg {
    public Net_OnDeleteAccount() {
        OP = NetOP.OnDeleteAccount;
    }

    public byte MsgID { set; get; }
}
