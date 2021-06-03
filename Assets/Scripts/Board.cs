using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction : int {
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
    None = 4
}

public class Board : MonoBehaviour {
    public int boardSizeX = 7;
    public int boardSizeY = 7;
    public Cube[,] cubes;
    public Vector3[,] cubePositions;
    public bool[] isFallColumn;
    public static float cubeSize;
    public float cubeIntervalShrink = 0.9f;//0.9f

    public Stack<int> deleteCubeX = new Stack<int>();
    public Stack<int> deleteCubeY = new Stack<int>();

    public Queue<Cube> fallCubes = new Queue<Cube>();

    public static int points = 0;

    public int[] deleteCount;

    public PointUIController pointUI;

    public GameObject[,] particle;
    public GameObject[,] starParticle;

    //TODO いつ呼ばれるか調べる
    public Board (){
        cubes = new Cube[boardSizeX, boardSizeY];//Debug.Log("Created Board Instans ");
        deleteCount = new int[boardSizeX];
    }
    public void FieldPreparation(GameMode gm) {
        pointUI = GameObject.Find("PointUI").GetComponent<PointUIController>();
        isFallColumn = new bool[boardSizeX];
        cubes = new Cube[boardSizeX, boardSizeY];
        cubePositions = new Vector3[boardSizeX, boardSizeY + boardSizeY]; //todo fix

        particle = new GameObject[boardSizeX, boardSizeY];
        starParticle = new GameObject[boardSizeX, boardSizeY];
        for (int i = 0; i < boardSizeX; i++) {
            
            for (int j = 0; j < boardSizeY + boardSizeY; j++) {
                Vector3 pos = Logics.CalculateGenePos(i, j);
                cubePositions[i, j] = pos;
                if (j < boardSizeY) {
                    particle[i, j] = Instantiate(GameManeger.particleObjPrefabs, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) as GameObject;
                    starParticle[i, j] = Instantiate(GameManeger.starParticleObjPrefabs, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) as GameObject;
                    GenerateOneCube(i, j, pos, cubeSize,gm);
                }
                

            }
        }
        
    }
    public void GenerateOneCube(int i, int j, Vector3 pos, float cubeSize,GameMode gm) { //TODO??
        cubes[i, j] = new Cube();
        cubes[i, j].generateCube = Instantiate(GameManeger.generateCubePrefabs, pos, Quaternion.identity) as GameObject;
        cubes[i, j].animeCube = cubes[i, j].generateCube.transform.GetChild(0).gameObject;
        if (gm == GameMode.Coldness) {
            cubes[i, j].animeCube.transform.GetChild(0).localScale = new Vector3(cubeSize * 0.94f, cubeSize * 0.94f, cubeSize * 1.06f); //Change size of sizeCube LogChange
        } else {
            cubes[i, j].animeCube.transform.GetChild(0).localScale = new Vector3(cubeSize,cubeSize,cubeSize);
        }
        
        //cubes[i, j].animeCube.transform.GetChild(0).localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        cubes[i, j].rotSave = cubes[i, j].animeCube.transform.rotation;//todo necessary?
        cubes[i, j].posX = i;
        cubes[i, j].posY = j;
        cubes[i, j].CubeCon = cubes[i, j].generateCube.GetComponent<CubeController>();
        cubes[i, j].CubeCon.endPos = cubePositions[i, j];
        cubes[i, j].CubeCon.hasFall = true;

        cubes[i, j].CubeCon.i = i;
        cubes[i, j].CubeCon.j = j;
    }

