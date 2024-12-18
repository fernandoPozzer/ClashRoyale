using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElixirManager : MonoBehaviour
{
    private float currentElixir = 0f;
    private float lastIncreaseTime = 0f;
    private const float timePerDecimal = 0.1f;

    [SerializeField]
    private Image currentElixirImage;

    [SerializeField]
    private TMP_Text label;

    void Start()
    {
        lastIncreaseTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= lastIncreaseTime + timePerDecimal)
        {
            currentElixir = Mathf.Min(currentElixir + 0.1f, 10f);
            currentElixirImage.fillAmount = currentElixir / 10f;

            int labelValue = (int)currentElixir;
            label.text = labelValue.ToString();

            lastIncreaseTime = Time.time;
        }
    }
}
