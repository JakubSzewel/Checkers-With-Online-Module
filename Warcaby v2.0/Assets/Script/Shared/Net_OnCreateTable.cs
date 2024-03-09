[System.Serializable]
public class Net_OnCreateTable : NetMsg {
    public Net_OnCreateTable() {
        OP = NetOP.OnCreateTable;
    }

    public byte MsgID { set; get; }
    public int TableID { set; get; }
}
