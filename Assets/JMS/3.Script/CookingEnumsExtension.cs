using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    /// <summary>
    /// 재료가 조리된 정도
    /// </summary>
    public enum RipeState
    {
        None = 0,
        Raw = 1,
        Undercook = 2,
        Welldone = 3,
        Overcook = 4,
        Burn = 5,
    }

    public enum CookType
    {
        None = 0,
        Boil = 1,
        Broil = 2,
        Grill = 3,
    }

    public static class CookingEnumsExtension
    {
        #region RipeState
        public static RipeState ToRipeState(this string word)
        {
            switch (word)
            {
                case "RAW":
                    return RipeState.Raw;
                case "UNDERCOOK":
                    return RipeState.Undercook;
                case "WELLDONE":
                    return RipeState.Welldone;
                case "OVERCOOK":
                    return RipeState.Overcook;
                case "BURN":
                    return RipeState.Burn;
            }

            return RipeState.None;
        }

        public static string ToString(this RipeState ripeState)
        {
            switch (ripeState)
            {
                case RipeState.Raw:
                    return "RAW";
                case RipeState.Undercook:
                    return "UNDERCOOK";
                case RipeState.Welldone:
                    return "WELLDONE";
                case RipeState.Overcook:
                    return "OVERCOOK";
                case RipeState.Burn:
                    return "BURN";
            }

            return null;
        }
        #endregion
        #region CookType
        public static CookType ToCookType(this string word)
        {
            switch (word)
            {
                case "BOIL":
                    return CookType.Boil;
                case "BROIL":
                    return CookType.Broil;
                case "GRILL":
                    return CookType.Grill;
            }

            return CookType.None;
        }

        public static string ToString(this CookType cookType)
        {
            switch (cookType)
            {
                case CookType.None:
                    return "NONE";
                case CookType.Boil:
                    return "BOIL";
                case CookType.Broil:
                    return "BROIL";
                case CookType.Grill:
                    return "GRILL";
            }

            return null;
        }
        #endregion
    }
}
