[System.Serializable]
public class Net_JoinTable : NetMsg {
    public Net_JoinTable() {
        OP = NetOP.JoinTable;
    }

    public string TableName { set; get; }
    public string Password { set; get; }
    public string Username { set; get; }
}
