[System.Serializable]
public class Net_ScanTables : NetMsg {
    public Net_ScanTables() {
        OP = NetOP.ScanTables;
    }
}