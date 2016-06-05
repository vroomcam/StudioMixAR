﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MixerController : MonoBehaviour {

    SongManager songManager;
    List<SongManager.Song> allTracks;
    GameObject mixer;

    Object trackPrefab;
    Vector3 cameraPos;

	void Start () {
        cameraPos = Camera.main.gameObject.transform.position + new Vector3(0, 0, 1.25f);
        songManager = GetComponent<SongManager>();
        mixer = GameObject.Find("Mixer");

        allTracks = songManager.getAllSongs();
        initializeTracks(allTracks);
    }

	void Update () {
        
	}

    void initializeTracks (List<SongManager.Song> songs)
    {
        for (int i = 0; i < songs.Count; i++)
        {
            instantiateTracks(songs[i]);
        }

        //if (mixer.GetComponentInChildren<Transform>().childCount != 0)
        //{
        //    List<GameObject> children = new List<GameObject>();
        //    foreach (Transform child in mixer.transform)
        //    {
        //        children.Add(child.gameObject);
        //    }

        //    foreach (var child in children)
        //    {
        //        Destroy(child);
        //    }
        //}
       
    }

    void instantiateTracks(SongManager.Song song)
    {
        GameObject songFolder = new GameObject(song.name);
        songFolder.transform.parent = mixer.transform;

        for (int i = 0; i < song.tracks.Count; i++)
        {
            string prefabName = song.tracks[i].name + "_" + song.tracks[i].songName;
            // plots prefab tracks equally spaced around the user
            int angle = i * (360 / song.tracks.Count);
            Vector3 plotPos = Circle(cameraPos, 2.2f, angle);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, cameraPos - plotPos);

            // creates prefab based on instrument/track type or family
            string prefabType = song.tracks[i].type;
            trackPrefab = Resources.Load("Prefabs/" + prefabType);

            // instantiates a track prefab
            var track = Instantiate(trackPrefab, plotPos, rotation);
            track.name = prefabName;

            // sets this track prefab to child of Mixer/:songName
            GameObject currentTrack = GameObject.Find(prefabName);
            currentTrack.transform.parent = songFolder.transform;
            AudioClip currentClip;

            // dynamically sets current tracks audio clip
            currentClip = Resources.Load<AudioClip>("Audio/" + song.tracks[i].clipName);
            initTrackAudio(prefabName, currentClip, song.tracks[i].volume);

        }

        songFolder.SetActive(song.isActive);
    }

    private void initTrackAudio(string name, AudioClip audioClip, float volume)
    {
        //Debug.Log(GameObject.Find(name));
        AudioSource audioSource = GameObject.Find(name).transform.GetChild(0).GetComponent<AudioSource>();

        // initializes audioClip of instantiated prefab
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    //void getSong(string songTitle)
    //{
    //    Debug.Log(songTitle);
    //    if (songTitle == "Recall")
    //    {
    //        song = getRecall();
    //    }
    //    else if (songTitle == "Not Recall")
    //    {
    //        song = notRecall();
    //    }
    //    //return song;
    //    mixer.SendMessage("buildSong", song);
    //}

    void activateSong(string name)
    {
        Transform[] songFolders;
        Transform songFolder;

        int index = name.IndexOf("_Button");
        if (index != -1)
        {
            name = name.Remove(index);
        }

        foreach (Transform child in mixer.transform)
        {
            Debug.Log(child.gameObject.name);
            Debug.Log(name);
            if (child.gameObject.name == name)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
        //songFolder = mixer.transform.Find(name);
        //songFolder.gameObject.SetActive(false);

    }

    Vector3 Circle(Vector3 center, float radius, int ang)
    {
        float angle = ang;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }
}
