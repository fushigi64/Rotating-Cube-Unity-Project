using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class GameSettings {
    public static string thisSceneName;
    public static int RandomFlickAmount = 1; //sizeもやがreadonly?const?static?
    public static float rotatableNum = 0.45f; //const

    public static int deletableNumber = 3;

    public static string game1SceneName = "Scene1";
    public static string LoadCubeName = "GenerateCube2"; //GenerateCube2_6
    public static float fallSpeed = 2f; //2f


}
public static class ScreenSettings {


}
public enum GameState : int {
    plane,
    touch,
    fall
}
public enum FlickState : int {
    firstTime,
    xRotated,
    yRotated
}

public class GameManeger : MonoBehaviour {

    static int width = Screen.width;
    static int height = Screen.height;

    static int diffHW = (Mathf.Max(width, height) - Mathf.Min(width, height)) / 2;
    static int upperLim = Mathf.Min(width, height) + diffHW;
    static int lowerLim = diffHW;
    static int leftLim;
    static int rightLim;
    public static int halfWidth = Screen.width / 2;
    public static int quarterWidth = Screen.width / 4;
    public static int _10ofWidth = Screen.width / 10; //端末の物理サイズによって変える??

    public static int x = 0, y = 0;

    public static Vector2 min;
    public static Vector2 max;
    public static float diff;
    public static float two;
    public static float one;
    public static float screenBuff;

    static Vector2 pos = new Vector2(0, 0);
    static Vector2 sPos = new Vector2(0, 0);
    public static float diffX, diffY, diffXY;

    public static GameObject generateCubePrefabs;
    GameObject canvas;
    public static GameObject clearText;

    public static bool isInputLegal = true;
    public static GameState gameState = GameState.plane;
    public FlickState preState = FlickState.firstTime; //0:first,1:x,2:y,

    Direction direction;

    public static Board board; //TODO add "static" for use by Events & GenerateCubeCon  

    public int mode = 0; // 0,1,2

    void Start(){
        
        ObjectFind();
        
        culcScreenAndSize();
        board.FieldPreparation();
        Initialize();
        //board.BoardCubeRandomFlick();
        board.BoardCubeFlick();


    }

