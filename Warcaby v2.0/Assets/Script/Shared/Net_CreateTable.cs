[System.Serializable]
public class Net_CreateTable : NetMsg {
    public Net_CreateTable() {
        OP = NetOP.CreateTable;
    }

    public string TableName { set; get; }
    public string Password { set; get; }
    public string Username { set; get; }
}
