using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PearlController : MonoBehaviour {

    public Text pickupPearlText;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            pickupPearlText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            pickupPearlText.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && pickupPearlText.gameObject.activeSelf) {
            gameObject.SetActive(false);
            pickupPearlText.gameObject.SetActive(false);
        }
    }
}
