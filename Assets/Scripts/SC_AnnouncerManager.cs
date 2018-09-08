using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AnnouncerManager : MonoBehaviour {

    public delegate void AnnouceToController();
    public static event AnnouceToController FinishAnnouncementEvent;
    public static event AnnouceToController StartAnnouncementEvent;


    public void OnStartAnnouncement() {
        if (StartAnnouncementEvent != null)
            StartAnnouncementEvent();
    }

    public void OnFinishAnnouncement() {
        if (FinishAnnouncementEvent != null)
            FinishAnnouncementEvent();
    }
}
