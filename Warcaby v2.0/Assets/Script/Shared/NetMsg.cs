public static class NetOP {
    public const int None = 0;

    public const int CreateAccount = 1;
    public const int LoginRequest = 2;
    public const int OnCreateAccount = 3;
    public const int OnLoginRequest = 4;

    public const int CreateTable = 5;
    public const int JoinTable = 6;
    public const int OnCreateTable = 7;
    public const int OnJoinTable = 8;

    public const int MakeMove = 9;

    public const int ScanTables = 11;
    public const int OnScanTables = 12;

    public const int SideChoice = 13;
    public const int OnSideChoice = 14;
    public const int OnOpponentJoin = 15;

    public const int ChangePassword = 16;
    public const int OnChangePassword = 17;
    public const int ChangeUsername = 18;
    public const int OnChangeUsername = 19;
    public const int DeleteAccount = 20;
    public const int OnDeleteAccount = 21;

    public const int ChatMsg = 22;
    public const int OnChatMsg = 23;

}

[System.Serializable]
public abstract class NetMsg {
    public byte OP { set; get; }

    public NetMsg() {
        OP = NetOP.None;
    }
}