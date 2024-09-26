using PanteonGames.Game.Runtime.View;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.Factory;

namespace PanteonGames.Game.Runtime.Controller
{
    public class ProductionMenuController
    {
        public static ProductionMenuController Instance;

        public IProductViewManager ProductView;

        public void Initialize(IProductViewManager productView)
        {
            ProductView = productView;
            ProductView.OnProductChoosen += ChooseUnit;
            ProductView.OnProductChecksIsFitOnPosition += CheckChoosenProductFitOnPosition;
            ProductView.OnProductTryToPlaceOnPosition += TryToPlaceChoosenProductOnPosition;
        }

        public void ChooseUnit(UnitType unitType, string productKey)
        {
            ProductionMenuModel.Instance.ChoosenProduct = UnitFactoryHelper.GetUnit(unitType, productKey);
        }

        public void CheckChoosenProductFitOnPosition(int x, int y, bool forceToRecalculate = false)
        {
            bool isHoveredPositionSame = ProductionMenuModel.Instance.LastHoveredPosition.X == x && ProductionMenuModel.Instance.LastHoveredPosition.Y == y;
            if(isHoveredPositionSame && !forceToRecalculate) return;
            ProductionMenuModel.Instance.LastHoveredPosition = new(x, y);

            if(x < 0 || y < 0)
            {
                GridController.Instance.SetDefaultGridsStatus();
            }
            else
            {
                bool isFit = GridController.Instance.IsSizeFitForPosition(ProductionMenuModel.Instance.ChoosenProduct.Size, new Vector2Int(x, y));
                Vector2Int coveredLastPosition = new(x + ProductionMenuModel.Instance.ChoosenProduct.Size.X, y + ProductionMenuModel.Instance.ChoosenProduct.Size.Y);
                GridController.Instance.ChangeGridsStatus(ProductionMenuModel.Instance.LastHoveredPosition, coveredLastPosition, isFit);
            }
        }

        public void TryToPlaceChoosenProductOnPosition(int x, int y)
        {
            UnitModel product = ProductionMenuModel.Instance.ChoosenProduct;

            ProductionMenuModel.Instance.ChoosenProduct = null;
            ProductionMenuModel.Instance.LastHoveredPosition.Set(-1, -1);

            Vector2Int position = new Vector2Int(x, y);
            bool isFit = GridController.Instance.IsSizeFitForPosition(product.Size, position);

            GridController.Instance.SetDefaultGridsStatus();

            if(isFit)
            {
                IUnitView unitView = ProductView.GetChoosenUnitView();
                UnitController.Instance.MatchViewAndModel(unitView, product);
                GridController.Instance.PlaceUnitOnPosition(product, position);
                ProductView.PlaceProduct(position.X, position.Y);
                
                AttackerUnitController.Instance.RecalculateAttackerUnitsBehaviour();
            }
            else
            {
                product.PoolObject();
                ProductView.RemoveProduct();
            }
        }
    }
}