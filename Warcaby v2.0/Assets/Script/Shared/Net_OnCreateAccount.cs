[System.Serializable]
public class Net_OnCreateAccount : NetMsg {
    public Net_OnCreateAccount() {
        OP = NetOP.OnCreateAccount;
    }

    public byte MsgID { set; get; } // 0 - konto stworzone, 1 - istnieje taki login
}
