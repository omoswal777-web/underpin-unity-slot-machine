using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Underpin.Slots
{
    /// <summary>Owns slot outcomes, balance, bets, payouts and game UI.</summary>
    public sealed class SlotGameManager : MonoBehaviour
    {
        [Header("Reels")]
        [SerializeField] private SlotReel[] reels = new SlotReel[3];
        [SerializeField] private Sprite[] symbols = new Sprite[4];

        [Header("UI")]
        [SerializeField] private Text balanceText;
        [SerializeField] private Text betText;
        [SerializeField] private Text resultText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button increaseBetButton;
        [SerializeField] private Button decreaseBetButton;

        [Header("Game Rules")]
        [SerializeField] private int startingBalance = 500;
        [SerializeField] private int minimumBet = 5;
        [SerializeField] private int maximumBet = 50;
        [SerializeField] private int betStep = 5;

        private int balance;
        private int currentBet;
        private bool isSpinning;

        // Payout multipliers correspond to the supplied symbols:
        // Seven = 50x, Cherries = 15x, Bell = 10x, BAR = 5x.
        private readonly int[] payoutMultipliers = { 50, 15, 10, 5 };

        private void Awake()
        {
            balance = startingBalance;
            currentBet = minimumBet;

            for (int i = 0; i < reels.Length; i++)
                reels[i].Configure(FindReelImage(i), symbols);

            if (spinButton != null) spinButton.onClick.AddListener(Spin);
            if (increaseBetButton != null) increaseBetButton.onClick.AddListener(IncreaseBet);
            if (decreaseBetButton != null) decreaseBetButton.onClick.AddListener(DecreaseBet);

            UpdateUI();
            SetResult("MATCH ALL THREE SYMBOLS TO WIN!");
        }

        // Reel image references are assigned by the scene builder using this lookup.
        private Image FindReelImage(int index)
        {
            GameObject imageObject = GameObject.Find("ReelSymbol" + (index + 1));
            return imageObject != null ? imageObject.GetComponent<Image>() : null;
        }

        public void Spin()
        {
            if (isSpinning) return;
            if (balance < currentBet)
            {
                SetResult("NOT ENOUGH CREDITS — LOWER YOUR BET");
                return;
            }
            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            isSpinning = true;
            balance -= currentBet;
            UpdateUI();
            SetResult("GOOD LUCK!");

            // Select final outcomes before animation. The visual reel animation does not
            // influence the result, keeping game logic separate from presentation.
            int[] outcomes = new int[reels.Length];
            for (int i = 0; i < outcomes.Length; i++)
                outcomes[i] = Random.Range(0, symbols.Length);

            for (int i = 0; i < reels.Length; i++)
                StartCoroutine(reels[i].SpinTo(outcomes[i], 1.15f + i * 0.48f));

            // Wait for the longest reel animation.
            yield return new WaitForSeconds(2.40f);

            if (outcomes[0] == outcomes[1] && outcomes[1] == outcomes[2])
            {
                int payout = currentBet * payoutMultipliers[outcomes[0]];
                balance += payout;
                SetResult("JACKPOT!  +" + payout + " CREDITS");
            }
            else
            {
                SetResult("NO WIN — TRY AGAIN!");
            }

            isSpinning = false;
            UpdateUI();
        }

        public void IncreaseBet()
        {
            if (isSpinning) return;
            currentBet = Mathf.Min(maximumBet, currentBet + betStep);
            UpdateUI();
        }

        public void DecreaseBet()
        {
            if (isSpinning) return;
            currentBet = Mathf.Max(minimumBet, currentBet - betStep);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (balanceText != null) balanceText.text = "CREDITS  " + balance;
            if (betText != null) betText.text = "BET  " + currentBet;
            if (spinButton != null) spinButton.interactable = !isSpinning && balance >= currentBet;
            if (increaseBetButton != null) increaseBetButton.interactable = !isSpinning && currentBet < maximumBet;
            if (decreaseBetButton != null) decreaseBetButton.interactable = !isSpinning && currentBet > minimumBet;
        }

        private void SetResult(string message)
        {
            if (resultText != null) resultText.text = message;
        }
    }
}
