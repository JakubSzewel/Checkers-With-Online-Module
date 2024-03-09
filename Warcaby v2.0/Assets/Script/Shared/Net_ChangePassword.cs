[System.Serializable]
public class Net_ChangePassword : NetMsg {
    public Net_ChangePassword() {
        OP = NetOP.ChangePassword;
    }

    public string Username { set; get; }
    public string OldPassword { set; get; }
    public string NewPassword { set; get; }
}
