using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Underpin.Slots
{
    /// <summary>Controls the visual spin and stop animation for one reel.</summary>
    public sealed class SlotReel : MonoBehaviour
    {
        [SerializeField] private Image symbolImage;
        [SerializeField] private Sprite[] symbols;
        [SerializeField] private float startTickInterval = 0.045f;
        [SerializeField] private float endTickInterval = 0.15f;

        public int CurrentSymbolIndex { get; private set; }
        public bool IsSpinning { get; private set; }

        public void Configure(Image image, Sprite[] reelSymbols)
        {
            symbolImage = image;
            symbols = reelSymbols;
            SetSymbol(0);
        }

        public IEnumerator SpinTo(int targetIndex, float duration)
        {
            if (symbols == null || symbols.Length == 0 || symbolImage == null)
                yield break;

            IsSpinning = true;
            float elapsed = 0f;

            // The changing interval creates a convincing slow-down before the reel stops.
            while (elapsed < duration)
            {
                float normalized = Mathf.Clamp01(elapsed / duration);
                SetSymbol(Random.Range(0, symbols.Length));
                float interval = Mathf.Lerp(startTickInterval, endTickInterval, normalized * normalized);
                yield return new WaitForSeconds(interval);
                elapsed += interval;
            }

            SetSymbol(targetIndex);
            yield return BounceStop();
            IsSpinning = false;
        }

        private void SetSymbol(int index)
        {
            CurrentSymbolIndex = Mathf.Clamp(index, 0, symbols.Length - 1);
            symbolImage.sprite = symbols[CurrentSymbolIndex];
            symbolImage.preserveAspect = true;
        }

        private IEnumerator BounceStop()
        {
            if (symbolImage == null) yield break;
            RectTransform rect = symbolImage.rectTransform;
            Vector3 original = Vector3.one;
            float elapsed = 0f;
            const float duration = 0.16f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float pulse = 1f + Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI) * 0.12f;
                rect.localScale = original * pulse;
                yield return null;
            }
            rect.localScale = original;
        }
    }
}
