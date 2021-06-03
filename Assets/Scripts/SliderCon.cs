using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCon : MonoBehaviour {
    public static Slider slider;
    void Start() {
        slider = this.GetComponent<Slider>();
        slider.value = 1;
    }
}
