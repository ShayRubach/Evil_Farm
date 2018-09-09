using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * a simple utility class to transfer data between scenes
 */
public static class SharedDataHandler {
    public static string nextScreenRequested {get;set;}
    public static int wins {get;set;}
    public static int loses {get;set;}

    static SharedDataHandler() {
        nextScreenRequested = MenuModel.INITIAL_SCENE;
    }
}
