using _ClashRoyal.Scripts.Units.Base;
using _ClashRoyal.Scripts.Units.Base.Enums;
using _ClashRoyal.Scripts.Units.Base.Scriptables.Configs;

namespace _ClashRoyal.Scripts.Units
{
    public static class TargetFilter
    {
        public static bool CanAttack(Unit attacker, Unit target)
        {
            if (!attacker || !target) return false;
            
            var attackConfig = attacker.Parameters.GetConfig<UnitAttackConfig>();
            if (attackConfig == null) return false;
            
            var targetType = GetTargetType(target);
            if (targetType == TargetType.None) return false;
            
            return attackConfig.CanAttackTargets.HasFlag(targetType);
        }
        
        private static TargetType GetTargetType(Unit unit)
        {
            return unit.UnitType switch
            {
                UnitType.Ground => TargetType.Ground,
                UnitType.Air => TargetType.Air,
                UnitType.Tower => TargetType.Tower,
                _ => TargetType.None
            };
        }
    }
}

