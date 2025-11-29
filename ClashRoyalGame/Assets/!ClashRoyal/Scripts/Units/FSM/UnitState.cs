namespace _ClashRoyal.Scripts.Units.FSM
{
    [UnityEngine.CreateAssetMenu(fileName = "UnitState", menuName = "Configs/Units/FSM/State", order = 0)]
    public abstract class UnitState : UnityEngine.ScriptableObject
    {
        public abstract void OnUpdate();
        
    }
}