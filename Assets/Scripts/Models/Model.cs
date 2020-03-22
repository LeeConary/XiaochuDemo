using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {

    /// <summary>
    /// 种类：1-宝石；2-道具
    /// </summary>
    public int type;

    /// <summary>
    /// 物品标识
    /// 种类为宝石时：0-绿色，1-蓝色，2-黄色，3-红色，4-紫色
    /// 种类为道具时：0-炸弹
    /// </summary>
    public int id;

    /// <summary>
    /// 位置信息
    /// </summary>
    public Vector2 position;

    [HideInInspector]
    public bool isAsked = false;

    public Model up
    {
        get
        {
            try
            {
                int indexX = (int)position.x + 1;
                int indexY = (int)position.y;
                return Global.gems[indexX, indexY];
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        set { up = value; }
    }

    public Model down
    {
        get
        {
            try
            {
                int indexX = (int)position.x - 1;
                int indexY = (int)position.y;
                return Global.gems[indexX, indexY];
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        set { down = value; }
    }

    public Model left
    {
        get
        {
            try
            {
                int indexX = (int)position.x;
                int indexY = (int)position.y - 1;
                return Global.gems[indexX, indexY];
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        set { left = value; }
    }

    public Model right
    {
        get
        {
            try
            {
                int indexX = (int)position.x;
                int indexY = (int)position.y + 1;
                return Global.gems[indexX, indexY];
            }
            catch (System.Exception)
            {
                return null;
            }
        }
        set { right = value; }
    }

    public int posRow
    {
        get
        {
            return (int)position.x;
        }
    }

    public int posCol
    {
        get
        {
            return (int)position.y;
        }
    }
}
