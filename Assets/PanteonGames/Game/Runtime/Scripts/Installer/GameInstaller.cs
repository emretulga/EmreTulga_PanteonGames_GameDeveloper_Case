using UnityEngine;
using PanteonGames.Game.Runtime.Controller;
using PanteonGames.Game.Runtime.Model;
using PanteonGames.Game.Runtime.View;
using PanteonGames.Game.Runtime.Config;
using PanteonGames.Game.Runtime.Factory;

namespace PanteonGames.Game.Runtime.Installer
{
    public class GameInstaller : MonoBehaviour
    {
        private void Awake()
        {
            IConfigLoader.Instance = new ConfigLoader();
            IUnitFactory.Instance = new UnitFactory(65);
            IAttackerUnitFactory.Instance = new AttackerUnitFactory(410);
            IProducerUnitFactory.Instance = new ProducerUnitFactory(45);
            MapConfig mapConfig = IConfigLoader.Instance.GetMapConfig ();
            MapModel.Instance = new (mapConfig.SizeX, mapConfig.SizeY);
            GridController.Instance = new ();
            ProductionMenuModel.Instance = new ();
            ProductionMenuController.Instance = new ();
            InformationController.Instance = new ();
            UnitController.Instance = new ();
            ProducerUnitController.Instance = new ();
            AttackerUnitController.Instance = new ();
            IPathNodeFactory.Instance = new PathNodeFactory(mapConfig.SizeX * mapConfig.SizeY);
            PathFindingController.Instance = new (new Model.Vector2Int(mapConfig.SizeX, mapConfig.SizeY));
        }

        private async void Start()
        {
            UnitController.Instance.Initialize();
            GridViewManager.Instance.CreateGrids(MapModel.Instance.MapSize.X, MapModel.Instance.MapSize.Y, AttackerUnitController.Instance.OnRequestMovement);
            ProductionMenuController.Instance.Initialize(ProductionMenuView.Instance);
            GridController.Instance.GridView = GridViewManager.Instance;
            InputController.Instance = new ();

            await AttackerUnitController.Instance.InitializeUpdateLoop();
        }
    }
}