using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {
    //Vector3 startPos;
    public Vector3 endPos;
    //Board board;
    public bool hasFall = true;
    //public bool isGeneSoon = true;
    //public Vector3 pos;

    public int i = 0;
    public int j = 0;

    Animator animator;
    GameObject AnimeCube;

    ParticleSystem particle;
    //float add = 0;
    public enum CubeState : int {
        plane,
        fall,
        effect
    }

    private void Start() {
        
        AnimeCube = this.transform.GetChild(0).gameObject;
        animator = AnimeCube.GetComponent<Animator>();
        particle = this.GetComponent<ParticleSystem>();
    }
    public CubeController() {

    }
    void Awake() {
        //animator = GetComponent<Animator>();
        //add = 0;
    }
    float goalPosY;
    void Update() {
        /*if (isFall && this.transform.position.y > goalPosY) {
            transform.position += new Vector3(0, -GameSettings.fallSpeed, 0);
        }*/
        //float p1 = transform.position.y * 1000;
        //float p2 = pos.y * 1000;

        if (this.transform.position.y == endPos.y) { // && !isGeneSoon
            if (hasFall) {
                //Debug.Log("-----hasFall = false----");
                
                hasFall = false;
                //add = 0;
            }

        } else {
            //add += 0.015f;
            transform.position = Vector3.MoveTowards(transform.position, endPos, (GameSettings.fallSpeed) * Time.deltaTime);//add
            //Debug.Log("-----hasFall = true----");
            if (!hasFall) {
                //Debug.Log("-----hasFall = true----");
                hasFall = true;
            }

        }

        /*
        if (this.transform.position.y == pos.y) { // && !isGeneSoon
            if (Board.isFall_Test[i,j]) {
                //Debug.Log("-----hasFall = false----");
                Board.isFall_Test[i,j] = false;
            }

        } else {
            transform.position = Vector3.MoveTowards(transform.position, endPos, GameSettings.fallSpeed * Time.deltaTime);
            //Debug.Log("-----hasFall = true----");
            if (!Board.isFall_Test[i,j]) {
                //Debug.Log("-----hasFall = true----");
                Board.isFall_Test[i,j] = true;
            }
        }*/
    }

    public void Small() {
        AnimeCube.GetComponent<Animator>().Play("Smaller");
        //animator.SetTrigger("SmallerTrigger");
    }
    public void Big() {
        AnimeCube.GetComponent<Animator>().Play("Bigger");
        //animator.SetTrigger("BiggerTrigger");
    }

    public void Fall(Vector3 ePos) {
        //Debug.Log("call fall in cc  epos:"+ePos);
        endPos = ePos;
        hasFall = true;
        //isGeneSoon = false;
    }

    public void PlayParticle() {
       
        particle.Play();
    }
    public void DisappearSmall() {
        AnimeCube.GetComponent<Animator>().Play("DisappearSmallAnimation");
    }
}

