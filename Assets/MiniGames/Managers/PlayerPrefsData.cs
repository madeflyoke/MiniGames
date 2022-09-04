using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.Managers
{
    public class PlayerPrefsData
    {
        public Dictionary<LevelType, bool> IsLevelCompletedOnce;

        public PlayerPrefsData()
        {
            IsLevelCompletedOnce = new();
            foreach (var item in Enum.GetValues(typeof(LevelType)))
            {          
                string key = item.ToString();
                if (PlayerPrefs.HasKey(key) == false)
                {
                    PlayerPrefs.SetInt(key, 0);
                }
                IsLevelCompletedOnce[(LevelType)item] = PlayerPrefs.GetInt(key) == 0 ? false : true;
            }
        }

        public void SaveFirstComplition(LevelType levelType)
        {
            PlayerPrefs.SetInt(levelType.ToString(), 1);
            IsLevelCompletedOnce[levelType] = true;
        }

        public void ClearData()
        {
            PlayerPrefs.DeleteAll();
        }

    }
}

