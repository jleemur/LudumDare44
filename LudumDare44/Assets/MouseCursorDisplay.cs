﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorDisplay : MonoBehaviour
{

    Vector2 mouse;
    int width = 64;
    int height = 64;
    public Texture2D cursor;
     
     void Start()
     {
         Cursor.visible = false;
     }
     
     void Update()
     {
         mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
     }
     
     void OnGUI()
     {
         GUI.DrawTexture(new Rect(mouse.x - (width / 2), mouse.y - (height / 2), width, height), cursor);
     }
}
