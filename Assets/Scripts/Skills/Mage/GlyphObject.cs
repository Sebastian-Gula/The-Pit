using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlyphObject : MonoBehaviour
{
    private int life = 5;
    public static GlyphObject Instance;


    public void Awake()
    {
        Instance = this;
    }


    public void decreaseLife()
   {
        life--;

        if (life == 0)
            Destroy(gameObject);
    }
}