    // Update is called once per frame
    void Update(){
        if (Input.GetKey(KeyCode.T)) {
            Time.timeScale = 0f;
        }
        if (Input.GetKey(KeyCode.R)) {
            Time.timeScale = 0.001f;
        }
        if (Input.GetKey(KeyCode.E)) {
            Time.timeScale = 0.01f;
        }
        if (Input.GetKey(KeyCode.W)) {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKey(KeyCode.Q)) {
            Time.timeScale = 1;
        }
        bool hasFall = false;

        for (int i = 0; i < board.boardSizeX; i++) {
            for (int j = 0; j < board.boardSizeY; j++) {
                if (board.cubes[i, j].CubeCon.hasFall == true) { //
                                                                 //Debug.Log("hasFall == true  forfor");
                    hasFall = true;
                    break;
                } else {
                    //Debug.Log("hasFall == false!!!!!!!!!!!!!!!!!");
                }
            }
        }

        if (!hasFall) {

            GameManeger.gameState = GameState.plane;
        }
        if (gameState == GameState.plane) {
            gameState = GameState.fall;
            //Debug.Log("movecube");
            board.MoveCube();

        }


        if (gameState == GameState.plane || gameState == GameState.touch) {
            if (Input.GetMouseButtonDown(0)) {
                calculateXYandInputCheck();
            } else if (Input.GetMouseButton(0) && isInputLegal) {
                calculateRotateAmount();
            } else if (Input.GetMouseButtonUp(0) && isInputLegal) {
                WhichDirection();
            }
        }




    }
    public void culcScreenAndSize() {
        min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0,6));
        max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1,6));
        screenBuff = max.x / 8;
        min.x += screenBuff;
        max.x -= screenBuff;
        
        diff = max.x - min.x;
        two = diff / (board.boardSizeX);
        one = two / 2;

        Board.cubeSize = diff / board.boardSizeX;
        Board.cubeSize *= board.cubeIntervalShrink;
    }

    public void ObjectFind() {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        clearText = canvas.transform.Find("ClearText").gameObject;
        generateCubePrefabs = (GameObject)Resources.Load("Prefabs/" + GameSettings.LoadCubeName);
        GameSettings.thisSceneName = SceneManager.GetActiveScene().name;
        board = GameObject.FindGameObjectWithTag("Stage").GetComponent<Board>();
    }
    public void Initialize() {
        clearText.SetActive(false);
        //should i make BoardInit method?
        board.InitIsCheckedCube(board.boardSizeX, board.boardSizeX);

        board.deleteCubeX.Clear();
        board.deleteCubeY.Clear();

        /* 生成準備 */
    }
    public void calculateXYandInputCheck() {
        
        if (EventSystem.current.IsPointerOverGameObject()) {
            isInputLegal = false;
            return;
        }
        
        Vector2 pos = Input.mousePosition;
        if ((int)pos.y < lowerLim || upperLim < (int)pos.y) {
            isInputLegal = false;
            return;
        }

        isInputLegal = true;
        sPos = pos;
        x = (int)(pos.x / (width / board.boardSizeX));
        y = (int)((pos.y - diffHW) / (width / board.boardSizeY));

    }
    public void calculateRotateAmount() {
        diffXY = Logics.CalculateDiffXY(Input.mousePosition, sPos);
        
        float xRotate = 0;
        float yRotate = 0;
        /* !! Note that x and y are reversed here !! */
        if (diffXY > 0) { //Xの方が大きい /*TODO メソッド引数のtrue,falseを改善する*/
            if (preState == FlickState.yRotated) {
                board.AssignRotExclude(x, y, false);
                board.CubesBiggerExclude(x,y,false);
                board.CubesSmaller(x, y, true);
            } else if (preState == FlickState.firstTime) {
                board.CubesSmaller(x, y, true);
            }

            diffXY *= Mathf.Pow(1.4f, -diffXY + 0.5f); //TODO should i fix this like a 2/x 
            //TODO -diffXY 直す??
            yRotate = -diffXY;
            JudgeDirectionX(diffX);
            //TODO else ここにpreState = FlickState.forstTime; いれる??

            preState = FlickState.xRotated;
        } else if (diffXY < 0) { //Yの方が大きい
            if (preState == FlickState.xRotated) {
                board.AssignRotExclude(x, y, true);
                board.CubesBiggerExclude(x, y,true);
                board.CubesSmallerExclude(x,y,false);
            } else if (preState == FlickState.firstTime) {
                board.CubesSmaller(x, y, false);
            }
            diffXY *= Mathf.Pow(1.4f, diffXY + 0.5f);
            xRotate = -diffXY;
            JudgeDirectionY(diffY);

            preState = FlickState.yRotated;
        } else {

        }
        board.AssignRotAndRotate(direction, xRotate, yRotate, x, y);
    }
    public void WhichDirection() {
        diffXY = Logics.CalculateDiffXY(Input.mousePosition, sPos);
        bool isTargetRow = true;
        if (Mathf.Abs(diffXY) >= GameSettings.rotatableNum) {

            if (diffXY > 0) { //Xの方が大きい
                JudgeDirectionX(diffX);
                isTargetRow = false;
            } else if (diffXY < 0) { //Yの方が大きい
                JudgeDirectionY(diffY);
                isTargetRow = true;
            }
            //board.CubesBigger(x, y, isX);
            board.Flick(x, y, direction);
        } else {

        }
        board.CubesBiggerByScale(x,y);
        //board.AssignRot(); // <---
        board.AssignRotExclude(x, y, isTargetRow); //TODO isTargertROw ではなくDirection??
        preState = FlickState.firstTime;
        if (GameSettings.thisSceneName == GameSettings.game1SceneName) {
            if (board.isAllSameColor()) {
                float clearWaittime = 0.16f;
                Invoke("WaitingVisibleUI", clearWaittime);
                Debug.Log("----- Fin!! -----");
            } else { //DebugYo
                clearText.gameObject.SetActive(false);
            }
            
        } else {
            gameState = GameState.fall; //falling??
        }

    }
    public void JudgeDirectionX(float diff) {
        if (diff > 0) {
            direction = Direction.Right;
        } else if (diff < 0) {
            direction = Direction.Left;
        }
    }
    public void JudgeDirectionY(float diff) {
        if (diff > 0) {
            direction = Direction.Up;
        } else if (diff < 0) { 
            direction = Direction.Down;
        }
    }
    public void WaitingVisibleUI() {
        clearText.gameObject.SetActive(true); //これで大きくならないバグ??EventSystem.current.IsPointerOverGameObject
    }
    public void restart() {
        for (int i = 0; i < board.boardSizeX; i++) {
            for (int j = 0; j < board.boardSizeY; j++) {
                Destroy(board.cubes[i, j].generateCube);
                //cubes[i, j] = null;
            }
        }
        ObjectFind();
        board.FieldPreparation();
        Initialize();
    }

}
