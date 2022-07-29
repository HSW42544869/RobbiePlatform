using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public GameObject deathVFXPrefabs;

    int trapsLayer;
    // Start is called before the first frame update
    void Start()
    {
        trapsLayer = LayerMask.NameToLayer("Traps");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == trapsLayer)
        {
            Instantiate(deathVFXPrefabs,transform.position,transform.rotation);
            gameObject.SetActive(false);

            AudioManmager.PlayDeathAudio();

            //­«·s¸ü¤J
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Gamemanager.PlayerDied();
        }
    }
}
