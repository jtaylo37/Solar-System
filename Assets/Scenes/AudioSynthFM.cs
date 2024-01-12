using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSynthFM : MonoBehaviour
{
    [Range(20, 4000)]
    public float frequency;

    [Range(0, 20)]
    public float carrierMultiplier;

    [Range(0, 20)]
    public float modularMultiplier;

    public float sampleRate = 44100;

    [Range(0.1f, 2)]
    public float amplitude;

    AudioSource audioSource;
    int timeIdx = 0;

    float maxRadius = 200.0f;
    const int scalar = 11;

    //public AudioClip collisionSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.Stop();
        frequency = 440;
        carrierMultiplier = 1.4f;
        modularMultiplier = 0.5f;
        amplitude = 0.7f;
    }

    public void SetFrequencyV(float velocity)
    {
        frequency = Mathf.Clamp(velocity * scalar, 20, 4000);
        //frequency = Mathf.Clamp(velocity * velocity * 5, 20, 4000);
        if (audioSource != null && !audioSource.isPlaying)
        {
        audioSource.Play();
        }
        //Debug.Log("Frequency set to: " + frequency);
    }

    public void SetAmplitudeV(float velocity)
    {
        amplitude = Mathf.Clamp(velocity / Random.Range(6250, 6500), 0.1f, 2.0f);
    }

    
    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Space)) // trigger of sound synth

        {

            if (audioSource != null && !audioSource.isPlaying)

            {

                timeIdx = 0;  //resets timer before playing sound

                audioSource.Play();

            }

        }

        // turn off the audio when the envelope is small enough.

        if (timeIdx > 1000 && Envelope(timeIdx) < 0.001)

        {

            audioSource.Stop();

            timeIdx = 0;

        }

        // Graphic Part: cube reacts to the audio

        // cube.transform.position = new Vector3(0, 0, Envelope(timeIdx) * 10);

        // cube.transform.localScale = new Vector3(carrierMultiplier, modularMultiplier + Envelope(timeIdx) * 10, carrierMultiplier + Envelope(timeIdx) * 10);

        // cube.transform.rotation = new Quaternion(carrierMultiplier * (1 + Envelope(timeIdx)), modularMultiplier * (1+ Envelope(timeIdx)), (1 + Envelope(timeIdx)), (1 + Envelope(timeIdx)));

        // Color customColor = new Color(frequency/1000, carrierMultiplier / 10 + Envelope(timeIdx) * 10, modularMultiplier / 10 + Envelope(timeIdx));

        // cubeRenderer.material.SetColor("_Color", customColor);

    }


    void OnAudioFilterRead(float[] data, int channels)

    {

        for (int i = 0; i < data.Length; i += channels)

        {

            data[i] = amplitude * Envelope(timeIdx) * FM(timeIdx, frequency, carrierMultiplier, modularMultiplier);

            data[i + 1] = data[i];

            timeIdx++;

        }

    }

    // Compute frequency in angular frequency

    public float ComputeFreq(float frequency)

    {   // why? http://hplgit.github.io/primer.html/doc/pub/diffeq/._diffeq-solarized002.html#:~:text=Mathematically%2C%20the%20oscillations%20are%20described,means%2044100%20samples%20per%20second.

        return 2 * Mathf.PI * frequency / sampleRate; // e.g. 2*pi*440/44100

    }

    // Frequency Modulation computation

    public float FM(int timeIdx, float frequency, float carMul, float modMul)

    {

        //return Mathf.Sin(ComputeFreq(frequency) * timeIdx); // Sine wave

        float adjuster = .02f;//Mathf.Clamp01(frequency / 4000.0f) * 10;
        return Mathf.Sin((ComputeFreq(frequency * carMul) * timeIdx + Mathf.Sin(ComputeFreq(frequency * modMul) * timeIdx) * adjuster));

        ///return Mathf.Sin((ComputeFreq(frequency * carMul) * timeIdx + Mathf.Sin(ComputeFreq(frequency * modMul) * timeIdx))); // fluctuating FM

        //return Mathf.Sin((ComputeFreq(frequency * carMul) * timeIdx + Mathf.Sin(ComputeFreq(frequency * modMul) * timeIdx) * Envelope(timeIdx/3))); // fluctuating FM

    }

    public float Envelope(int timeIdx)

    {   // should have something looks like..: /\__

        // https://www.sciencedirect.com/topics/engineering/envelope-function

        float a = 0.13f;

        float b = 0.45f;

        float tempo = 1000f;// timeIdx is an integer increasing rapidly so calm down

        return Mathf.Abs(Mathf.Exp(-a * (timeIdx)/tempo) - Mathf.Exp(-b * (timeIdx) / tempo));
    }
}