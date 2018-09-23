using System;
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
    public static bool isMultiplayer = false;

    static SharedDataHandler() {
        nextScreenRequested = MenuModel.INITIAL_SCENE;
    }

    internal static void SetMultiplayerMode(bool mode) {
        isMultiplayer = mode;
    }
}
