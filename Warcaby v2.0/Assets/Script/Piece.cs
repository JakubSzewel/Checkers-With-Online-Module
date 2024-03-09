using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
	public bool color; // Kolor pionka: false - czarny, true - bia³y
	public bool status = false; // Czy pionek jest królówk¹: false - zwyk³y, true - królówka
	public bool occupied = false; // Czy pole jest zajmowane: false - puste, true - zajete
}
