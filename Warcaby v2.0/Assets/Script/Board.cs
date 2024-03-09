using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Piece[,] pArr = new Piece[10, 10];

	public GameObject whitePiecePrefab;
	public GameObject blackPiecePrefab;
	public GameObject emptyPrefab;
	public GameObject selectedPiecePrefab;
	public GameObject possibleMovePrefab;
	public GameObject whiteQueenPrefab;
	public GameObject blackQueenPrefab;

	private GameObject Prefab_SP;
	private List<GameObject> Prefab_PM = new List<GameObject>();

	private void Start() {
		GenerateBoard();
	}
	
	private void GenerateBoard() {
		//int x = 40; // jest 40 pionków na start
		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
				GeneratePiece(j, i);
			}
		}
    }

	private void GeneratePiece(int x, int y) {
		GameObject go;
		if (y < 4 && (x + y) % 2 != 0)
			go = Instantiate(whitePiecePrefab);
		else if (y >= 6 && (x + y) % 2 != 0)
			go = Instantiate(blackPiecePrefab);
		else
			go = Instantiate(emptyPrefab);
		go.transform.SetParent(transform);
		Piece p = go.GetComponent<Piece>();
		pArr[x, y] = p;
		p.transform.position = (Vector3.right * x * 1.115f) + (Vector3.forward * y * 1.1225f);
	}

	public void MovePiece(int x1, int y1, int x2, int y2) { // Ruszenie pionka (dzia³a na zasadzie zamiany miejscami z niewidocznym "pionkiem")
		Piece p1 = pArr[x1, y1];
		Piece p2 = pArr[x2, y2];

		Piece temp = pArr[x1, y1]; // Zamiana miejsca w tablicy pArr
		pArr[x1, y1] = pArr[x2, y2];
		pArr[x2, y2] = temp;
		

		Vector3 tempPos = p1.transform.position;
		p1.transform.position = p2.transform.position;
		p2.transform.position = tempPos;
	}

	public void Death(int x, int y) { // Usuniecie pionka
		Vector3 tempPos = pArr[x, y].transform.position; // Zapisanie pozycji pionka
		pArr[x, y].transform.position = (Vector3.right * 1000) + (Vector3.forward * 1000); // i myk nie ma pionka
		GameObject go = Instantiate(emptyPrefab); // Zastapienie niewidocznym pionkiem
		go.transform.SetParent(transform);
		Piece p = go.GetComponent<Piece>();
		pArr[x, y] = p;
		p.transform.position = tempPos;

	}

	public void DrawPossibleMove(int x, int y, List<List<int>> PM) { // Pokazanie mo¿liwych ruchów na planszy
		Prefab_SP = Instantiate(selectedPiecePrefab);
		Prefab_SP.transform.position = (Vector3.right * x * 1.115f) + (Vector3.forward * y * 1.1225f) + (Vector3.down * 0.2f);
		for (int i = 0; i < PM.Count; i++) {
			GameObject go = Instantiate(possibleMovePrefab);
			Prefab_PM.Add(go);
			Prefab_PM[i].transform.position = (Vector3.right * PM[i][0] * 1.115f) + (Vector3.forward * PM[i][1] * 1.1225f);
		}
    }

	public void DestroyPossibleMove(List<List<int>> PM) { // Pozbycie siê nieaktualnych znaczników mo¿liwych ruchów
		Destroy(Prefab_SP);
		for (int i = 0; i < PM.Count; i++) {
			Destroy(Prefab_PM[i]);
		}
		Prefab_PM.Clear();
	}

	public void Queen(int x, int y) { // Zmiana pionka na królówkê
		if (pArr[x, y].status == false) { // Je¿eli jeszcze nie jest królówk¹
			Vector3 tempPos = pArr[x, y].transform.position; // Zapisanie pozycji pionka
			pArr[x, y].transform.position = (Vector3.right * 1000) + (Vector3.forward * 1000); // i myk nie ma pionka
			if (pArr[x, y].color == true) { // Zamiana na bia³¹ królówkê
				GameObject go = Instantiate(whiteQueenPrefab);
				go.transform.SetParent(transform);
				Piece p = go.GetComponent<Piece>();
				pArr[x, y] = p;
				p.transform.position = tempPos;
			}
			else { // Zamiana na czarn¹ królówkê
				GameObject go = Instantiate(blackQueenPrefab);
				go.transform.SetParent(transform);
				Piece p = go.GetComponent<Piece>();
				pArr[x, y] = p;
				p.transform.position = tempPos;
			}
		}
	}
}