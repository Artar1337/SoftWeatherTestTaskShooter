﻿using UnityEngine;

//
//счетчик фпс
//

public class FPSCounter : MonoBehaviour
{
    private TMPro.TMP_Text text;

    private void Start()
    {
        text = transform.Find("Text").GetComponent<TMPro.TMP_Text>();
        InvokeRepeating(nameof(UpdateFPS), 1, 1);
    }

    public void UpdateFPS()
    {
        text.text = (int)(1f / Time.unscaledDeltaTime) + " FPS";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            text.gameObject.SetActive(!text.gameObject.activeInHierarchy);
    }
}
