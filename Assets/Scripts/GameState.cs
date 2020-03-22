using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏状态模型
/// </summary>
public class GameState {

    static GameState state;

    #region 单例
    public static GameState instance
    {
        get
        {
            if (state == null)
            {
                state = new GameState();
            }
            return state;
        }
    }
    #endregion

    public Queue<Model> GameObjPool = new Queue<Model>();
    public int currentCombos = 0;

    public int ownCoins = 100000;

    public int costCoins;

    public int level1_bricks;
    public int level2_bricks;
    public int level3_bricks;

    public int currentLevel;

    /// <summary>
    /// 关卡显示状态，用key表示当前关，Stack记录进度
    /// </summary>
    public Dictionary<int, Stack<GameObject>> levelState = new Dictionary<int, Stack<GameObject>>();

    /// <summary>
    /// 宝石消除数量。宝石类型（目前表示为颜色），数量
    /// </summary>
    public Dictionary<int, int> crashGems;

    public int totalScore;
    public bool isRestart = false;

    public Queue<AwardNote> awardNotes = new Queue<AwardNote>();

    public Sprite[] gemSprites;
    public GameObject confirmWindow;

    public GameState() { }
}

/// <summary>
/// 大奖记录模型
/// </summary>
public class AwardNote
{
    public int awardCombo;
    public int crashGemsId;
    public int crashGemsCount;
    public long awardScore;
    public DateTime date;

    public AwardNote(int awardCombo, int crashGemsId, int crashGemsCount, long awardScore, DateTime date)
    {
        this.awardCombo = awardCombo;
        this.crashGemsId = crashGemsId;
        this.crashGemsCount = crashGemsCount;
        this.awardScore = awardScore;
        this.date = date;
    }

    public AwardNote()
    {
    }
}
