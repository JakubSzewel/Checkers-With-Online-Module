[System.Serializable]
public class Net_OnSideChoice : NetMsg {
    public Net_OnSideChoice() {
        OP = NetOP.OnSideChoice;
    }
    public bool Color { set; get; }
    public bool TimePlay { set; get; }
    public int RoundTime { set; get; }
}
