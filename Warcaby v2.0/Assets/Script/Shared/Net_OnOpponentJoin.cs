[System.Serializable]
public class Net_OnOpponentJoin : NetMsg {
    public Net_OnOpponentJoin() {
        OP = NetOP.OnOpponentJoin;
    }

    public string OpponentName { set; get; }
}
