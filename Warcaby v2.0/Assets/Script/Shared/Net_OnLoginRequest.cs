[System.Serializable]
public class Net_OnLoginRequest : NetMsg {
    public Net_OnLoginRequest() {
        OP = NetOP.OnLoginRequest;
    }

    public byte MsgID { set; get; } // 0 - zalogowany, 1 - zly login, 2 - zle haslo
}