    public bool isAllSameColor() {
        int front = cubes[0, 0].diceData.fNum;
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) {
                if (front == cubes[i, j].diceData.fNum) {
                    front = cubes[i, j].diceData.fNum;
                } else {
                    return false;
                }
            }
        }
        return true;
    }
    public void InitIsCheckedCube(int sizeX, int sizeY) {
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                cubes[i, j].isCheckedCube = false;
                cubes[i, j].isAnimeCubeAvailable = true; //TODO
            }
        }
    }
    public void Flick(int x, int y, Direction dir) {
        if (dir == Direction.Up || dir == Direction.Down) {
            for (int i = 0; i < boardSizeY; i++) {
                cubes[x, i].FlickProcess(dir);
            }
        } else {
            for (int i = 0; i < boardSizeX; i++) {
                cubes[i, y].FlickProcess(dir);
            }
        }
    }
    public void AssignRotAndRotate(Direction dir, float xRotate, float yRotate,int x,int y) {
        if (dir==Direction.Right || dir == Direction.Left) {
            for (int i = 0; i < boardSizeX; i++) {
                cubes[i, y].AssignRot();
                cubes[i, y].AnimeCubeRotate(dir, xRotate, yRotate);
            }
        } else {
            for (int i = 0; i < boardSizeY; i++) {
                cubes[x, i].AssignRot();
                cubes[x, i].AnimeCubeRotate(dir, xRotate, yRotate);
            }
        }
    }
    /*
    public void AssignRot() {
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) {
                cubes[i, j].AssignRot();
            }
        }
    }*/
    public void AssignRot(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeY; i++) {
                cubes[i, y].AssignRot();
            }
        } else {
            for (int i = 0; i < boardSizeX; i++) {
                cubes[x, i].AssignRot();
            }
        }
    }
    public void AssignRotExclude(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeX; i++) {
                if (i != x) {
                    cubes[i, y].AssignRot();
                }
            }
        } else {
            for (int i = 0; i < boardSizeX; i++) {
                if (i != y) {
                    cubes[x, i].AssignRot();
                }
            }
        }
    }
    
    public void CubesBiggerByScale(int x, int y) {
        for (int i = 0; i < boardSizeX; i++) { //TODO
            if (cubes[i, y].isAnimeCubeAvailable && cubes[i, y].animeCube.transform.localScale.x < 1 && i != x) {
                cubes[i, y].Bigger();
            }
        }
        for (int i = 0; i < boardSizeY; i++) { //TODO
            if (cubes[x, i].isAnimeCubeAvailable && cubes[x, i].animeCube.transform.localScale.x < 1) {
                cubes[x, i].Bigger();
            }
        }
    }
    public void CubesSmaller(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeX; i++) {
                cubes[i, y].Smaller();
            }

        } else if (!isOperateRow) {
            for (int i = 0; i < boardSizeY; i++) {
                cubes[x, i].Smaller();
            }
        }
    }
    public void CubesSmallerExclude(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeY; i++) {
                if (i != x) {
                    cubes[i, y].Smaller();
                }
            }
        } else if (!isOperateRow) {
            for (int i = 0; i < boardSizeX; i++) {
                if (i != y) {
                    cubes[x, i].Smaller();
                }
            }
        }
    }
    public void CubesBigger(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeX; i++) {
                cubes[i, y].Bigger();
            }
        } else if (!isOperateRow) {
            for (int i = 0; i < boardSizeY; i++) {
                cubes[x, i].Bigger();
            }
        }
    }
    public void CubesBiggerExclude(int x, int y, bool isOperateRow) {
        if (isOperateRow) {
            for (int i = 0; i < boardSizeX; i++) {
                if (i != x) {
                    cubes[i, y].Bigger();
                }
            }
        } else if (!isOperateRow) {
            for (int i = 0; i < boardSizeY; i++) {
                if (i != y) {
                    cubes[x, i].Bigger();
                }
            }
        }
    }

    public void MoveCube() {

        //DeleteStackedCube();
        Fall();
        AllisAnimeCubeAvailableAndisCheckedCubeInit();
    }
    public bool HasFallCube() {
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) {
                if (cubes[i, j].CubeCon.hasFall == true) { //
                    return true;
                }
            }
        }
        return false;
    }

    public bool CanDeleteAndSearch(int sizeX, int sizeY ,GameMode gm) {
        
        bool searchResult = false;  //Debug.Log("Search start");
        for (int i = 0; i < sizeX; i++) {
            for (int j = 0; j < sizeY; j++) {
                int cnt = 0;
                if (gm == GameMode.Coldness) {
                    
                    if (!cubes[i, j].isCheckedCube && cubes[i, j].isAnimeCubeAvailable && (cubes[i, j].diceData.fNum == 1 || cubes[i, j].diceData.fNum == 4)) { // && cubes[i, j].diceData.fNum ==1 //LogChange
                        RecursionSearch2(sizeX, sizeY, cubes, i, j, ref cnt);
                    }
                } else {
                    if (!cubes[i, j].isCheckedCube && cubes[i, j].isAnimeCubeAvailable) { // && cubes[i, j].diceData.fNum ==1 //LogChange
                        RecursionSearch(sizeX, sizeY, cubes, i, j, cubes[i, j].diceData.fNum, ref cnt);
                    }
                }

                if (cnt < GameSettings.deletableNumber) { //TODO ここのロジック改善
                    for (int k = 0; k < cnt; k++) {
                        int x = deleteCubeX.Pop();
                        int y = deleteCubeY.Pop();
                        cubes[x, y].isAnimeCubeAvailable = true; // Debug.Log("Poped__x:" + x + ",y:" + y);
                    }
                } else {
                    searchResult = true; //Debug.Log("xxxxxxxxxxxxxx  cnt>=4--------cnt:" + cnt + ",x:" + i + ",y:" + j);
                }
            }
        }
        return searchResult;
    }
    /*  再帰ver*/
    public void RecursionSearch(int sizeX, int sizeY, Cube[,] cubes, int x, int y, int diceNum, ref int cnt) {
        cnt++;
        cubes[x, y].isCheckedCube = true;
        deleteCubeX.Push(x);
        deleteCubeY.Push(y);
        if (x + 1 < sizeX && cubes[x + 1, y].diceData.fNum == diceNum && !cubes[x + 1, y].isCheckedCube && cubes[x + 1, y].isAnimeCubeAvailable) {
            RecursionSearch(sizeX, sizeY, cubes, x + 1, y, diceNum, ref cnt);
        }
        if (0 <= x - 1 && cubes[x - 1, y].diceData.fNum == diceNum && !cubes[x - 1, y].isCheckedCube && cubes[x - 1, y].isAnimeCubeAvailable) {
            RecursionSearch(sizeX, sizeY, cubes, x - 1, y, diceNum, ref cnt);
        }
        if (y + 1 < sizeY && cubes[x, y + 1].diceData.fNum == diceNum && !cubes[x, y + 1].isCheckedCube && cubes[x, y + 1].isAnimeCubeAvailable) {
            RecursionSearch(sizeX, sizeY, cubes, x, y + 1, diceNum, ref cnt);
        }
        if (0 <= y - 1 && cubes[x, y - 1].diceData.fNum == diceNum && !cubes[x, y - 1].isCheckedCube && cubes[x, y - 1].isAnimeCubeAvailable) {
            RecursionSearch(sizeX, sizeY, cubes, x, y - 1, diceNum, ref cnt);
        }
    }
    public void RecursionSearch2(int sizeX, int sizeY, Cube[,] cubes, int x, int y, ref int cnt) {
        cnt++;
        cubes[x, y].isCheckedCube = true;
        deleteCubeX.Push(x);
        deleteCubeY.Push(y);
        if (x + 1 < sizeX && (cubes[x + 1, y].diceData.fNum == 1 || cubes[x + 1, y].diceData.fNum == 4) && !cubes[x + 1, y].isCheckedCube && cubes[x + 1, y].isAnimeCubeAvailable) {
            RecursionSearch2(sizeX, sizeY, cubes, x + 1, y, ref cnt);
        }
        if (0 <= x - 1 && (cubes[x - 1, y].diceData.fNum == 1 || cubes[x - 1, y].diceData.fNum == 4) && !cubes[x - 1, y].isCheckedCube && cubes[x - 1, y].isAnimeCubeAvailable) {
            RecursionSearch2(sizeX, sizeY, cubes, x - 1, y, ref cnt);
        }
        if (y + 1 < sizeY && (cubes[x, y+1].diceData.fNum == 1 || cubes[x , y + 1].diceData.fNum == 4) && !cubes[x, y + 1].isCheckedCube && cubes[x, y + 1].isAnimeCubeAvailable) {
            RecursionSearch2(sizeX, sizeY, cubes, x, y + 1, ref cnt);
        }
        if (0 <= y - 1 && (cubes[x, y - 1].diceData.fNum == 1 || cubes[x, y - 1].diceData.fNum == 4) && !cubes[x, y - 1].isCheckedCube && cubes[x, y - 1].isAnimeCubeAvailable) {
            RecursionSearch2(sizeX, sizeY, cubes, x, y - 1, ref cnt);
        }
    }

    /* Destoroy以外の方法  Stackを指定して疎結合?*/
    public void DeleteStackedCube() {

        Logics.ArrayInit(deleteCount, 0);
        while (deleteCubeX.Count>0) {
            int x = deleteCubeX.Pop();
            int y = deleteCubeY.Pop();
            /*
            for (int number = y + 1; number < boardSizeY;number++) {
                
                if (cubes[x, number].isAnimeCubeAvailable) {
                    cubes[x, number].fallAmount++;
                    if (!fallCubes.Contains(cubes[x, number])) {
                        fallCubes.Enqueue(cubes[x, number]);
                    }
                }
            }*/
            for (int num = y + 1; num < boardSizeY; num++) {

                if (cubes[x, num].isAnimeCubeAvailable) {
                    cubes[x, num].fallAmount++;
                }
            }
            
            /* Animation によって小さくなったままの場合がある */
            cubes[x, y].animeCube.gameObject.transform.localScale = new Vector3(1,1,1);
            //TODO_01
            cubes[x, y].CubeCon.DisappearSmall();
            StartCoroutine("Corou1", (cubes[x, y]));
            //Debug.Log("SetActive   false");
            particle[x, y].GetComponent<ParticleSystem>().Play();
            starParticle[x, y].GetComponent<ParticleSystem>().Play();

            cubes[x, y].isAnimeCubeAvailable = false;
            deleteCount[x]++;  //Debug.Log("x:" + x + " ,gene++" + gene[x]);
            points++;
            pointUI.Anime();
        }
        deleteCubeX.Clear();
        deleteCubeY.Clear();
        //PrintFallAmount();

        for (int i=0;i<boardSizeX;i++) {
            for (int j=1;j < boardSizeY; j++) {
                if (cubes[i,j].fallAmount>0) {
                    fallCubes.Enqueue(cubes[i,j]);
                }
            }
        }


    }
    IEnumerator Corou1(Cube cube) {

        yield return new WaitForSeconds(0.21f);
        //Debug.Log("Corou1--------");
        cube.animeCube.transform.localScale = new Vector3(1, 1, 1);
        cube.animeCube.SetActive(false);
        //SliderCon.slider.value += 0.1f;
    }

    public void Fall() {
        int fallAmount = 0;

        while(fallCubes.Count>0) {

            Cube cubeU = fallCubes.Dequeue();
            fallAmount = cubeU.fallAmount;
            int x = cubeU.posX;
            int y = cubeU.posY;
            Cube cubeD = cubes[x, y - fallAmount];
            
            //Debug.Log("  fallCube:   x:"+x+",y:"+(y-fallAmount)+"fallamt="+fallAmount);
            /*
            cubeU.FallCube(cubePositions[x, y - fallAmount]);
            cubeU.isAnimeCubeAvailable = false;

            Cube.CubeDataExchange(cubeU, cubes[x, y - fallAmount]);
            cubeU.generateCube.transform.GetChild(0).gameObject.SetActive(false);
            cubes[x, y - fallAmount].generateCube.transform.GetChild(0).gameObject.SetActive(true);

            cubeU.CubeCon.hasFall = true;
            cubeU.isAnimeCubeAvailable = false; //todo <- sus

            cubes[x, y - fallAmount].isAnimeCubeAvailable = true;
            */
            
            cubeU.generateCube.transform.GetChild(0).gameObject.SetActive(true);
            cubeD.generateCube.transform.GetChild(0).gameObject.SetActive(true);
            //cubeU.FallCube(cubePositions[x, y - fallAmount]); //
            Cube.CubeDataExchange(cubeU,cubeD);
            cubeU.InitCubeData();
            cubeU.isAnimeCubeAvailable = false;


            cubeD.FallCube(cubePositions[x, y - fallAmount]);
            cubeD.CubeCon.hasFall = true;
            cubeD.isAnimeCubeAvailable = false;

            //cubeU.generateCube.transform.GetChild(0).gameObject.SetActive(false);
        }

        //Debug.Log("fall process start");
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 1; j <= deleteCount[i]; j++) {

                int thisY = (boardSizeY - 1) + j; //Debug.Log("i:" + i + ",j:" + j + ",gene[i]:" + gene[i]);
                int fallCnt = deleteCount[i];
                //Debug.Log("  thisY - fallAmt  : " + (thisY - fallCnt) + " ,fallAmt: " + fallCnt);
                
                cubes[i, thisY - fallCnt].InitCubeData(); //i,3
                cubes[i, thisY - fallCnt].generateCube.transform.localPosition = cubePositions[i, thisY]; //thisY
                cubes[i, thisY - fallCnt].generateCube.transform.GetChild(0).gameObject.SetActive(true);
                
                /* Animation によって小さくなったままの場合がある */
                //ubes[i, thisY - fallCnt].animeCube.gameObject.transform.localScale = new Vector3(4,2,2);

                cubes[i, thisY - fallCnt].CubeRandomDirction();
                

                cubes[i, thisY - fallCnt].FallCube(cubePositions[i, thisY - fallCnt]);
                cubes[i, thisY - fallCnt].isAnimeCubeAvailable = false;

                cubes[i, thisY - fallCnt].CubeCon.hasFall = true;
                
            }
        }

        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) {
                cubes[i, j].fallAmount = 0;
            }
        }

    }
    public void AllisAnimeCubeAvailableAndisCheckedCubeInit() {
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) { //TODO 
                cubes[i, j].isAnimeCubeAvailable = true;
                cubes[i, j].isCheckedCube = false;
            }
        }
    }

    public void BoardCubeRandomFlick() {
        GameManeger.clearText.gameObject.SetActive(false);
        int t = UnityEngine.Random.Range(boardSizeX * GameSettings.RandomFlickAmount, boardSizeY * GameSettings.RandomFlickAmount * 2);
        for (int i = 0; i < t; i++) {
            int x = UnityEngine.Random.Range(0, boardSizeX);
            int y = UnityEngine.Random.Range(0, boardSizeY);
            int r = UnityEngine.Random.Range(0, 4);
            this.Flick(x, y, (Direction)r);
        }
    }
    public void BoardCubeFlick() {
        GameManeger.clearText.gameObject.SetActive(false);
        for (int i = 0; i < boardSizeX; i++) {
            for (int j = 0; j < boardSizeY; j++) { //TODO 
                cubes[i, j].FlickProcess((Direction)((i+j)%4));
            }
        }
        
    }

    public void PrintColor() {
        //Debug.Log("<color=red>Hello, world!</color>");
        Debug.Log("-----------");
        for (int i = boardSizeY - 1; i >= 0; i--) {
            string hoge = "|";
            for (int j = 0; j < boardSizeX; j++) {
                int c = cubes[j, i].diceData.fNum;
                if (c==0) {
                    hoge += "<color=red>" + c + "</color>,";
                } else if (c == 1) {
                    hoge += "<color=orange>" + c + "</color>,";
                } else if (c == 2) {
                    hoge += "<color=green>"+c+ "</color>,";
                } else if (c == 3) {
                    hoge += "<color=yellow>" + c + "</color>,";
                } else if (c == 4) {
                    hoge += "<color=cyan>" + c + "</color>,";
                } else{
                    hoge += "<color=blue>" + c + "</color>,";
                }
            }
            Debug.Log(hoge+"|");
        }
        Debug.Log("-----------");
    }
    public void PrintIsAnime() {
        for (int i = boardSizeY - 1; i >= 0; i--) {
            string hoge = "";
            for (int j = 0; j < boardSizeX; j++) {
                hoge += cubes[j,i].isAnimeCubeAvailable;
            }
            Debug.Log(hoge);
        }
        Debug.Log("---^^isAnime^^--------");
    }
    public void PrintFallAmount() {
        for (int i = boardSizeY - 1; i >= 0; i--) {
            string hoge = "";
            for (int j = 0; j < boardSizeX; j++) {
                hoge += cubes[j, i].fallAmount+",";
            }
            Debug.Log(hoge);
        }
        Debug.Log("-----------");
    }


}
