using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbitss : MonoBehaviour
{
    public int sphereCount = 500;
    public float maxRadius = 200;
    public GameObject[] spheres;
    public Material[] mats;
    public Material trailMat;

    public int holderCount = 5;
    const int divider = 2;
    private List<GameObject>[] vHolder;
    private AudioSynthFM[] audioArr;

    private void Awake()
    {
        spheres = new GameObject[sphereCount];
    }

    private void Start()
    {
        // Initialize bins
        vHolder = new List<GameObject>[holderCount];
        for (int i = 0; i < holderCount; i++)
        {
            vHolder[i] = new List<GameObject>();
        }

        audioArr = new AudioSynthFM[holderCount];
        CreateSpheres(sphereCount, maxRadius);
    }

    public void CreateSpheres(int count, float radius)
    {
        var sphereToCopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody rb = sphereToCopy.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        
        for (int i = 0; i < count; i++)
        {
            var sp = Instantiate(sphereToCopy);
            sp.AddComponent<SphereCollider>();
            float distance = Random.Range(10, radius);
            float angle = Random.Range(0f, 360f);
            float height = Random.Range(-10, 10);
            sp.transform.position = transform.position + new Vector3(distance * Mathf.Cos(angle), height, distance * Mathf.Sin(angle));
            sp.transform.localScale *= Random.Range(0.5f, 1);
            sp.GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
            TrailRenderer tr = sp.AddComponent<TrailRenderer>();
            tr.time = 1.0f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0;
            tr.material = trailMat;
            tr.startColor = new Color(1, 1, 0, 0.1f);
            tr.endColor = new Color(0, 0, 0, 0);
            SphereCollider sc = sp.AddComponent<SphereCollider>();
            sc.isTrigger = false;
            
            Rigidbody sphereRb = sp.GetComponent<Rigidbody>();
            Vector3 orbitalPlaneNormal = Vector3.Cross(sp.transform.position - transform.position, Vector3.up).normalized;
            Vector3 velocityDirection = Vector3.Cross(orbitalPlaneNormal, (sp.transform.position - transform.position).normalized);
            float velocityMagnitude = Mathf.Sqrt(6.7f * transform.localScale.x * 300 / distance);
            sphereRb.velocity = velocityDirection * velocityMagnitude;
            sp.AddComponent<ParticleCollision>();
            sp.tag = "Particle";
            
            spheres[i] = sp;
        }

        Destroy(sphereToCopy);
    }

    private void Update()
    {
        // Clear the bins
        foreach (var bin in vHolder)
        {
            bin.Clear();
        }

        // Place each particle in a bin based on its velocity
        foreach (GameObject s in spheres)
        {
            Vector3 difference = transform.position - s.transform.position;
            float dist = difference.magnitude;
            Vector3 gravityDirection = difference.normalized;
            float gravity = 6.7f * (transform.localScale.x * s.transform.localScale.x * 300) / (dist * dist);
            Vector3 gravityVector = gravityDirection * gravity;
            s.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);

            float velocity = s.GetComponent<Rigidbody>().velocity.magnitude;
            int idx = (int)velocity / divider;
            idx = Mathf.Clamp(idx, 0, holderCount - 1);
            vHolder[idx].Add(s);
        }

        // Adjust audio frequency for each bin
        for (int i = 0; i < holderCount; i++)
        {
            if (vHolder[i].Count > 0)
            {
                if (audioArr[i] == null)
                {
                    GameObject audioObj = new GameObject("Audio " + i);
                    audioArr[i] = audioObj.AddComponent<AudioSynthFM>();
                }

                audioArr[i].SetFrequencyV(i * divider);
                audioArr[i].SetAmplitudeV(i);
                //Debug.Log("Setting frequency for bin " + i + " with velocity " + (i * 5));
            }
            else if (audioArr[i] != null)
            {
                Destroy(audioArr[i].gameObject);
                audioArr[i] = null;
            }
        }
    }
}
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// public class Orbitss : MonoBehaviour
// {

