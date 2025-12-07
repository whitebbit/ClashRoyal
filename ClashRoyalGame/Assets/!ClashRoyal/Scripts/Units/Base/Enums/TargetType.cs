using System;

namespace _ClashRoyal.Scripts.Units.Base.Enums
{
    [Flags]
    public enum TargetType
    {
        None = 0,
        Ground = 1 << 0,  // Может атаковать наземных
        Air = 1 << 1,     // Может атаковать воздушных
        Tower = 1 << 2    // Может атаковать башни
    }
}

