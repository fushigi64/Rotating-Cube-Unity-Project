using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Events : MonoBehaviour {

    public GameObject cubeA;
    public Cube cubeB;
    //public GameObject pointObj;
    public int i;
    public int j;

    GameManeger gameManeger;
    Board board;
    GameObject canvas;
    GameObject DebugPanel;
    InputField inputField;
    Dropdown dropdown;
    void Start() {
        gameManeger = GameObject.FindWithTag("MainCamera").GetComponent<GameManeger>();
        //board = gameManeger.board;
        board = GameObject.FindWithTag("Stage").GetComponent<Board>();
        canvas = transform.parent.gameObject;
        
    }

    void Update() {

    }


    public void RandomOnClick() {
        board.BoardCubeRandomFlick();
    }
    public void DebugOnClick() {

        /*toggle*/
        DebugPanel = canvas.transform.Find("DebugPanel").gameObject;
        DebugPanel.gameObject.SetActive(!DebugPanel.activeSelf);

    }
    public void RestartOnClick() {
        gameManeger.restart();

    }
    public void PrintColorOnClick() {
        board.PrintColor();
        board.PrintIsAnime();
        
    }
    public void geneTwoCube() {
        /*
        Vector3 v = GameManeger.CalculateGenePos(0, 0);
        board.GenerateOneCube(0, 0, v, 1f);
        v = GameManeger.CalculateGenePos(0, 1);
        board.GenerateOneCube(0, 1, v, 1f);
        */
        
        Debug.Log("gene TwoCube Button Clicked!!!!!!!");
        //board.cubes[i,j].generateCube.transform.GetChild(0).gameObject.SetActive(true);
        //board.cubes[i, j].animeCube.gameObject.SetActive(true);
        //cubeA.transform.GetChild(0).gameObject.SetActive(true);
        board.cubes[0, 0].generateCube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }
    public void ClassChangeTestOnClick() {
        Debug.Log("befor 0,0 = " + board.cubes[0, 0].generateCube.transform.localPosition.x+","+ board.cubes[0, 0].generateCube.transform.localPosition.y);
        Debug.Log("befor 1,0 = " + board.cubes[1,0].generateCube.transform.localPosition.x + "," + board.cubes[0,1].generateCube.transform.localPosition.y);
        Cube.CubeDataExchange(board.cubes[0, 0], board.cubes[1,0]);

        Debug.Log("befor 0,0 = " + board.cubes[0, 0].generateCube.transform.localPosition.x + "," + board.cubes[0, 0].generateCube.transform.localPosition.y);
        Debug.Log("befor 1,0 = " + board.cubes[1,0].generateCube.transform.localPosition.x + "," + board.cubes[0, 1].generateCube.transform.localPosition.y);
    }

    public void TetsInputEnd() {
        inputField = DebugPanel.transform.Find("InputField").GetComponent<InputField>();
        Debug.Log("inputEnd");
        string text = inputField.text;
        inputField.text = "";
        /*
        if (text.Substring(0, 4) == "size") {

        }
        char a = text[0];
        char b = text[1];
        //board.boardSizeX
        */
        //board.DeleteHoge(Convert.ToInt32(text));
    }
    public void DropDownChange() {
        if (this.name == "Dropdown") {
            /*  Puzzle,
                SixSides,
                Coldness
            */
            gameManeger.gameMode = (GameMode)this.GetComponent<Dropdown>().value;
        } else if (this.name=="DropdownX") {
            board.boardSizeX = this.GetComponent<Dropdown>().value + 2;
        } else if (this.name == "DropdownY") {
            board.boardSizeY = this.GetComponent<Dropdown>().value + 2;
        }
        
    }
}



