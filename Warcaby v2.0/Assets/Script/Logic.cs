using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour {
    public static Logic Instance { private set; get; } // Do odwo³ywania siê do zmiennych zawartych w klasie Logic
    public Board b;

    private Piece SelectedPiece; // Wybrany pionek
    int spX, spY; // Pozycja wybranego pionka

    public Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector3 endDrag;

    public Vector3 boardOffset = new Vector3(-5.095f, 0, -4.0f);
    public Vector3 pieceOffset = new Vector3(0.05f, 0, -0.92f);

    private bool turnEnd = false; // 0 - trwa tura, 1 - koniec
    public bool turn = true; // 0 - czarny, 1 - bia³y

    private bool flagMove = false; // Czy ruch zosta³ wykonany

    private Piece Killer = null; // Przechowanie pionka który dokona³ zbicia
    private int killerX, killerY;
    private bool flagKill = false;

    private int kX;
    private int kY;
    private int bX;
    private int bY;

    List<List<int>> PossibleMove = new List<List<int>>(); // Lista wspó³rzêdnych z mo¿liwymi ruchami
    List<List<int>> PossibleKill = new List<List<int>>(); // Lista mozliwych zbiæ

    private bool MultiplayerEnabled;
    private bool YourColor;
    private bool MadeAMove;

    public int NumOfWhite = 20;
    public int NumOfBlack = 20;

    private void Start() {
        Instance = this;
        GameObject.Find("VideoPlayer").SetActive(false);
        MultiplayerEnabled = Client.Instance.MultiplayerEnabled;
        if (MultiplayerEnabled == true)
            YourColor = Client.Instance.YourColor;
        SelectedPiece = GameObject.FindObjectOfType(typeof(Piece)) as Piece;
    }

    //
    // Loop
    //

    private void Update() {
        if (MultiplayerEnabled == true && turn != YourColor) {
            if (Client.Instance.OpMadeAMove == true) {
                Net_MakeMove om = Client.Instance.OpMove;
                if (om.ifKilled == true) {
                    if (b.pArr[om.kX, om.kY].color == true)
                        NumOfWhite--;
                    else
                        NumOfBlack--;
                    b.Death(om.kX, om.kY);
                }
                b.MovePiece(om.bX, om.bY, om.x, om.y);
                if (om.y == 0 && b.pArr[om.x, om.y].color == false)
                    b.Queen(om.x, om.y);
                if (om.y == 9 && b.pArr[om.x, om.y].color == true)
                    b.Queen(om.x, om.y);
                if (om.turnEnd == true) {
                    if (turn == true)
                        turn = false;
                    else
                        turn = true;
                }
                Client.Instance.OpMadeAMove = false;
            }
        }
        else {
            MouseUpdate();
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;
            if (Input.GetMouseButtonDown(0)) {
                b.DestroyPossibleMove(PossibleMove); // Usuwa oznaczenia mo¿liwgo ruchu
                SelectPiece(x, y); // Klikniêcie na pionka
                Move(x, y); // Wykonanie ruchu je¿eli jakiœ pionek zosta³ poprzednio wybrany
                PassTurn(x, y); // Sprawdzenie czy tura siê zakoñczy³a
                if (!turnEnd && SelectedPiece) {
                    Analyze(spX, spY); // Anaiza Ruchu
                    b.DrawPossibleMove(spX, spY, PossibleMove);
                }
                if (MultiplayerEnabled == true && MadeAMove == true) {
                    Client.Instance.SendMakeMove(bX, bY, x, y, flagKill, kX, kY, turnEnd);
                }
            }
        }
    }

    //
    //  Obs³uga myszy
    //

    private void MouseUpdate() {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("board"))) {
            mouseOver.x = (hit.point.x - boardOffset.x - pieceOffset.x - 5 + 0.55f) * 0.89286f;
            mouseOver.y = (hit.point.z - boardOffset.z - pieceOffset.z - 5 + 0.55f) * 0.89286f;
        }
        else {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void SelectPiece(int x, int y) {
        if (CheckIfInside(x,y) == false)
            return;
        if (b.pArr[x, y] != null && b.pArr[x, y].occupied == true) {
            SelectedPiece = b.pArr[x, y];
            startDrag = mouseOver;
            spX = x;
            spY = y;
        }
    }

    //
    // Logika Gry
    //

    private void Move(int x, int y) { // Wykonanie Ruchu
        MadeAMove = false;
        flagKill = false; // Czy jakiœ pionek zosta³ zbity
        if (!CheckIfInside(x, y))
            return;
        for (int i = 0; i < PossibleMove.Count; i++) {
            if (PossibleMove[i][0] == x && PossibleMove[i][1] == y) {
                for (int j = 0; j < PossibleKill.Count; j++) {
                    if (Between(spX, x, PossibleKill[j][0]) && Between(spY, y, PossibleKill[j][1])) {
                        if (b.pArr[PossibleKill[j][0], PossibleKill[j][1]].color == true)
                            NumOfWhite--;
                        else
                            NumOfBlack--;
                        b.Death(PossibleKill[j][0], PossibleKill[j][1]);
                        kX = PossibleKill[j][0];
                        kY = PossibleKill[j][1];
                        flagKill = true;
                        break;
                    }
                }
                bX = spX;
                bY = spY;
                b.MovePiece(spX, spY, x, y);
                if (flagKill == true) {
                    Killer = b.pArr[x, y];
                    killerX = x;
                    killerY = y;
                    SelectedPiece = b.pArr[x, y];
                    spX = x;
                    spY = y;
                }
                else
                    SelectedPiece = null;
                if (y == 0 && b.pArr[x, y].color == false)
                    b.Queen(x, y);
                if (y == 9 && b.pArr[x,y].color == true)
                    b.Queen(x, y);
                flagMove = true;
                MadeAMove = true;

                spX = -100;
                spY = -100;
                break;
            }
        }
        PossibleMove.Clear(); // Czysci tablice mo¿liwych zbiæ i mo¿liwych ruchów
        PossibleKill.Clear();
    }

    private void PassTurn(int x, int y) { // Sprawdzenie czy tura sie zakonczyla
        turnEnd = false;
        if (flagMove == true && (Killer == null || (Killer.status == false && CanYouKill(killerX, killerY) == false) || (Killer.status == true && CanYouKillQ(killerX, killerY) == false))) { // Czy by³ ruch i albo nie by³o zbicia, albo nie mo¿e byæ ju¿ kolejnego
            if (turn == true)
                turn = false;
            else
                turn = true;
            flagMove = false;
            turnEnd = true;
            Killer = null;
            killerX = -1;
            killerY = -1;
        }
    }

    private void Analyze(int x, int y) { // Analiza ruchu dla wybranego pionka
        if (SelectedPiece.occupied == true && SelectedPiece.color == turn) { // Czy jakiœ pionek zosta³ wybrany i czy turn jest jego
            if (SelectedPiece.status == false) {
                if (CanYouKill(x, y) == true) {// Czy wybrany moze zabic
                    Kill(x, y);
                }
                else if (CanAnyoneKill() == false) { // Je¿eli którykolwiek z pionków moze zbiæ nie mo¿na wykonaæ zwyk³ego ruchu
                    if (CheckIfInside(x - 1, y - 1) && b.pArr[x - 1, y - 1] && b.pArr[x - 1, y - 1].occupied == false && b.pArr[x, y].color == false) // sprawdza czy pole znajduje siê na planszy i jest puste
                        PossibleMove.Add(new List<int> { x - 1, y - 1 }); // Dodaje ruch do mozliwych ruchow
                    if (CheckIfInside(x + 1, y - 1) && b.pArr[x + 1, y - 1] && b.pArr[x + 1, y - 1].occupied == false && b.pArr[x, y].color == false)
                        PossibleMove.Add(new List<int> { x + 1, y - 1 });
                    if (CheckIfInside(x - 1, y + 1) && b.pArr[x - 1, y + 1] && b.pArr[x - 1, y + 1].occupied == false && b.pArr[x, y].color == true)
                        PossibleMove.Add(new List<int> { x - 1, y + 1 });
                    if (CheckIfInside(x + 1, y + 1) && b.pArr[x + 1, y + 1] && b.pArr[x + 1, y + 1].occupied == false && b.pArr[x, y].color == true)
                        PossibleMove.Add(new List<int> { x + 1, y + 1 });
                }
            }
            else {
                if (CanYouKillQ(x,y) == true) {
                    KillQ(x, y);
                }
                else if (CanAnyoneKill() == false) {
                    CheckIfEmptyQ(x - 1, y - 1);
                    CheckIfEmptyQ(x + 1, y - 1);
                    CheckIfEmptyQ(x - 1, y + 1);
                    CheckIfEmptyQ(x + 1, y + 1);
                }
            }
        }
    }

    //private bool KillEverythingBetween (int bX, int bY, int eX, int eY) {
    //    int dirX = eX - bX;
    //    int dirY = eY - bY;
    //    x2 = bX + dirX;  
    //}

    public static bool Between(int x1, int x2, int a) { // Sprawdza czy a jest pomiedzy x1 i x2
        if ((x1 > a && x2 < a) || (x1 < a) && (x2 > a))
            return true;
        else
            return false;
    }

    public static bool CheckIfInside(int x, int y) { // Sprawdza czy pole jest na planszy
        if (x < 0 || x >= 10 || y < 0 || y >= 10)
            return false;
        return true;
    }

    private bool CheckIfEmpty(int x, int y) { // Sprawdza czy pole nie jest zajmowane
        if (CheckIfInside(x, y) == false)
            return false;
        else {
            if (b.pArr[x, y].occupied == true)
                return false;
            return true;
        }
    }

    private void CheckIfEmptyQ(int x, int y) { // Sprawdza czy pole za bijanym pionkiem s¹ puste i dodaje je do tablicy mo¿liwych ruchów
        int dirX = x - spX, dirY = y - spY;
        while (CheckIfEmpty(x, y) == true) {
            PossibleMove.Add(new List<int> { x, y });
            x += dirX;
            y += dirY;
        }
    }

    private bool CanYouKill(int x, int y) { // Czy dany pionek moze zbiæ
        if (CheckIfInside(x - 1, y - 1) && !CheckIfEmpty(x - 1, y - 1) && CheckIfEmpty(x - 2, y - 2) && b.pArr[x - 1, y - 1].color != turn)
            return true;
        if (CheckIfInside(x + 1, y - 1) && !CheckIfEmpty(x + 1, y - 1) && CheckIfEmpty(x + 2, y - 2) && b.pArr[x + 1, y - 1].color != turn)
            return true;
        if (CheckIfInside(x - 1, y + 1) && !CheckIfEmpty(x - 1, y + 1) && CheckIfEmpty(x - 2, y + 2) && b.pArr[x - 1, y + 1].color != turn)
            return true;
        if (CheckIfInside(x + 1, y + 1) && !CheckIfEmpty(x + 1, y + 1) && CheckIfEmpty(x + 2, y + 2) && b.pArr[x + 1, y + 1].color != turn)
            return true;
        return false;
    }

    private bool CanYouKillQ(int x, int y) { // Czy dana królówka mo¿e zbiæ
        int dirX, dirY; // Kierunek w którym znajduje siê pole od królówki
        int x2 = x, y2 = y; // Pozycja sprawdzanych pól
        if (CheckIfInside(x, y) == false)
            return false;
        dirX = -1;
        while (dirX <= 1) { // Sprawdza przekontne od (x,y). Najpierw Lewo dó³ (dirX = -1, dirY = -1) potem Lewo góra (dirX = -1, dirY = 1) itd..
            dirY = -1;
            while (dirY <= 1) {
                x2 = x + dirX;
                y2 = y + dirY;
                while (CheckIfInside(x2, y2) == true) {
                    if (b.pArr[x2, y2].color != b.pArr[x, y].color && CheckIfEmpty(x2, y2) == false && CheckIfEmpty(x2 + dirX, y2 + dirY)) // Sprawdza czy pole jest zajête i czy pole za jest puste
                        return true;
                    else if (CheckIfEmpty(x2, y2) == false)
                        break;
                    x2 += dirX;
                    y2 += dirY;
                }
                dirY += 2;
            }
            dirX += 2;
        }
        return false;
    }

    private bool CanAnyoneKill() { // Czy jakikolwiek pionek jest w stanie zbiæ
        for (int i = 0; i < 10; i++) {
            if (i % 2 == 0) {
                for (int j = 1; j < 10; j += 2)
                    if (!CheckIfEmpty(j, i) && b.pArr[j, i].color == turn && ((b.pArr[j, i].status == false && CanYouKill(j, i) == true) || (b.pArr[j, i].status == true && CanYouKillQ(j, i) == true)))
                        return true;
            }
            else {
                for (int j = 0; j < 10; j += 2)
                    if (!CheckIfEmpty(j, i) && b.pArr[j, i].color == turn && ((b.pArr[j, i].status == false && CanYouKill(j, i) == true) || (b.pArr[j, i].status == true && CanYouKillQ(j, i) == true)))
                        return true;
            }
        }

        return false;
    }

    private void Kill(int x, int y) { // Dodanie ruchu do mozliwych zbic dla wybranego pionka
        if (!CheckIfEmpty(x - 1, y - 1) && CheckIfEmpty(x - 2, y - 2)) { // sprawdza czy pole obok jest zajete i czy pole za jest puste
            if (b.pArr[x - 1, y - 1].color != SelectedPiece.color) {
                PossibleMove.Add(new List<int> { x - 2, y - 2 }); // Dodaje pole do mozliwych ruchow
                PossibleKill.Add(new List<int> { x - 1, y - 1 }); // Dodaje pole zajmowane przez pionek przeciwnika do mozliwych zabojstw
            }
        }
        if (!CheckIfEmpty(x + 1, y - 1) && CheckIfEmpty(x + 2, y - 2)) {
            if (b.pArr[x + 1, y - 1].color != SelectedPiece.color) {
                PossibleMove.Add(new List<int> { x + 2, y - 2 });
                PossibleKill.Add(new List<int> { x + 1, y - 1 });
            }
        }
        if (!CheckIfEmpty(x - 1, y + 1) && CheckIfEmpty(x - 2, y + 2)) {
            if (b.pArr[x - 1, y + 1].color != SelectedPiece.color) {
                PossibleMove.Add(new List<int> { x - 2, y + 2 });
                PossibleKill.Add(new List<int> { x - 1, y + 1 });
            }
        }
        if (!CheckIfEmpty(x + 1, y + 1) && CheckIfEmpty(x + 2, y + 2)) {
            if (b.pArr[x + 1, y + 1].color != SelectedPiece.color) {
                PossibleMove.Add(new List<int> { x + 2, y + 2 });
                PossibleKill.Add(new List<int> { x + 1, y + 1 });
            }
        }
    }

    private void KillQ(int x, int y) { // Dodanie ruchu do mozliwych zbic dla wybranej królówki
        int dirX, dirY; // Kierunek w którym znajduje siê pole od królówki
        int x2 = x, y2 = y; // Pozycja sprawdzanych pól
        dirX = -1;
        while (dirX <= 1) { // Sprawdza przekontne od (x,y). Najpierw Lewo dó³ (dirX = -1, dirY = -1) potem Lewo góra (dirX = -1, dirY = 1) itd..
            dirY = -1;
            while (dirY <= 1) {
                x2 = x + dirX;
                y2 = y + dirY;
                while (CheckIfInside(x2, y2) == true) {
                    if (b.pArr[x2,y2].color != b.pArr[x, y].color && CheckIfEmpty(x2, y2) == false && CheckIfEmpty(x2 + dirX, y2 + dirY)) { // Sprawdza czy pole jest zajête i czy pole za jest puste
                        PossibleKill.Add(new List<int> { x2, y2 });
                        int x3 = x2 + dirX, y3 = y2 + dirY; // Pola za pionkiem dodane do tablicy mo¿liwych ruchów
                        while (x3 >= 0 && y3 >= 0 && x3 < 10 && y3 < 10) {
                            if (CheckIfEmpty(x3, y3) == true)
                                PossibleMove.Add(new List<int> { x3, y3 });
                            else
                                break;
                            x3 += dirX;
                            y3 += dirY;
                        }
                        break;
                    }
                    else if (CheckIfEmpty(x2, y2) == false)
                        break;
                    x2 += dirX;
                    y2 += dirY;
                }
                dirY += 2;
            }
            dirX += 2;
        }
    }
};