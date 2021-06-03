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
    public static string LoadCubeName = "GenerateCube2"; //GenerateCube2_6 //LogChange
    public static string puzzleCubeName = "GenerateCube2";
    public static string logCubeName = "GenerateCube2_8";
    public static float fallSpeed = 2f; //2f


}
public static class ScreenSettings {


}
public enum GameMode : int {
    Puzzle,
    SixSides,
    Coldness
}
public enum GameState1 : int {
    plane,
    touch,
    fall
}
public enum GameState : int {
    Prepare,
    Check,
    Flickable,
    Effect,
    CanFall,
    Fall
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
    public static GameObject particleObjPrefabs;
    public static GameObject starParticleObjPrefabs;
    GameObject canvas;
    public static GameObject clearText;

    public static bool isInputLegal = true;
    public static GameState gameState = GameState.Prepare;
    public FlickState preState = FlickState.firstTime; //0:first,1:x,2:y,

    Direction direction;

    public static Board board; //TODO add "static" for use by Events & GenerateCubeCon  


    public float cameraPosition = -6;

    public GameMode gameMode = GameMode.SixSides;

    void Start() {
        if (gameMode == GameMode.Puzzle) {
            GameSettings.LoadCubeName = GameSettings.puzzleCubeName;
        } else if (gameMode == GameMode.SixSides) {
            GameSettings.LoadCubeName = GameSettings.puzzleCubeName;
        } else if (gameMode == GameMode.Coldness) {
            GameSettings.LoadCubeName = GameSettings.logCubeName;
        }

        ObjectFind();

        culcScreenAndSize();
        board.FieldPreparation(gameMode);
        Initialize();
        //board.BoardCubeRandomFlick();
        board.BoardCubeFlick();
        gameState = GameState.Check;

    }


    // Update is called once per frame
    void Update() {

        //SliderCon.slider.value -= 0.0004f;
        //Debug.Log("gameMOde:" + gameMode);
        if (gameMode == GameMode.Puzzle) {
            if (Input.GetMouseButtonDown(0)) {
                calculateXYandInputCheck();
            } else if (Input.GetMouseButton(0) && isInputLegal) {
                calculateRotateAmount();
            } else if (Input.GetMouseButtonUp(0) && isInputLegal) {
                WhichDirection();
                if (board.isAllSameColor()) {
                    float clearWaittime = 0.16f;
                    Invoke("WaitingVisibleUI", clearWaittime);
                    Debug.Log("----- Fin!! -----");
                } else { //DebugYo
                    clearText.gameObject.SetActive(false);
                }
            }
        } else if (gameMode == GameMode.SixSides) {
            GameMode1();
        } else if (gameMode == GameMode.Coldness) {
            GameMode1();
        }



    }
    public void GameMode1() {
        DebugTimeControll();

        if (gameState == GameState.Check) {
            board.InitIsCheckedCube(board.boardSizeX, board.boardSizeY);
            bool k = board.CanDeleteAndSearch(board.boardSizeX, board.boardSizeY,gameMode);
            if (k) {
                gameState = GameState.Effect;
                /* change gamestate to CanFall*/
                board.DeleteStackedCube();
                StartCoroutine("Corou1");

            } else {
                GameManeger.gameState = GameState.Flickable;
            }
        } else if (gameState == GameState.Flickable) {
            if (Input.GetMouseButtonDown(0)) {
                calculateXYandInputCheck();
            } else if (Input.GetMouseButton(0) && isInputLegal) {
                calculateRotateAmount();
            } else if (Input.GetMouseButtonUp(0) && isInputLegal) {
                WhichDirection();

                gameState = GameState.Check;

            }
        } else if (gameState == GameState.CanFall) {
            board.MoveCube();
            gameState = GameState.Fall;
        } else if (gameState == GameState.Fall && !board.HasFallCube()) {

            GameManeger.gameState = GameState.Check;
        }
    }
    IEnumerator Corou1() {
        
        yield return new WaitForSeconds(0.22f);
        //Debug.Log("Corou1");
        GameManeger.gameState = GameState.CanFall;

    }

    public void culcScreenAndSize() {
        min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -cameraPosition));
        max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -cameraPosition));
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
        particleObjPrefabs = (GameObject)Resources.Load("Prefabs/particleObj2");
        starParticleObjPrefabs = (GameObject)Resources.Load("Prefabs/StarParticleObj");
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
                board.CubesBiggerExclude(x, y, false);
                board.CubesSmaller(x, y, true);
            } else if (preState == FlickState.firstTime) {
                board.CubesSmaller(x, y, true);
            }

            diffXY *= Mathf.Pow(1.4f, -diffXY + 0.5f); //TODO should i fix this like a 2/x 
            //TODO -diffXY 直す??
            yRotate = -diffXY;
            direction = Logics.JudgeDirectionX(diffX);
            //TODO else ここにpreState = FlickState.forstTime; いれる??

            preState = FlickState.xRotated;
        } else if (diffXY < 0) { //Yの方が大きい
            if (preState == FlickState.xRotated) {
                board.AssignRotExclude(x, y, true);
                board.CubesBiggerExclude(x, y, true);
                board.CubesSmallerExclude(x, y, false);
            } else if (preState == FlickState.firstTime) {
                board.CubesSmaller(x, y, false);
            }
            diffXY *= Mathf.Pow(1.4f, diffXY + 0.5f);
            xRotate = -diffXY;
            direction = Logics.JudgeDirectionY(diffY);

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
                direction = Logics.JudgeDirectionX(diffX);
                isTargetRow = false;
            } else if (diffXY < 0) { //Yの方が大きい
                direction = Logics.JudgeDirectionY(diffY);
                isTargetRow = true;
            }
            //board.CubesBigger(x, y, isX);
            board.Flick(x, y, direction);
        } else {

        }
        board.CubesBiggerByScale(x, y);
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


        }

    }


    public void WaitingVisibleUI() {
        clearText.gameObject.SetActive(true); //これで大きくならないバグ??EventSystem.current.IsPointerOverGameObject
    }
    
    public void restart() {
        for (int i = 0; i < board.cubes.GetLength(0); i++) {
            for (int j = 0; j < board.cubes.GetLength(1); j++) {
                Destroy(board.cubes[i, j].generateCube);
                Destroy(board.particle[i, j]);
                Destroy(board.starParticle[i, j]);
            }
        }

        board.cubes = new Cube[board.boardSizeX, board.boardSizeY];
        board.deleteCount = new int[board.boardSizeX];

        culcScreenAndSize();

        if (gameMode == GameMode.Puzzle) {
            //Debug.Log("puzzle");
            GameSettings.LoadCubeName = GameSettings.puzzleCubeName;
        } else if (gameMode == GameMode.SixSides) {
            //Debug.Log("sixside");
            GameSettings.LoadCubeName = GameSettings.puzzleCubeName;
        } else if (gameMode == GameMode.Coldness) {
            GameSettings.LoadCubeName = GameSettings.logCubeName;
        }
        ObjectFind();
        board.FieldPreparation(gameMode);
        Initialize();
        //board.BoardCubeRandomFlick();
        board.BoardCubeFlick();
        gameState = GameState.Check;
        //Debug.Log("restart");
        //Debug.Log("gameMOde:"+gameMode);
    }

    public static void DebugTimeControll(){
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
    }

}
