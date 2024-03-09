[System.Serializable]
public class Net_SideChoice : NetMsg {
    public Net_SideChoice() {
        OP = NetOP.SideChoice;
    }

    public bool Color { set; get; }
    public bool TimePlay { set; get; }
    public int RoundTime { set; get; }
    public int TableID { set; get; }
}
