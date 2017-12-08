using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {

    public bool active;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        gameObject.SetActive(active);
    }
}
