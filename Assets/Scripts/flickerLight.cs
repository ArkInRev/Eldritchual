using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flickerLight : MonoBehaviour {

    //fluctuate between these numbers
    public float minAlpha;
    public float maxAlpha;


    private float coreAlpha;
    private float colorShiftAmount;
    private Color color;

    public float freqMultiplier; // speed up or slow down the frequency

    public WaveMethod flickerType;

    public SpriteRenderer sr;

    public enum WaveMethod{
        Smooth,
        Jagged
    }


	// Use this for initialization
	void Start () {
        coreAlpha = sr.color.a;
		
	}
	
	// Update is called once per frame
	void Update () {
        float flickerRange = maxAlpha - minAlpha;
        color = sr.color;

        switch (flickerType)
        {
            case (WaveMethod.Jagged):
                colorShiftAmount = Mathf.PingPong(Time.time, flickerRange) * freqMultiplier;

                break;
            case (WaveMethod.Smooth):
                colorShiftAmount = Mathf.Sin(Time.time * freqMultiplier) * (flickerRange);
                break;

        }
        color.a = colorShiftAmount;
        sr.color = color;



    }
}
