using UnityEngine;
using Neocortex.Data;

namespace Neocortex.Samples
{
    public class ChatSample : MonoBehaviour
    {
        [SerializeField] private NeocortexChatPanel chatPanel;
        [SerializeField] private NeocortexTextChatInput chatInput;
        [SerializeField] private OllamaModelDropdown modelDropdown;
        [SerializeField, TextArea] private string systemPrompt;

        private OllamaRequest request;

        void Start()
        {
            request = new OllamaRequest();
            request.OnChatResponseReceived += OnChatResponseReceived;
            request.ModelName = modelDropdown.options[0].text;
            chatInput.OnSendButtonClicked.AddListener(OnUserMessageSent);
            modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

            request.AddSystemMessage(systemPrompt);
        }

        private void OnDropdownValueChanged(int index)
        {
            request.ModelName = modelDropdown.options[index].text;
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            chatPanel.AddMessage(response.message, false);
            ApplyCustomStyling(response.message, false);
        }

        private void OnUserMessageSent(string message)
        {
            request.Send(message);
            chatPanel.AddMessage(message, true);
            ApplyCustomStyling(message, true);
        }

        private void ApplyCustomStyling(string message, bool isUser)
        {
            // Get the last added message UI element from the chat panel
            Transform chatContent = chatPanel.transform.Find("Content") ?? chatPanel.transform;
            
            if (chatContent.childCount > 0)
            {
                Transform lastMessageTransform = chatContent.GetChild(chatContent.childCount - 1);
                GameObject messageUI = lastMessageTransform.gameObject;
                
                CustomMessageUI customStyle = messageUI.GetComponent<CustomMessageUI>();
                if (customStyle == null)
                {
                    customStyle = messageUI.AddComponent<CustomMessageUI>();
                }
                customStyle.StyleMessage(message, isUser);
            }
        }
    }
}