using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class Popup : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image mask;
        public CanvasGroup panel;
        public float targetAlpha = 0.7f;

        [HideInInspector] public bool isAllowClick = true;
        [SerializeField] private bool closeWhenClickMask;


        public void Reset()
        {
            var maskColor = mask.color;
            maskColor.a = 0;
            mask.color = maskColor;
            panel.alpha = 0f;
            panel.transform.localScale = Vector3.zero;
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = Vector3.zero;
            rectTransform.offsetMax = new Vector2();
            rectTransform.offsetMin = new Vector2();
        }

        public virtual void Start()
        {
            if (closeWhenClickMask && mask != null)
                mask.gameObject.GetComponent<Button>().onClick.AddListener(OnClickHide);
        }

        protected virtual void OnDestroy()
        {
            if (mask != null && mask != null)
                mask.gameObject.GetComponent<Button>().onClick.RemoveListener(OnClickHide);
        }


        public virtual void OnShow()
        {
            if (!isAllowClick)
                return;

            isAllowClick = false;
            Reset();
            transform.SetAsLastSibling();
            panel.alpha = 1f;
            panel.transform.DOScale(Vector3.one, 0.4f)
                .SetEase(Ease.OutBack)
                .SetUpdate(UpdateType.Normal, true).OnComplete(() => { isAllowClick = true; });
            mask.DOFade(targetAlpha, 0.4f).SetUpdate(UpdateType.Normal, true);
        }

        public virtual void OnClickHide()
        {
            if (!isAllowClick)
                return;

            // AudioManager.Instance.PlaySound(SOUND_INDEX.BUTTON);
            OnHide(null);
        }

        public void OnHide(Action callBack)
        {
            isAllowClick = false;
            var sequence = DOTween.Sequence();
            sequence.Insert(0,
                panel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(UpdateType.Normal, true));
            sequence.Insert(0, mask.DOFade(0f, 0.3f).SetUpdate(UpdateType.Normal, true));
            sequence.OnComplete(() =>
            {
                isAllowClick = true;
                gameObject.SetActive(false);
                if (callBack != null) callBack();
            });
        }

        public virtual void OnFadeIn()
        {
            if (!isAllowClick)
                return;
            isAllowClick = false;
            Reset();
            panel.transform.localScale = Vector3.one * 1.5f;
            panel.transform.DOScale(Vector3.one, 0.3f)
                .SetEase(Ease.OutSine)
                .SetUpdate(UpdateType.Normal, true).OnComplete(() => { isAllowClick = true; });
            panel.DOFade(1f, 0.3f).SetUpdate(UpdateType.Normal, true);
            mask.DOFade(targetAlpha, 0.3f).SetUpdate(UpdateType.Normal, true);
        }

        public void OnFadeOut(Action callBack)
        {
            isAllowClick = false;
            var sequence = DOTween.Sequence();
            sequence.Insert(0,
                panel.transform.DOScale(Vector3.one * 1.5f, 0.3f).SetEase(Ease.OutSine)
                    .SetUpdate(UpdateType.Normal, true));
            sequence.Insert(0, panel.DOFade(0f, 0.3f).SetUpdate(UpdateType.Normal, true));
            sequence.Insert(0, mask.DOFade(0f, 0.3f).SetUpdate(UpdateType.Normal, true));
            sequence.OnComplete(() =>
            {
                isAllowClick = true;
                gameObject.SetActive(false);
                callBack();
            });
        }

        public void InstantShowHide(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}