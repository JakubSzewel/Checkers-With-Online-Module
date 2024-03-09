[System.Serializable]
public class Net_MakeMove : NetMsg {
    public Net_MakeMove() {
        OP = NetOP.MakeMove;
    }

    public int bX { set; get; } // Pozycja poczatkowa
    public int bY { set; get; } 
    public int x { set; get; }  // Pozycja koncowa
    public int y { set; get; }
    public bool ifKilled { set; get; } // Czy jaki� pionek zosta� zbity
    public int kX { set; get; } // Pozycja zbitego pionka
    public int kY { set; get; }
    public bool turnEnd { set; get; }
    public int TableID { set; get; } // ID sto�u
}