//     public AudioClip particleSound;
//     public int sphereCount = 500;
//     public float maxRadius = 200;
//     public float frequency;
//     public GameObject[] spheres;
//     public Material[] mats;
//     public Material trailMat;
//     [Range(20, 4000)]
//     public float baseFrequency = 440; 
//     public float sampleRate = 44100;
//     [Range(0.1f, 2)]
//     public float amplitude = 0.7f;
//     public int maxAudioSources = 50;  // Maximum number of simultaneous AudioSources for pooling

//     private List<AudioSource> audioSourcePool = new List<AudioSource>();
//     private Dictionary<GameObject, AudioSource> particleToAudioSourceMap = new Dictionary<GameObject, AudioSource>();

    
//     private void Awake()
//     {
//         spheres = new GameObject[sphereCount];

//         for (int i = 0; i < maxAudioSources; i++)
//         {
//             AudioSource audioSource = gameObject.AddComponent<AudioSource>();
//             //audioSource.clip = particleSound;
//             audioSource.playOnAwake = false;
//             audioSourcePool.Add(audioSource);
//         }
//     }
    
//     private void Start()
//     {
//         CreateSpheres(sphereCount, maxRadius);
//     }

//     public void CreateSpheres(int count, float radius)
//     {
//         var sphereToCopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//         Rigidbody rb = sphereToCopy.AddComponent<Rigidbody>();
//         rb.useGravity = false;
        
//         for (int i = 0; i < count; i++)
//         {
//             var sp = Instantiate(sphereToCopy);
//             float distance = Random.Range(10, radius);
//             float angle = Random.Range(0f, 360f);
//             float height = Random.Range(-10, 10);
//             sp.transform.position = transform.position + new Vector3(distance * Mathf.Cos(angle), height, distance * Mathf.Sin(angle));
//             sp.transform.localScale *= Random.Range(0.5f, 1);
//             sp.GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
//             TrailRenderer tr = sp.AddComponent<TrailRenderer>();
//             tr.time = 1.0f;
//             tr.startWidth = 0.1f;
//             tr.endWidth = 0;
//             tr.material = trailMat;
//             tr.startColor = new Color(1, 1, 0, 0.1f);
//             tr.endColor = new Color(0, 0, 0, 0);
            
//             Rigidbody sphereRb = sp.GetComponent<Rigidbody>();
//             Vector3 orbitalPlaneNormal = Vector3.Cross(sp.transform.position - transform.position, Vector3.up).normalized;
//             Vector3 velocityDirection = Vector3.Cross(orbitalPlaneNormal, (sp.transform.position - transform.position).normalized);
//             float velocityMagnitude = Mathf.Sqrt(6.7f * transform.localScale.x * 300 / distance);
//             sphereRb.velocity = velocityDirection * velocityMagnitude;
            
//             spheres[i] = sp;
//         }

//         Destroy(sphereToCopy);
//     }

//     // Update is called once per frame
//     private void Update()
//     {
//         foreach (GameObject s in spheres)
//         {
//             Vector3 difference = transform.position - s.transform.position;
//             float dist = difference.magnitude;
//             Vector3 gravityDirection = difference.normalized;
//             float gravity = 6.7f * (transform.localScale.x * s.transform.localScale.x * 300) / (dist * dist);
//             Vector3 gravityVector = gravityDirection * gravity;
//             s.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);

//             float velocity = s.GetComponent<Rigidbody>().velocity.magnitude;
//             float normalizedVelocity = Mathf.Clamp01(velocity / maxRadius);

//             // Map velocity to FM parameters
//             frequency = baseFrequency + normalizedVelocity * 1000;

//             // Handle AudioSource for the particle
//             if (!particleToAudioSourceMap.ContainsKey(s))
//             {
//                 if (audioSourcePool.Count > 0)
//                 {
//                     AudioSource audioSource = audioSourcePool[0];
//                     audioSourcePool.RemoveAt(0);
//                     particleToAudioSourceMap[s] = audioSource;
//                 }
//                 else
//                 {
//                     // No available AudioSources in the pool
//                     continue;
//                 }
//             }

//             AudioSource sAudioSource = particleToAudioSourceMap[s];
//             sAudioSource.volume = 1.0f;

