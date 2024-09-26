using PanteonGames.Game.Runtime.View;

namespace PanteonGames.Game.Runtime.Controller
{
    public class InputController
    {
        public static InputController Instance;

        public InputController()
        {
            IInputView.Instance.OnRightClickedUnit += RightClickedOnUnit;
        }

        public void RightClickedOnUnit(IUnitView unitView)
        {
            AttackerUnitController.Instance.StartAttackToUnit(unitView);
        }
    }
}