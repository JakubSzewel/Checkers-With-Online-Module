using UnityEngine;

public class VideoPlayer : MonoBehaviour {
    void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