//             sAudioSource.pitch =  Mathf.Clamp(frequency / 440f, 0.5f, 2f);//frequency / 440f;

//             // Start playing sound if not already playing
//             if (!sAudioSource.isPlaying)
//             {
//                 sAudioSource.Play();
//                 //Debug.Log("AudioSource started playing for particle: " + s.name);
//             }

//             // Check envelope to stop and return AudioSource to pool
//             if (Envelope(sAudioSource.timeSamples) < 0.001)
//             {
//                 sAudioSource.Stop();
//                 audioSourcePool.Add(sAudioSource);
//                 particleToAudioSourceMap.Remove(s);
//             }
//         }
//     }

//      public float Envelope(int timeIdx)

//     {   // should have something looks like..: /\__

//         // https://www.sciencedirect.com/topics/engineering/envelope-function

//         float a = 0.13f;

//         float b = 0.45f;

//         float tempo = 1000f;// timeIdx is an integer increasing rapidly so calm down

//         return Mathf.Abs(Mathf.Exp(-a * (timeIdx)/tempo) - Mathf.Exp(-b * (timeIdx) / tempo));
//     }
// }
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Orbitss : MonoBehaviour
{
    public int sphereCount = 500;
    public float maxRadius = 200;
    public GameObject[] spheres;
    public Material[] mats;
    public Material trailMat;
    
    private void Awake()
    {
        spheres = new GameObject[sphereCount];
    }
    
    private void Start()
    {
        CreateSpheres(sphereCount, maxRadius);
    }

    public void CreateSpheres(int count, float radius)
    {
        var sphereToCopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody rb = sphereToCopy.AddComponent<Rigidbody>();
        rb.useGravity = false;
        
        for (int i = 0; i < count; i++)
        {
            var sp = Instantiate(sphereToCopy);
            float distance = Random.Range(10, radius);
            float angle = Random.Range(0f, 360f);
            float height = Random.Range(-10, 10);
            sp.transform.position = transform.position + new Vector3(distance * Mathf.Cos(angle), height, distance * Mathf.Sin(angle));
            sp.transform.localScale *= Random.Range(0.5f, 1);
            sp.GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
            TrailRenderer tr = sp.AddComponent<TrailRenderer>();
            tr.time = 1.0f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0;
            tr.material = trailMat;
            tr.startColor = new Color(1, 1, 0, 0.1f);
            tr.endColor = new Color(0, 0, 0, 0);
            
            Rigidbody sphereRb = sp.GetComponent<Rigidbody>();
            Vector3 orbitalPlaneNormal = Vector3.Cross(sp.transform.position - transform.position, Vector3.up).normalized;
            Vector3 velocityDirection = Vector3.Cross(orbitalPlaneNormal, (sp.transform.position - transform.position).normalized);
            float velocityMagnitude = Mathf.Sqrt(6.7f * transform.localScale.x * 300 / distance);
            sphereRb.velocity = velocityDirection * velocityMagnitude;
            
            spheres[i] = sp;
        }

        Destroy(sphereToCopy);
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (GameObject s in spheres)
        {
            Vector3 difference = transform.position - s.transform.position;
            float dist = difference.magnitude;
            Vector3 gravityDirection = difference.normalized;
            float gravity = 6.7f * (transform.localScale.x * s.transform.localScale.x * 300) / (dist * dist);
            Vector3 gravityVector = gravityDirection * gravity;
            s.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
        }
    }
}*/
/*public class Orbitss : MonoBehaviour
{
    public int sphereCount = 500;
    public float maxRadius = 200;
    public GameObject[] spheres;
    public Material[] mats;
    public Material trailMat;
    
    private void Awake()
    {
        spheres = new GameObject[sphereCount];
    }
    
    private void Start()
    {
        CreateSpheres(sphereCount, maxRadius);
    }

    public void CreateSpheres(int count, float radius)
    {
        var sphereToCopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody rb = sphereToCopy.AddComponent<Rigidbody>();
        rb.useGravity = false;
        
        for (int i = 0; i < count; i++)
        {
            var sp = Instantiate(sphereToCopy);
            float distance = Random.Range(10, radius);
            float angle = Random.Range(0f, 360f);
            float height = Random.Range(-5, 5);
            sp.transform.position = transform.position + new Vector3(distance * Mathf.Cos(angle), height, distance * Mathf.Sin(angle));
            sp.transform.localScale *= Random.Range(0.5f, 1);
            sp.GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
            TrailRenderer tr = sp.AddComponent<TrailRenderer>();
            tr.time = 1.0f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0;
            tr.material = trailMat;
            tr.startColor = new Color(1, 1, 0, 0.1f);
            tr.endColor = new Color(0, 0, 0, 0);
            
            Rigidbody sphereRb = sp.GetComponent<Rigidbody>();
            Vector3 orbitalPlaneNormal = Vector3.Cross(sp.transform.position - transform.position, Vector3.up).normalized;//calculate radius from sun then finds the vector perpendicular to y axis(clock example: line perpendicular to center of clock)
            Vector3 velocityDirection = Vector3.Cross(orbitalPlaneNormal, (sp.transform.position - transform.position).normalized);//calculates vector tangent to orbital circle(clock example: line tangent to clocks circumference pointing in direction of spheres oribit clockwise or counter)
            float velocityMagnitude = Mathf.Sqrt(6.7f * transform.localScale.x * 300 / distance);//formula for v
            sphereRb.velocity = velocityDirection * velocityMagnitude;//set velocity
            
            spheres[i] = sp;
        }

        Destroy(sphereToCopy);
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (GameObject s in spheres)
        {
            Vector3 difference = transform.position - s.transform.position;
            float dist = difference.magnitude;
            Vector3 gravityDirection = difference.normalized;
            float gravity = 6.7f * (s.transform.localScale.x * 300) / (dist * dist);
            Vector3 gravityVector = gravityDirection * gravity;
            s.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
        }
    }
}
*/

