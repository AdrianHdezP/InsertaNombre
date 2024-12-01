using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinner : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.attachedRigidbody && other.attachedRigidbody.GetComponent<PlayerMovement>())
        {
            SceneManager.LoadScene(2);
        }
    }
}
