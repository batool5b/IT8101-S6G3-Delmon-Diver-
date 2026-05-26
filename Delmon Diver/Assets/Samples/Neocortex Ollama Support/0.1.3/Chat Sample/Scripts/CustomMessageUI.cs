using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Neocortex.Samples
{
    public class CustomMessageUI : MonoBehaviour
    {
        // USER = Dark Brown text
        [SerializeField]
        private Color userMessageColor =
            new Color(0.478f, 0.306f, 0.157f); // #7A4E28

        // AGENT = Parchment text
        [SerializeField]
        private Color agentMessageColor =
            new Color(0.910f, 0.835f, 0.710f); // #E8D5B5

        [Header("Text Settings")]
        [SerializeField]
        private int textSize = 60; //message text size

        [Header("Writing Indicator Settings")]
        [SerializeField]
        private RectTransform writingIndicator; //drag Writing Indicator here

        [SerializeField]
        private float writingIndicatorHeight = 90f; //make writing indicator taller

        private Text textComponent;
        private TextMeshProUGUI tmpTextComponent;
        private LayoutElement layoutElement;
        private HorizontalLayoutGroup horizontalLayoutGroup;

        private void Awake()
        {
            //get text components
            textComponent = GetComponentInChildren<Text>(true);
            tmpTextComponent = GetComponentInChildren<TextMeshProUGUI>(true);

            //get layout components
            layoutElement = GetComponent<LayoutElement>();
            horizontalLayoutGroup = transform.parent?.GetComponent<HorizontalLayoutGroup>();

            //resize writing indicator if assigned
            ResizeWritingIndicator();
        }

        /// <summary>
        /// Style the message based on sender
        /// </summary>
        public void StyleMessage(string message, bool isUser)
        {
            //add sender prefix
            string displayText = isUser
                ? "[AlHaetham] " + message
                : "[Balbol] " + message;

            //apply to legacy Text
            if (textComponent != null)
            {
                textComponent.text = displayText;

                textComponent.color = isUser
                    ? userMessageColor
                    : agentMessageColor;

                textComponent.fontSize = textSize;
            }

            //apply to TextMeshPro
            if (tmpTextComponent != null)
            {
                tmpTextComponent.text = displayText;

                tmpTextComponent.color = isUser
                    ? userMessageColor
                    : agentMessageColor;

                //text size
                tmpTextComponent.fontSize = textSize;

                //force alignment
                tmpTextComponent.alignment = TextAlignmentOptions.TopLeft;

                //better readability
                tmpTextComponent.enableWordWrapping = true;
            }

            //align layout left
            if (horizontalLayoutGroup != null)
            {
                horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            }

            //flexible width
            if (layoutElement != null)
            {
                layoutElement.flexibleWidth = 1;

                //helps the big text not get squeezed
                layoutElement.minHeight = writingIndicatorHeight;
                layoutElement.preferredHeight = writingIndicatorHeight;
            }

            ResizeWritingIndicator();
        }

        private void ResizeWritingIndicator()
        {
            if (writingIndicator == null) return;

            Vector2 size = writingIndicator.sizeDelta;
            size.y = writingIndicatorHeight;
            writingIndicator.sizeDelta = size;

            LayoutElement indicatorLayout = writingIndicator.GetComponent<LayoutElement>();

            if (indicatorLayout != null)
            {
                indicatorLayout.minHeight = writingIndicatorHeight;
                indicatorLayout.preferredHeight = writingIndicatorHeight;
            }
        }
    }
}