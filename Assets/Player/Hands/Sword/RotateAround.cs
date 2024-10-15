using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [Header("RotationSwordSettings")]
    [SerializeField] private bool ShouldAct = false;
    Camera mainCam;
    private Vector3 mousePos;
    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (ShouldAct)
        {
            matchRotation();
        }
    }

    public void matchRotation()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotZ + 35);
    }
}
