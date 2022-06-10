using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerConversant>();
            nextButton.onClick.AddListener(Next);

            UpdateUI();
        }

        private void Next()
        {
            playerConversant.Next();
            UpdateUI();
        }

        private void UpdateUI()
        {
            AIText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext()); // button "Next" not active
        }

        private void OnDestroy()
        {
            nextButton.onClick.RemoveListener(Next);
        }
    }
}
