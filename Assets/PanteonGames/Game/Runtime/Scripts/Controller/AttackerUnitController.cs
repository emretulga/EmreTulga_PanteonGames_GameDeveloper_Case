using System.Collections.Generic;
using System.Threading.Tasks;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.View;

namespace PanteonGames.Game.Runtime.Controller
{
    public class AttackerUnitController
    {
        public static AttackerUnitController Instance;

        public readonly Dictionary<AttackerUnitModel, Vector2Int> MovingUnitsDesiredPositions = new ();
        public readonly Dictionary<AttackerUnitModel, UnitModel> UnderAttackUnitsMap = new ();

        public readonly List<AttackerUnitModel> MovingUnits = new ();
        public readonly List<AttackerUnitModel> AttackerUnits = new ();
        private readonly List<AttackerUnitModel> _recalculatingUnitsBehaviour = new ();

        private List<AttackerUnitModel> _attackingUnits = new ();

        private bool _isRecalculatingBehaviour = false;

        public async Task InitializeUpdateLoop()
        {
            await Update();
        }

        private async Task Update()// Runs every for one second. It's just created for attacking
        {
            while(true)
            {
                foreach(AttackerUnitModel attacker in AttackerUnits)
                {
                    UnitModel underAttackUnit = UnderAttackUnitsMap[attacker];

                    if(!GridController.Instance.AreUnitsAroundEachOther(attacker, underAttackUnit)) continue;
                    
                    _attackingUnits.Add(attacker);
                }

                foreach(AttackerUnitModel attacker in _attackingUnits)
                {
                    if(attacker == null) continue;

                    if(!UnderAttackUnitsMap.ContainsKey(attacker)) continue;

                    UnitModel underAttackUnit = UnderAttackUnitsMap[attacker];

                    if(underAttackUnit == null) continue;

                    UnitController.Instance.DamageUnit(underAttackUnit, attacker.AttackDamage);
                }

                _attackingUnits.Clear();

                await Task.Delay(1000);
            }
        }

        public void OnRequestMovement(Vector2Int gridPosition)
        {
            bool isAnyUnitDisplayed = InformationController.Instance.DisplayedUnit != null;
            if(!isAnyUnitDisplayed) return;

            TryToMoveUnitOnPosition(InformationController.Instance.DisplayedUnit, gridPosition);
        }
        
        public void StopMovement(AttackerUnitModel attackerUnit)
        {
            MovingUnits.Remove(attackerUnit);
            MovingUnitsDesiredPositions.Remove(attackerUnit);

            UnitController.Instance.UnitViewsByModel[attackerUnit].StopProceedingToDestination();
        }

        public void TryToMoveUnitOnPosition(IUnitView unitView, Vector2Int desiredPosition)
        {
            UnitModel matchedUnit = UnitController.Instance.UnitModelsByView[unitView];

            if(matchedUnit == null) return;
            if(matchedUnit is not AttackerUnitModel) return;

            AttackerUnitModel attackerUnit = (AttackerUnitModel)matchedUnit;

            StopAttacking(attackerUnit);
            StopMovement(attackerUnit);

            Vector2Int avaiblePosition;
            int pathPoint;
            Stack<Vector2Int> destination = PathFindingController.Instance.FindDestination(matchedUnit.Position, desiredPosition, out avaiblePosition, out pathPoint);

            if(destination == null) return;

            MovingUnits.Add(attackerUnit);
            MovingUnitsDesiredPositions.Add(attackerUnit, desiredPosition);

            unitView.ProceedToDestination(destination);
        }

        public void RecalculateAttackerUnitsBehaviour(AttackerUnitModel unitOutOfRecalculating = null)
        {
            if(_isRecalculatingBehaviour) return;

            _isRecalculatingBehaviour = true;

            foreach(AttackerUnitModel unit in MovingUnits)
            {
                if(unit == unitOutOfRecalculating) continue;

                _recalculatingUnitsBehaviour.Add(unit);
            }

            foreach(AttackerUnitModel unit in _recalculatingUnitsBehaviour)
            {
                TryToMoveUnitOnPosition(UnitController.Instance.UnitViewsByModel[unit], MovingUnitsDesiredPositions[unit]);
            }

            _recalculatingUnitsBehaviour.Clear();

            foreach(AttackerUnitModel unit in AttackerUnits)
            {
                if(unit == unitOutOfRecalculating) continue;

                _recalculatingUnitsBehaviour.Add(unit);
            }

            foreach(AttackerUnitModel unit in _recalculatingUnitsBehaviour)
            {
                StopMovement(unit);
                TryToReachTarget(unit);
            }

            _recalculatingUnitsBehaviour.Clear();
            _isRecalculatingBehaviour = false;
        }

