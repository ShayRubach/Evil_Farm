using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

public class SC_AudioManager : MonoBehaviour {

    public List<AudioSource> sounds;

    private const int DEFAULT_VOLUME = 50;


    void Awake () {
        for (int i = 0; i < transform.childCount ; i++) {
            sounds.Add(transform.GetChild(i).GetComponent<AudioSource>());
        }
	}
	
    public void Play(string name) {
        sounds.Find(sound => sound.name == name).Play();
    }

    public void Stop(string name) {
        sounds.Find(sound => sound.name == name).Stop();
    }


    
}

//public class SC_AudioManager : MonoBehaviour {

//    [SerializeField]
//    private Sound[] soundPool;

//    public

//    void Awake() {
//        foreach (Sound s in soundPool) {
//            s.source = gameObject.AddComponent<AudioSource>();
//            s.source.clip = s.clip;
//            s.name = s.clip.name;
//            s.source.volume = s.volume;
//            s.source.pitch = s.pitch;
//        }
//    }

//    public void Play(string name) {
//        Sound s = Array.Find(soundPool, sound => sound.name == name);
//        s.source.Play();
//    }

//}

//[System.Serializable]
//public class Sound {
//    [HideInInspector]
//    public AudioSource source;
//    public AudioClip clip;
//    public string name;

//    [Range(0f, 1f)]
//    public float volume;
//    [Range(.1f, 3f)]
//    public float pitch;
//}