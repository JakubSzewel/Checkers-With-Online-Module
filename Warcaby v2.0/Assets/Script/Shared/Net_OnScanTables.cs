using System.Collections.Generic;
[System.Serializable]
public class Net_OnScanTables : NetMsg {
    public Net_OnScanTables() {
        OP = NetOP.OnScanTables;
    }

    public List<string> Tables { set; get; }
}