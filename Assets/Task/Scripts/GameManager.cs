using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup layoutGroup;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioClip pickCardSound,mismatchSound,matchSound;
    [SerializeField] private AudioSource audioSource;
    private int MatchesInRow,Turns,Matches=0;
    private Card CurrentCard;
    
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
    }

    public void PlayerPickedCard(Card card)
    {
     if(CurrentCard && card.cardInfo.cardID==CurrentCard.cardInfo.cardID) //matched two cards
     {
      CardMatch(card);
     }
     else if(CurrentCard) // picked wrong second card
     {
      CardMismatch(card);
     }
     else //picked first card
     {
      CurrentCard=card;
      CurrentCard.ShowCardImage();
      if(audioSource&&pickCardSound)
        {
        audioSource.clip=pickCardSound;
        audioSource.Play();
        }
     }
     //update scores
     if(uiManager)
     uiManager.UpdateScoring(MatchesInRow,Matches,Turns);
    }

    public void ClearSaved()
    {
      MatchesInRow=0;
      Turns=0;
      Matches=0;
    }

    private void CardMatch(Card card)
    {
        //audio
        if(audioSource&&matchSound)
        {
        audioSource.clip=matchSound;
        audioSource.Play();
        }
     //visuals
        card.ShowCardImage();
      StartCoroutine(uiManager.CardMatch(CurrentCard.cardInfo.cardPosition,card.cardInfo.cardPosition,0.5f));
      //scores
      Matches++;
      Turns++;
      MatchesInRow++;

      if(MatchesInRow%3==0 && uiManager) // inRow reward for every 3 matches in a row
      uiManager.AddInRowReward();

      CurrentCard=null;
    }
    private void CardMismatch(Card card)
    {
        if(audioSource&&mismatchSound)//audio
        {
        audioSource.clip=mismatchSound;
        audioSource.Play();
        }
     //visuals
        card.ShowCardImage();
      StartCoroutine(CurrentCard.HideCardImage(0.3f));
      StartCoroutine(card.HideCardImage(0.3f));
      //scores
      CurrentCard=null;
      MatchesInRow=0;
      Turns++;
    }

    public void LevelFinish()
    {
      Turns=0;
      Matches=0;
      uiManager.UpdateScoring(MatchesInRow,Matches,Turns);
    }
}


