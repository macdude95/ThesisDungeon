using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistence : MonoBehaviour {

	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
