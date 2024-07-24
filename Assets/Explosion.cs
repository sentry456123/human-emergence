using UnityEngine;

[RequireComponent(typeof(Bullet))]

public class Explosion : MonoBehaviour
{
    [SerializeField] private Bullet fragment;
    [SerializeField] private int fragmentCount = 8;
    [SerializeField] private AudioClip sound;

    private bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy() 
    {
        if (!isQuitting)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                Bullet instance = Instantiate(fragment, transform.position, Random.rotation);
                instance.team = GetComponent<Bullet>().team;
                AudioSource.PlayClipAtPoint(sound, transform.position);
            }
        }
    }
}
