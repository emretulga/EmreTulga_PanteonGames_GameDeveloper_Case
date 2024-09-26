using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PanteonGames.Game.Runtime.View
{
    public delegate void InformationEventHandler();

    public interface IInformationView
    {
        static IInformationView Instance;
        
        event InformationEventHandler InformationMenuClosed;

        void Close();
    }

    public class InformationView : MonoBehaviour, IInformationView
    {
        public static InformationView Instance;

        public event InformationEventHandler InformationMenuClosed;

        public GameObject ProduceMenu;
        public Image UnitImage, HealthBar;
        public TextMeshProUGUI UnitNameText, HealthText;
    
        private UnitView _displayedUnit;

        private void Awake()
        {
            Instance = this;
            IInformationView.Instance = this;
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Open(UnitView unitView, Sprite unitSprite, string unitName, int maxHealth, int health)
        {
            ProduceMenu.SetActive(false);

            if(_displayedUnit != null && _displayedUnit != unitView)
            {
                _displayedUnit.DeselectUnit();
            }
            
            _displayedUnit = unitView;
            UnitImage.sprite = unitSprite;
            UnitNameText.text = unitName;
            HealthText.text = health + " / " + maxHealth;
            float healthPercentage = (float)health / maxHealth;
            HealthBar.fillAmount = healthPercentage;

            gameObject.SetActive(true);
        }

        public void Close()
        {
            if(_displayedUnit != null)
            {
                _displayedUnit.DeselectUnit();
                _displayedUnit = null;
            }

            gameObject.SetActive(false);

            InformationMenuClosed?.Invoke();
        }
    }
}