using System;
using UnityEngine;

public class Logics {

    public static float CalculateDiffXY(Vector2 pos,Vector2 sPos) {

        GameManeger.diffX = (pos.x - sPos.x) / GameManeger.quarterWidth; //‰¡ˆÚ“®—Ê double???
        GameManeger.diffY = (pos.y - sPos.y) / GameManeger.quarterWidth; //cˆÚ“®—Ê
        return Mathf.Abs(GameManeger.diffX) - Mathf.Abs(GameManeger.diffY);
    }

    public static Vector3 CalculateGenePos(int i, int j) {
        
        float x = GameManeger.min.x + (GameManeger.one + GameManeger.two * i);
        float d = GameManeger.max.y - GameManeger.max.x;
        float y = d + GameManeger.min.y + (GameManeger.one + GameManeger.two * j);
         
        return new Vector3(x, y, 0);
    }

    public static void ArrayInit(int[] array,int initData) {
        for (int i=0;i<array.Length;i++) {
            array[i] = initData;
        }
    }




}
