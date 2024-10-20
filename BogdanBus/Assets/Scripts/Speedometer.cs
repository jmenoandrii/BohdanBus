using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro _textMesh;
    [SerializeField]
    private Bus _bus;

    void Update()
    {
        _textMesh.text = Mathf.RoundToInt(_bus.CurrentSpeed).ToString();
    }
}
