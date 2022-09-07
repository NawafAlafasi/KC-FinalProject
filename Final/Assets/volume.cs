using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volume : MonoBehaviour
{
    public Slider vol;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("ftime") == 0)
        {
            PlayerPrefs.SetInt("ftime", 1);
            PlayerPrefs.SetFloat("Slider", 100);
        }
        vol.value = PlayerPrefs.GetFloat("Slider");
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("Slider", vol.value);
        GameObject.Find("AudioManager").GetComponent<AudioSource>().volume = vol.value / 100;
    }
}
