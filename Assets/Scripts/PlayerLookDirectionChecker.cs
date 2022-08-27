using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookDirectionChecker : MonoBehaviour
{
    public float _mouseSensitivity = 1000f;
    public bool _canMove = true;

    private Transform _playerBody;
    private float _XRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerBody = GameObject.Find("FPS Controller").transform;
        _mouseSensitivity = PreferencesHandler.instance.Sens;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _XRotation -= mouseY;
        _XRotation = Mathf.Clamp(_XRotation, -90f, 76f);
        transform.localRotation = Quaternion.Euler(_XRotation, 0f, 0f);

        _playerBody.Rotate(Vector3.up * mouseX);
    }
}
