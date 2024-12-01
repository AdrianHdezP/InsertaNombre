using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform cameraTF;

    private void Awake()
    {
        cameraTF = Camera.main.transform;
    }
    void Update()
    {
        Vector3 cameraFlatPos = cameraTF.position;
       cameraFlatPos.y = transform.position.y;

       transform.rotation = Quaternion.LookRotation(transform.position - cameraFlatPos, Vector3.up);
    }
}
