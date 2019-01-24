using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Sound : MonoBehaviour {

    private AudioSource sound01;

    [SerializeField]
    private VideoClip TitleVideo = null;
    // Use this for initialization
    void Start () {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        sound01 = audioSources[0];
        if (TitleVideo == null)
        {
            sound01.PlayOneShot(sound01.clip);
        }
        else
        {
            this.transform.GetComponent<VideoPlayer>().clip = TitleVideo;
            this.transform.GetComponent<VideoPlayer>().SetTargetAudioSource(0, this.transform.GetComponent<AudioSource>());
            this.transform.GetComponent<VideoPlayer>().Play();
            StartCoroutine("SoundCoroutine");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator SoundCoroutine()
    {
        yield return new WaitForSeconds(3.8f);
        sound01.PlayOneShot(sound01.clip);
    }
}
