using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pet.MainMenu
{
    public class MainMenuTester : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterTMP;
        [SerializeField] private Button increaseButton; 
        [SerializeField] private Button decreaseButton; 
        [SerializeField] private Button resetButton;
        
        private int counter;
        

        private void Start()
        {
            increaseButton.onClick.AddListener(IncrementCounter);
            decreaseButton.onClick.AddListener(DecreaseCounter);
            resetButton.onClick.AddListener(ResetCounter);
        }

        private void OnDestroy()
        {
            increaseButton.onClick.RemoveAllListeners();
            decreaseButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
        }

        private void IncrementCounter()
        {
            counter++;
            counterTMP.text = counter.ToString();
        }

        private void DecreaseCounter()
        {
            counter--;
            counterTMP.text = counter.ToString();
        }

        private void ResetCounter()
        {
            counter = 0;
            counterTMP.text = counter.ToString();
        }
    }
}
