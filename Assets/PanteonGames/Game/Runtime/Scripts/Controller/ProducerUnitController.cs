using System.Collections.Generic;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.View;
using PanteonGames.Game.Runtime.Factory;

namespace PanteonGames.Game.Runtime.Controller
{
    public class ProducerUnitController
    {
        public static ProducerUnitController Instance;

        private readonly List<string> _capableProducts = new ();

        public void TryToListProduciblesByUnit(ProducerUnitModel producerUnitModel)
        {
            int spawnPointX = producerUnitModel.ProductSpawnPoint.X + producerUnitModel.Position.X;
            int spawnPointY = producerUnitModel.ProductSpawnPoint.Y + producerUnitModel.Position.Y;
            Vector2Int spawnPoint = new Vector2Int(spawnPointX, spawnPointY);
            int producibleUnitsLength = producerUnitModel.ProducibleUnitKeys.Length;

            _capableProducts.Clear();

            for(int produciblesIndex = 0; produciblesIndex < producibleUnitsLength; produciblesIndex++)
            {
                string productKey = producerUnitModel.ProducibleUnitKeys[produciblesIndex];
               
                Vector2Int unitSize = IAttackerUnitFactory.Instance.GetSizeOfUnit(productKey);
                bool isCapableToProduce = GridController.Instance.IsSizeFitForPosition(unitSize, spawnPoint);
                
                if(isCapableToProduce)
                {
                    _capableProducts.Add(productKey);
                }
            }

            if(_capableProducts.Count == 0) return;

            IProduceMenu.Instance.Open(_capableProducts);
        }

        public void GenerateAttackerUnitByProducerUnit(ProducerUnitModel producerUnitModel, string productKey)
        {
            Vector2Int spawnPoint = new Vector2Int(producerUnitModel.Position.X + producerUnitModel.ProductSpawnPoint.X, producerUnitModel.Position.Y + producerUnitModel.ProductSpawnPoint.Y);
            AttackerUnitModel attackerUnit = IAttackerUnitFactory.Instance.GetUnit(productKey);

            GridController.Instance.PlaceUnitOnPosition(attackerUnit, spawnPoint);

            IUnitView soldierView = ProductionMenuController.Instance.ProductView.GetNewUnit("Soldier");

            UnitController.Instance.MatchViewAndModel(soldierView, attackerUnit);

            soldierView.PlaceOnPosition(spawnPoint.X, spawnPoint.Y);

            AttackerUnitController.Instance.RecalculateAttackerUnitsBehaviour();
        }
    }
}