/*public class Orbitss : MonoBehaviour
{
    public int sphereCount = 500;
    public int maxRadius = 200;
    public GameObject[] spheres;
    public Material[] mats;
    public Material trailMat;

    void Awake() 
    {
        spheres = new GameObject[sphereCount];
    }

    void Start()
    {
        spheres = CreateSpheres(sphereCount, maxRadius);
    }

    public GameObject[] CreateSpheres(int count, int radius)
    {
        var sphs = new GameObject[count];
        var sphereToCopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rigidbody rb = sphereToCopy.AddComponent<Rigidbody>();
        rb.useGravity = false;

        for(int i = 0; i < count; i++) {
            var sp = GameObject.Instantiate(sphereToCopy);
            sp.transform.position = this.transform.position + new Vector3(Random.Range(-maxRadius, maxRadius), Random.Range(-10, 10), Random.Range(-maxRadius, maxRadius));
            sp.transform.localScale *= Random.Range(0.5f, 1);
            sp.GetComponent<Renderer>().material = mats[Random.Range(0, mats.Length)];
            TrailRenderer tr = sp.AddComponent<TrailRenderer>();
            tr.time = 1.0f;
            tr.startWidth = 0.1f;
            tr.endWidth = 0;
            tr.material = trailMat;
            tr.startColor = new Color(1, 1, 0, 0.1f);
            tr.endColor = new Color(0, 0, 0, 0);
            spheres[i] = sp;
        }

        GameObject.Destroy(sphereToCopy);

        return spheres;
    }
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject s in spheres)
        {
            Vector3 difference = this.transform.position - s.transform.position; //radius position of sun minus the sphere position

            float dist = difference.magnitude;//exact radius
            Vector3 gravityDirection = difference.normalized;//direction we are traveling
            float gravity = 6.7f * (this.transform.localScale.x * s.transform.localScale.x * 200) / (dist * dist); //80 to scale the force of gravity up, play with this number

            Vector3 gravityVector = (gravityDirection * gravity);
            s.transform.GetComponent<Rigidbody>().AddForce(s.transform.forward, ForceMode.Acceleration);
            s.transform.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);//ignore Unity's physics system math calculation by not taking into account the mass of the rigidbody
        }   
    }
}*/
