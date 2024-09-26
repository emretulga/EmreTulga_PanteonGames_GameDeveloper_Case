using System.Collections.Generic;
using PanteonGames.Game.Runtime.View;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.Factory;
using PanteonGames.Game.Runtime.Config;

namespace PanteonGames.Game.Runtime.Controller
{
    public class UnitController
    {
        public static UnitController Instance;

        public readonly Dictionary<IUnitView, UnitModel> UnitModelsByView = new ();
        public readonly Dictionary<UnitModel, IUnitView> UnitViewsByModel = new ();

        private List<AttackerUnitModel> _attackers = new ();

        public void Initialize()
        {
            IInformationView.Instance.InformationMenuClosed += DeselectUnit;
            IProduceMenu.Instance.OnMenuButtonClicked += OnProducibleChoosen;
        }

        public void MatchViewAndModel(IUnitView unitView, UnitModel unitModel)
        {
            UnitModelsByView.Add(unitView, unitModel);
            UnitViewsByModel.Add(unitModel, unitView);
            unitView.OnPlacedUnitClick += OnPlacedUnitChoosen;
            unitView.ArrivedPosition += UpdatePositionOfUnit;
        }

        public virtual void OnPlacedUnitChoosen(IUnitView unitView)
        {
            bool isViewMatchesModel = UnitModelsByView.TryGetValue(unitView, out UnitModel matchedModel);
            if(!isViewMatchesModel) return;

            InformationController.Instance.DisplayedUnit = unitView;
            unitView.ShowInformation(matchedModel.Name, matchedModel.MaxHealth, matchedModel.Health);

            if(matchedModel is not ProducerUnitModel producerUnitModel) return;

            ProducerUnitController.Instance.TryToListProduciblesByUnit(producerUnitModel);
        }

        public void OnProducibleChoosen(string productKey)
        {
            ProducerUnitModel producerUnit = (ProducerUnitModel)UnitModelsByView[InformationController.Instance.DisplayedUnit];

            ProducerUnitController.Instance.GenerateAttackerUnitByProducerUnit(producerUnit, productKey);
        }
    
        public void UpdatePositionOfUnit(IUnitView unitView, Vector2Int position)
        {
            AttackerUnitModel matchedModel = (AttackerUnitModel)UnitModelsByView[unitView];
            GridController.Instance.UpdateUnitPosition(matchedModel, position);

            AttackerUnitController.Instance.RecalculateAttackerUnitsBehaviour(matchedModel);
            
            if(InformationController.Instance.DisplayedUnit != null)
            {
                OnPlacedUnitChoosen(InformationController.Instance.DisplayedUnit);
            }

            if(ProductionMenuModel.Instance.ChoosenProduct != null)
            {
                Vector2Int lastHoveredPosition = ProductionMenuModel.Instance.LastHoveredPosition;
                ProductionMenuController.Instance.CheckChoosenProductFitOnPosition(lastHoveredPosition.X, lastHoveredPosition.Y, true);
            }

            bool isUnitMoving = AttackerUnitController.Instance.MovingUnits.Contains(matchedModel);
            if(isUnitMoving)
            {
                Vector2Int desiredPosition = AttackerUnitController.Instance.MovingUnitsDesiredPositions[matchedModel];

                bool isUnitArrivedDesiredPosition = desiredPosition.X == matchedModel.Position.X && desiredPosition.Y == matchedModel.Position.Y;
                if(!isUnitArrivedDesiredPosition) return;

                AttackerUnitController.Instance.MovingUnits.Remove(matchedModel);
                AttackerUnitController.Instance.MovingUnitsDesiredPositions.Remove(matchedModel);
            }
        }

        public void DeselectUnit()
        {
            InformationController.Instance.DisplayedUnit = null;
        }

        public void DamageUnit(UnitModel unit, int damage)
        {
            unit.SetHealth(unit.Health - damage);
            IUnitView unitView = UnitViewsByModel[unit];

            if(InformationController.Instance.DisplayedUnit == unitView)
            {
                unitView.ShowInformation(unit.Name, unit.MaxHealth, unit.Health);
            }

            if(unit.Health > 0) return;

            RemoveUnit(unit);
        }

        public void RemoveUnit(UnitModel unit)
        {
            IUnitView unitView = UnitViewsByModel[unit];
            GridController.Instance.RemoveUnit(unit);
            UnitModelsByView.Remove(unitView);
            UnitViewsByModel.Remove(unit);

            
            foreach(KeyValuePair<AttackerUnitModel, UnitModel> attackerToAttacked in AttackerUnitController.Instance.UnderAttackUnitsMap)
            {
                if(attackerToAttacked.Value == unit)
                {
                    _attackers.Add(attackerToAttacked.Key);
                }
            }
            
            foreach(AttackerUnitModel attacker in _attackers)
            {
                AttackerUnitController.Instance.StopAttacking(attacker);
            }
            _attackers.Clear();

            if(unit is AttackerUnitModel)
            {
                AttackerUnitModel attacker = (AttackerUnitModel)unit;
                AttackerUnitController.Instance.MovingUnits.Remove(attacker);
                AttackerUnitController.Instance.MovingUnitsDesiredPositions.Remove(attacker);
                AttackerUnitController.Instance.AttackerUnits.Remove(attacker);
                AttackerUnitController.Instance.UnderAttackUnitsMap.Remove(attacker);
            }

            if(InformationController.Instance.DisplayedUnit == unitView)
            {
                InformationController.Instance.DisplayedUnit = null;
                IInformationView.Instance.Close();
            }

            unitView.Remove();

            AttackerUnitController.Instance.RecalculateAttackerUnitsBehaviour();
        }

    }
}