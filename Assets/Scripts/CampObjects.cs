using UnityEngine;
using System.Collections;

public class CampObjects : MonoBehaviour {

	void Awake ()
    {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;

        GameManager.Instance.Obsacles[x][y] = 'X';
        GameManager.Instance.Obsacles[x + 1][y] = 'X';
        GameManager.Instance.Obsacles[x][y + 1] = 'X';
        GameManager.Instance.Obsacles[x + 1][y + 1] = 'X';
    }
}
