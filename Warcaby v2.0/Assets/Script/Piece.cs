using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
	public bool color; // Kolor pionka: false - czarny, true - bia�y
	public bool status = false; // Czy pionek jest kr�l�wk�: false - zwyk�y, true - kr�l�wka
	public bool occupied = false; // Czy pole jest zajmowane: false - puste, true - zajete
}
