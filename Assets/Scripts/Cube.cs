using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube {
    public GameObject generateCube;
    public GameObject animeCube; //todo ここ整理
    public bool isAnimeCubeAvailable = true;
    public bool isCheckedCube = false; //todo スコープ狭く
    public Quaternion rotSave = new Quaternion(0, 0, 0, 0);
    public DiceData diceData = new DiceData();
    public int fallAmount = 0;
    public int posX = 0;
    public int posY = 0;
    
    public CubeController CubeCon;


    public Cube(int type){ //TODO overload??? 生成時の設定
        if (type != 0) {
            switch (type) {//TODO Add type
                case 0:
                    
                    break;
                

            }
        }
    }
    public Cube() { //TODO overload???
        isAnimeCubeAvailable = true;
        isCheckedCube = false;
    }

    public class DiceData {
        public DiceData() { //TODO overload???
        }
        public int upNum = 0, downNum = 5, fNum = 1, backNum = 4, leftNum = 3, rightNum = 2; //logChange
        //public int upNum = 0, downNum = 5, fNum = 1, backNum = 1, leftNum = 3, rightNum = 2; 
        public void rotationDataMove(Direction toMove) {
            int uWork, dWork, fWork, bWork, lWork, rWork;
            switch (toMove) {
                case Direction.Up:
                    uWork = upNum;
                    dWork = downNum;
                    fWork = fNum;
                    bWork = backNum;

                    upNum = fWork;
                    downNum = bWork;
                    fNum = dWork;
                    backNum = uWork;
                    break;
                case Direction.Right:
                    lWork = leftNum;
                    fWork = fNum;
                    rWork = rightNum;
                    bWork = backNum;

                    leftNum = bWork;
                    fNum = lWork;
                    rightNum = fWork;
                    backNum = rWork;
                    break;
                case Direction.Down:
                    uWork = upNum;
                    dWork = downNum;
                    fWork = fNum;
                    bWork = backNum;

                    upNum = bWork;
                    downNum = fWork;
                    fNum = uWork;
                    backNum = dWork;
                    break;
                case Direction.Left:
                    lWork = leftNum;
                    fWork = fNum;
                    rWork = rightNum;
                    bWork = backNum;

                    leftNum = fWork;
                    fNum = rWork;
                    rightNum = bWork;
                    backNum = lWork;
                    break;
            }
        }
        public void init() {
            //upNum = 0; downNum = 5; fNum = 1; backNum = 1; leftNum = 3; rightNum = 2;
            upNum = 0; downNum = 5; fNum = 1; backNum = 4; leftNum = 3; rightNum = 2;
        }
    }


    public void AnimeCubeRotate(Direction r, float xAmount, float yAmount) {
        int[] xDirection = { 90, 0, -90, 0 };
        int[] yDirection = { 0, 90, 0, -90 };
        /*
        Vector3Int[] typesOfRotation = { new Vector3(90,0,0), new Vector3(0,90,0),
                                      new Vector3(-90,0,0),new Vector3(0,-90,0) };*/
        RotateDirectXY(xDirection[(int)r] * xAmount, yDirection[(int)r] * yAmount);
    }
    public void AnimeCubeRotate(Direction r) {
        int[] xDirection = { 90, 0, -90, 0 };
        int[] yDirection = { 0, 90, 0, -90 }; //-yDirection fix
        RotateDirectXY(xDirection[(int)r], -yDirection[(int)r]);
    }

    //TODO
    public void RotateDirectXY(float xAmount,float yAmount) {
        if (this.isAnimeCubeAvailable) {
            this.animeCube.transform.Rotate(new Vector3(xAmount, yAmount, 0), Space.World);
        }
        
    }
    public void FlickProcess(Direction dir) {
        this.AssignRot();
        this.AnimeCubeRotate(dir);
        this.SaveCubeRot();
        this.diceData.rotationDataMove(dir);
    }
    public void Smaller() {
        if (this.isAnimeCubeAvailable) {
            this.generateCube.GetComponent<CubeController>().Small();
        }
    }
    public void Bigger() {
        if (this.isAnimeCubeAvailable) {
            this.generateCube.GetComponent<CubeController>().Big();
        }
    }
    public void AssignRot() {
        if (this.isAnimeCubeAvailable) {
            this.animeCube.transform.rotation = this.rotSave;
        }
    }
    public void SaveCubeRot() {
        if (this.isAnimeCubeAvailable) {
            this.rotSave = this.animeCube.transform.rotation;
        }
    }

    public void FallCube(Vector3 tPos) { //todo Vector3を引数にするべき??
        // It doesn't have to be generateCube to move
        //Vector3 tPos = board.cubePositions[i, j];
        this.CubeCon.Fall(tPos);
    }

    public static void CubeDataExchange(Cube t1, Cube t2) {

        GameObject tmp = t1.generateCube;
        t1.generateCube = t2.generateCube;
        t2.generateCube = tmp;

        tmp = t1.animeCube;
        t1.animeCube = t2.animeCube;
        t2.animeCube = tmp;

        Quaternion tmpQuat = t1.rotSave;
        t1.rotSave = t2.rotSave;
        t2.rotSave = tmpQuat;

        DiceData tmpDice = t1.diceData;
        t1.diceData = t2.diceData;
        t2.diceData = tmpDice;

        CubeController ccTemp = t1.CubeCon;
        t1.CubeCon = t2.CubeCon;
        t2.CubeCon = ccTemp;


        int _i = t1.CubeCon.i;
        int _j = t1.CubeCon.j;
        t1.CubeCon.i = t2.CubeCon.i;
        t1.CubeCon.j = t2.CubeCon.j;
        t2.CubeCon.i = _i;
        t2.CubeCon.j = _j;
        
        bool bTmp = t1.isAnimeCubeAvailable;
        t1.isAnimeCubeAvailable = t2.isAnimeCubeAvailable;
        t2.isAnimeCubeAvailable = bTmp;


    }
    public void InitCubeData(){
        isAnimeCubeAvailable = true;
        isCheckedCube = false;
        fallAmount = 0;
        this.InitRotation();
    }
    public void InitRotation() {
        this.rotSave = new Quaternion(0, 0, 0, 0); //todo make init
        this.generateCube.transform.rotation = rotSave;
        this.animeCube.transform.rotation = rotSave;
        this.diceData.init();
        //upNum = 0, downNum = 5, fNum = 1, backNum = 4, leftNum = 3, rightNum = 2;
    }
    public void CubeRandomDirction() {

        int t = UnityEngine.Random.Range(3,7);
        for (int i = 0; i < t; i++) {
            int r = UnityEngine.Random.Range(0, 4);
            FlickProcess((Direction)r);
        }
    }
}
