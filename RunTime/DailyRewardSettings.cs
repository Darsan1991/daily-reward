using System.Collections.Generic;
using DGames.Essentials.Attributes;
using DGames.Essentials.Unity;
using UnityEngine;

namespace DGames.DailyRewards
{
    [DashboardResourceItem(path:"Settings")]
    public partial class DailyRewardSettings
    {
        public static DailyRewardSettings Default => Resources.Load<DailyRewardSettings>(nameof(DailyRewardSettings));
        
                
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MyGames/Settings/DailyRewardSettings")]
        public static void Open()
        {
            ScriptableEditorUtils.OpenOrCreateDefault<DailyRewardSettings>();
        }
#endif
    }

    public partial class DailyRewardSettings : ScriptableObject
    {
        [ScriptableSymbolsToggle("","DAILY_REWARD",BuildTargetGroup.Android | BuildTargetGroup.iOS)][SerializeField] private bool _active;
        [NoLabel][SerializeField] private List<int> _rewards = new();

        public bool Active => _active;

        public IEnumerable<int> Rewards => _rewards;
    }
    
    public partial class DailyRewardSettings
    {
        public const string ACTIVE_FIELD = nameof(_active);
        public const string REWARDS = nameof(_rewards);
    }
}