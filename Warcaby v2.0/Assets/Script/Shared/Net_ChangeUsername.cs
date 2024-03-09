[System.Serializable]
public class Net_ChangeUsername : NetMsg {
    public Net_ChangeUsername() {
        OP = NetOP.ChangeUsername;
    }

    public string NewUsername { set; get; }
    public string Password { set; get; }
    public string Username { set; get; }
}
