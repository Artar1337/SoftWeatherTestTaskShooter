using UnityEngine;

//
// класс для осмотра по сторонам с помощью мыши
//

public class PlayerLookDirectionChecker : MonoBehaviour
{
    //чувствительность мышки
    private float _mouseSensitivity = 300f;
    //игрок, которого поворачиваем
    private Transform _playerBody;
    //поворот по оси x
    private float _XRotation = 0f;
 
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
        //ограничиваем угол обзора по вертикали, чтобы персонаж не наматывался на валы
        _XRotation = Mathf.Clamp(_XRotation, -90f, 76f);
        //поворачиваем камеру
        transform.localRotation = Quaternion.Euler(_XRotation, 0f, 0f);
        //поворачиваем игрока
        _playerBody.Rotate(Vector3.up * mouseX);
    }
}
