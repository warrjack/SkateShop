using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider slider;

    private float fillSpeed = 0.5f;
    private float targetProgress = 0;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetProgress)
        {
            slider.value += fillSpeed * Time.deltaTime;
        }
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            IncrementProgress(0.05f);
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            slider.value = 0;
            targetProgress = 0f;
        }*/
    }

    public void IncrementProgress(float newProgress)
    {
        targetProgress = slider.value + newProgress;
    }

    public void ResetProgress()
    {
        slider.value = 0;
        targetProgress = 0;
    }
}
