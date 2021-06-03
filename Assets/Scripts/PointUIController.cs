using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointUIController : MonoBehaviour {

    Text pointText;
    GameObject pointObj;
    Animator anime;
    // Start is called before the first frame update
    void Start(){
        // todo refine
        //pointText = this.transform.GetChild(0).GetComponent<Text>();
        pointText = this.GetComponent<Text>();
        anime = pointText.GetComponent<Animator>();
        anime = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() { //todo should gathering update ??

    }
    /*
    if (Convert.ToInt32(pointText.text) != Board.points) {
            
    } 
    */

    public void Anime() {
        pointText.text = Board.points.ToString();
        GetComponent<Animator>().Play("PointAnimation");
        anime.Play("PointAnimation");
    }
}
