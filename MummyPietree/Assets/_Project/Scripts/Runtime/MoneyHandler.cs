using Etienne;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MummyPietree
{
    public class MoneyHandler : Singleton<MoneyHandler>
    {
        [SerializeField] private TMPro.TextMeshProUGUI moneyDisplay;
        [SerializeField] private TMPro.TextMeshProUGUI dayGamOverDiaplay;
        [SerializeField] private TMPro.TextMeshProUGUI dayLeftDiaplay, rentAmoutDisplay, worthDisplay, totalDisplay;
        [SerializeField] private int rentAmout = 2500;
        [SerializeField] private int rentInterval = 7;
        [SerializeField] private int startingMoney = 1400;
        [SerializeField, ReadOnly] private int currentMoney;
        [SerializeField] private SeedPot seedPot;
        [SerializeField] private GameObject defeatObject;
        private int dayLeft = 0;
        private int days = 0;

        protected override void Awake()
        {
            base.Awake();
            defeatObject.SetActive(false);
        }

        private void Start()
        {
            SetMoney(startingMoney);
            TimeDisplay.Instance.OnEndOfDay += EndOfDay;
            gameObject.SetActive(false);
            dayLeft = rentInterval;
        }

        private void EndOfDay()
        {
            gameObject.SetActive(true);
            TimeDisplay.Instance.ScaleTime(0);
            Time.timeScale = 0;
            dayLeft--;
            days++;
            dayGamOverDiaplay.text = days.ToString();
            dayLeftDiaplay.text = dayLeft.ToString();
            int sellingPrice = seedPot.CalculateSellingPrice();
            if (dayLeft < 1)
            {
                sellingPrice -= rentAmout;
                dayLeft = rentInterval;
                rentAmoutDisplay.text = $"{rentAmout} $";
            }
            else
            {
                rentAmoutDisplay.text = $"{0} $ ({rentAmout} when due.)";
            }
            SetSellingPrice(sellingPrice);
            totalDisplay.text = $"{sellingPrice} $";
            AddMoney(sellingPrice);
        }

        public void RestartDay()
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            TimeDisplay.Instance.ResetScaledTime();
        }

        private void SetSellingPrice(int sellingPrice)
        {
            worthDisplay.text = $"{sellingPrice} $";
        }

        private void SetMoney(int amount)
        {
            currentMoney = amount;
            moneyDisplay.text = $"{amount} $";
            if (currentMoney <= 0)
            {
                Defeat();
            }
        }

        private void Defeat()
        {
            defeatObject.SetActive(true);
        }

        public void AddMoney(int amount)
        {
            SetMoney(currentMoney + amount);
        }
        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