        public void StartAttackToUnit(IUnitView underAttackUnitView)
        {
            IUnitView displayedUnitView = InformationController.Instance.DisplayedUnit;
            if(displayedUnitView == null) return;

            UnitModel displayedUnit = UnitController.Instance.UnitModelsByView[displayedUnitView];
            if(displayedUnit is not AttackerUnitModel) return;

            AttackerUnitModel attackerUnit = (AttackerUnitModel)displayedUnit;
            UnitModel underAttackUnit = UnitController.Instance.UnitModelsByView[underAttackUnitView];

            if(attackerUnit == underAttackUnit) return;

            StopAttacking(attackerUnit);
            StopMovement(attackerUnit);
            AttackerUnits.Add(attackerUnit);
            UnderAttackUnitsMap.Add(attackerUnit, underAttackUnit);

            TryToReachTarget(attackerUnit);
        }

        public void TryToReachTarget(AttackerUnitModel attackerUnit)
        {
            if(!AttackerUnits.Contains(attackerUnit)) return;

            UnitModel underAttackUnit = UnderAttackUnitsMap[attackerUnit];
            if(GridController.Instance.AreUnitsAroundEachOther(attackerUnit, underAttackUnit)) return;

            List<Vector2Int> emptyGridsAroundUnderAttackedUnit = GridController.Instance.GetEmptyGridsAroundUnit(underAttackUnit);
            Vector2Int mapSize = MapModel.Instance.MapSize;
            int lowestPathPoint = mapSize.X * mapSize.Y * 14;
            Vector2Int nearestPointToUnderAttackUnit = new();
            bool isAnyWayFound = false;

            foreach(Vector2Int positionAroundUnderAttackEnemy in emptyGridsAroundUnderAttackedUnit)
            {
                Vector2Int avaiblePosition;
                int pathPoint;
                Stack<Vector2Int> destination = PathFindingController.Instance.FindDestination(attackerUnit.Position, positionAroundUnderAttackEnemy, out avaiblePosition, out pathPoint);

                bool isXPositionSame = positionAroundUnderAttackEnemy.X == avaiblePosition.X;
                bool isYPositionSame = positionAroundUnderAttackEnemy.Y == avaiblePosition.Y;

                bool isPositionSame = isXPositionSame && isYPositionSame;
                if(!isPositionSame) continue;

                if(!isAnyWayFound)
                {
                    lowestPathPoint = pathPoint;
                    nearestPointToUnderAttackUnit = positionAroundUnderAttackEnemy;
                    isAnyWayFound = true;
                    continue;
                }

                if(pathPoint >= lowestPathPoint) continue;

                lowestPathPoint = pathPoint;
                nearestPointToUnderAttackUnit = positionAroundUnderAttackEnemy;
            }

            if(!isAnyWayFound) return;

            Vector2Int lastAvaiblePosition;
            int lastPathPoint;
            Stack<Vector2Int> choosenDestination = PathFindingController.Instance.FindDestination(attackerUnit.Position, nearestPointToUnderAttackUnit, out lastAvaiblePosition, out lastPathPoint);
            
            IUnitView attackerView = UnitController.Instance.UnitViewsByModel[attackerUnit];
            attackerView.ProceedToDestination(choosenDestination);
        }

        public void StopAttacking(AttackerUnitModel attackerUnit)
        {
            AttackerUnits.Remove(attackerUnit);
            UnderAttackUnitsMap.Remove(attackerUnit);
            UnitController.Instance.UnitViewsByModel[attackerUnit].StopProceedingToDestination();
        }
    }
}