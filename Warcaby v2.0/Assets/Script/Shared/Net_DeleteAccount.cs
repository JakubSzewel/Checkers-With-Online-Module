[System.Serializable]
public class Net_DeleteAccount : NetMsg {
    public Net_DeleteAccount() {
        OP = NetOP.DeleteAccount;
    }

    public string Username { set; get; }
    public string Password { set; get; }
}
