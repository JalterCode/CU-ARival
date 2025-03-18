using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonADT : Button
{
    private Boolean favorite;
    private List<string> classes;
    // Start is called before the first frame update
    public ButtonADT() 
    {   
        favorite = false;
        classes = new List<string>();
    }
    public bool Favorite
    {
        get { return favorite; }
        set { favorite = value; }
    }
    public void addClass(String name){
        classes.Add(name);
    }
    public bool findClass(String name){
        foreach (string className in classes)
        {
            if (className == name)
            {
                return true;
            }
        }
        return false;
    }
 
}
