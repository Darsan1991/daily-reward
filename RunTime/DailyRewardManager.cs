
using System;
using System.Linq;
using DGames.ObjectEssentials.Scriptable;
using UnityEngine;

namespace DGames.DailyRewards
{
    // ReSharper disable once HollowTypeName
    public class DailyRewardManager:Singleton<DailyRewardManager>
    {

        [SerializeField] private ValueField<int> _coinsField = new("COINS");
#if DAILY_REWARD

        private static DateTime LastRewardTime
        {
            get
            {
                var time = long.Parse(PlayerPrefs.GetString(nameof(LastRewardTime), "0"));
                return new DateTime(time);
            }
            set => PlayerPrefs.SetString(nameof(LastRewardTime), value.Ticks.ToString());
        }



        public static int PendingRewardValue
        {
            get
            {
                if (!HasPendingDailyReward)
                    return -1;

                var val = PlayerPrefs.GetInt(nameof(PendingRewardValue), -1);

                if (val <= 0)
                {
                    PendingRewardValue = DailyRewardSettings.Default.Rewards.ElementAt(UnityEngine.Random.Range(0
                        , DailyRewardSettings.Default.Rewards.Count()));
                    return PendingRewardValue;
                }

                return val;
            }
            set => PlayerPrefs.SetInt(nameof(PendingRewardValue), value);
        }

        public static bool HasPendingDailyReward => DateTime.Now.Subtract(LastRewardTime).Days > 0;

        public static bool GetReward(int multiplexer=1)
        {
            if (!HasPendingDailyReward)
                return false;

            Instance._coinsField.Value.Set(PendingRewardValue * multiplexer);
            PendingRewardValue = -1;
            LastRewardTime = DateTime.Now;
            return true;
        }
#endif
    }
    
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;
                DontDestroyOnLoad(gameObject);
                OnAwake();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnAwake()
        {

        }
    }
}
