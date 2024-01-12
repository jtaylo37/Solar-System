using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleSystem : MonoBehaviour
{
    public int particleCount = 500;
    public int maxRadius = 20;
    public float gConst = 10;
    public GameObject nucleus;  // GameObject: Base class for all entities in Unity Scenes.
    public GameObject[] particles;
    public Material[] mats;
    void Awake()
    {
        particles = new GameObject[particleCount];
        nucleus = new GameObject();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Adding a Rigidbody component to an object will put its motion under the control of Unity's physics engine
        // https://docs.unity3d.com/ScriptReference/Rigidbody.html
        Rigidbody nucleusRb = nucleus.AddComponent<Rigidbody>();
        nucleusRb.mass = 10; // Set initial values for rigidbody
        nucleusRb.useGravity = false;

        // Init each particles condition. What do we need? Position, mass, velocity, color
        for (int i = 0; i < particleCount; i++)
        {
            particles[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Rigidbody particleRb = particles[i].AddComponent<Rigidbody>();
            particles[i].transform.position = new Vector3(Random.Range(-maxRadius, maxRadius),
                                                    Random.Range(-10, 10),
                                                    Random.Range(-maxRadius, maxRadius));
            //          Set rigidbody values
            //           - particleRb.mass 
            //           - particleRb.velocity
            //           - particles[i].transform.localScale
            particleRb.useGravity = false;

            Color customColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            particles[i].GetComponent<Renderer>().material.SetColor("_Color", customColor);
        }

    }
    // Update is called once per frame
    void Update()
    {   
        foreach (GameObject s in particles) // compute physics for each particle, every frame
        {
            // - position : nucleus.transform.position, s.transform.position
            // - mass : s.transform.GetComponent<Rigidbody>().mass
            // - acceleration : s.transform.GetComponent<Rigidbody>().AddForce(gravVector, ForceMode.Acceleration);
        }
    }
}
