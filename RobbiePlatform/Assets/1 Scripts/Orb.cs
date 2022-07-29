using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public GameObject exploosionVFXPrefab;

    int player;
    // Start is called before the first frame update
    void Start()
    {
        player = LayerMask.NameToLayer("Player");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == player)
        {
            Instantiate(exploosionVFXPrefab, transform.position, transform.rotation);
            gameObject.SetActive(false);

            AudioManmager.PlayOrbAudio();
        }
    }
}
