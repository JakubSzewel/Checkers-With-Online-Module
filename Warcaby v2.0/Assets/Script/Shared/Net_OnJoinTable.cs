[System.Serializable]
public class Net_OnJoinTable : NetMsg {
    public Net_OnJoinTable() {
        OP = NetOP.OnJoinTable;
    }

    public byte MsgID { set; get; }
    public int TableID { set; get; }
}
