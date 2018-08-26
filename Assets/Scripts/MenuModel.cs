using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MenuModel {

    private static MenuModel instance = null;
    private static readonly Object lockingObj = new Object();
    private string username = "sh";
    private string passowrd = "sh";

    private MenuModel() {}	

    public static MenuModel GetInstance {
        get {
            if(instance == null) {
                lock (lockingObj) {
                    instance = (instance == null) ? new MenuModel() : instance;
                }
            }
            return instance;
        }
    }
    public bool VerifyUsernameAndPassword(string un, string pass) {
        return username.Equals(un) && passowrd.Equals(pass);
    }
}
