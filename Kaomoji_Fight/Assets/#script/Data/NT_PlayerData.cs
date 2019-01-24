using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NT_PlayerData{

    //プレイヤーデータ
    private string name = "";
    private Sprite Face;
    private Color color;
    private string SelectStage = "";

    public static NT_PlayerData Instance
    {
        get;
        private set;
    }

    public NT_PlayerData(string PlayerName = "むめい", Sprite PlayerFace = null, Color PlayerColor = new Color(), string stage = "stage1")
    {
        Instance = this;

        name = PlayerName;
        Face = PlayerFace;
        color = PlayerColor;
        SelectStage = stage;
    }
}
