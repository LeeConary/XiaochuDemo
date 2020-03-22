using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global  {

    public static Model[,] gems;

    public static decimal ScoreRate(int gemId, int combos)
    {  
        switch (gemId)
        {
            case 0:
                return Rate(combos, 0);
            case 1:
                return Rate(combos, 1);
            case 2:
                return Rate(combos, 2);
            case 3:
                return Rate(combos, 3);
            case 4:
                return Rate(combos, 4);
            default:
                break;
        }
        return 0;
    }

    static decimal Rate(int combos, int pow)
    {
        float base_4 = 0.01f;
        float base_5 = 0.02f;
        float base_6 = 0.1f;
        float base_7 = 0.5f;
        float base_8 = 1f;
        float base_9 = 2f;
        float base_10 = 4f;
        float base_11 = 6f;
        float base_12 = 10f;
        float base_13 = 20f;
        float base_14 = 50f;
        float base_up15 = 100f;

        string rate;

        if (combos == 4) rate = (base_4 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 5) rate = (base_5 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 6) rate = (base_6 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 7) rate = (base_7 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 8) rate = (base_8 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 9) rate = (base_9 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 10) rate = (base_10 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 11) rate = (base_11 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 12) rate = (base_12 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 13) rate = (base_13 * Mathf.Pow(2, pow)).ToString();
        else if (combos == 14) rate = (base_14 * Mathf.Pow(2, pow)).ToString();
        else if (combos >= 15) rate = (base_up15 * Mathf.Pow(2, pow)).ToString();
        else rate = 1.ToString();

        return decimal.Parse(rate);
    }
}
