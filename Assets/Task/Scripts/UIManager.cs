using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField]private Card[] cardTypes;
    private Card[] spawnedCards;
    [SerializeField] private AudioSource audioSource;
    public Card SpaceCard;
    [SerializeField] private AudioClip gameEndSounds;
    [SerializeField] private Vector2[] layouts;
    [SerializeField] Text matchesInRowTxt,matchesTxt,turnsTxt,inRowRewardTxt,levelFinishedTxt,startNextTxt;
     private int InRowRewards,levelsFinished=0;
    private int cardCount=0;
     [SerializeField] private GridLayoutGroup layoutGroup;
     private bool bLevelOnScreen,bCardsShown=false;
     public Button StartNextButton,inRowRewardBtn,ClearSaves;
     [SerializeField] private float StartDelay=1.5f;
      [SerializeField]private GameManager gameManager;

    // Start is called before the first frame update
    void Start() // standard property load and button listeners
    {
        if(StartNextButton)
        StartNextButton.onClick.AddListener(StartNextClicked);
        if(inRowRewardBtn)
        inRowRewardBtn.onClick.AddListener(UseInRowReward);
        if(ClearSaves)
        ClearSaves.onClick.AddListener(ClearProgress);
        spawnedCards = new Card[0];
        
        audioSource=GetComponent<AudioSource>();
        if(!gameManager)
        gameManager=GameObject.FindAnyObjectByType<GameManager>();

        //load saves
        InRowRewards=SaveLoadManager.Instance.LoadInRowRewards();
        levelsFinished=SaveLoadManager.Instance.LoadFinishedLevels();
        if(inRowRewardTxt)
        inRowRewardTxt.text=InRowRewards.ToString();
        if(levelFinishedTxt)
     levelFinishedTxt.text=levelsFinished.ToString();
    }
    
    private void ClearProgress() // start from the begining
    {
      SaveLoadManager.Instance.ClearSaves();

      InRowRewards=0;
      levelsFinished=0;
      if(gameManager)
      gameManager.ClearSaved();
      
       if(inRowRewardTxt)
        inRowRewardTxt.text=InRowRewards.ToString();
        if(levelFinishedTxt)
     levelFinishedTxt.text=levelsFinished.ToString();

     if(bLevelOnScreen)
     LevelClear();
    }

    // scoring bonuses management
    public void AddInRowReward() // gaining of combo reward
    {
     InRowRewards++;
     if(inRowRewardTxt)
     inRowRewardTxt.text=InRowRewards.ToString();
    }
    private void UseInRowReward() // show all cards as a reward for a combo of matches as at start of the level
    {
      if(InRowRewards>0 && bLevelOnScreen)
      {
        InRowRewards--;
        if(inRowRewardTxt)
        inRowRewardTxt.text=InRowRewards.ToString();
      StartTheGame(); // show cards for startDelay
      }
    }
    
    public void UpdateScoring(int MatchesInRow,int Matches, int Turns)
    {
      Debug.Log("update scoring");
     if(matchesInRowTxt)
     matchesInRowTxt.text=MatchesInRow.ToString();
     if(matchesTxt)
     matchesTxt.text=Matches.ToString();
     if(turnsTxt)
     turnsTxt.text=Turns.ToString();
    }

  // level and card management
    void StartNextClicked()
    {
      if(levelsFinished<layouts.Length)
      SpawnLevelCards(layouts[levelsFinished]);
    }
    
    private List<Card> ShuffleCards(int rows, int columns)
    {
      List<Card> cardList = new List<Card>();
      int CardCountInDeck = rows*columns;
      if(CardCountInDeck%2!=0)
      CardCountInDeck-=1;
      int num=0;
        foreach (var item in cardTypes)
        {
          for (int i = 0; i < 2; i++)
          {
            Card tempCard;
            tempCard = Instantiate(item);
            cardList.Add(tempCard);
            num++;
          }
          if(num==CardCountInDeck)
          break;
        }
        Debug.Log("list count is"+cardList.Count);
        return cardList;
    }

    public void SpawnLevelCards(Vector2 gameLayout) // level prepearing
    {
      if(bLevelOnScreen) return;

        int rows=(int)gameLayout.x; 
        int columns=(int)gameLayout.y;
        bool bIsEven = (rows*columns)%2==0;
        SetupTheLevel(rows,columns);
 
        cardCount=spawnedCards.Length;
        if(!bIsEven)
        cardCount-=1;
        bLevelOnScreen=true;
        StartTheGame();
    }

    void StartTheGame()
    {
      if(startNextTxt)
      startNextTxt.enabled=false;
      Debug.Log("start the game");
      foreach (var item in spawnedCards) // show all cards for a start
      {
        item.ShowCardImage();
      }
      foreach (var item in spawnedCards) // hide all cards for a game
      {
        StartCoroutine(item.HideCardImage(StartDelay));
      }
    }

    private void SetupTheLevel(int rows, int columns)
    {
      bool bIsEven = (rows*columns)%2==0;

      int CardCountRequire = rows*columns;
        List<Card> cardList = ShuffleCards(rows,columns);
        int emptyCardNum=-1;
        if(!bIsEven)
        {
            emptyCardNum=(rows*columns)/2;
        }
        layoutGroup.cellSize=CalculateCardSize(rows,columns);

        for (int i = 0; i < CardCountRequire; i++)   
        {
            if(i==emptyCardNum) // if there is not even number of row*columns
            {
              Card temp;
          temp = Instantiate(SpaceCard);
          temp.transform.SetParent(layoutGroup.transform,false); // add empty space card
          System.Array.Resize(ref spawnedCards,spawnedCards.Length+1);
          spawnedCards[spawnedCards.Length - 1] = temp;
          continue;
            }

          System.Array.Resize(ref spawnedCards,spawnedCards.Length+1);
          if(cardList.Count<1) break;
          int randomCard =Random.Range(0,cardList.Count-1); // random shuffle card from a spawned cards
          spawnedCards[spawnedCards.Length - 1] = cardList[randomCard];
          cardList[randomCard].transform.SetParent(layoutGroup.transform,false);
          cardList[randomCard].cardInfo.cardPosition=i;

          cardList.RemoveAt(randomCard); //card was added to screen and we do not need it again
        }
    }

    public IEnumerator CardMatch(int first,int second, float delay)
    {
      yield return new WaitForSeconds(delay);
      //vanish cards from screen and deck
     if(spawnedCards[first])
     spawnedCards[first].VanishCard();
     if(spawnedCards[second])
     spawnedCards[second].VanishCard();
     cardCount-=2;

    if(cardCount<1)//level finish
     {
     levelsFinished++;
     if(audioSource&&audioSource.clip)
      audioSource.Play();
     LevelClear();

     if(levelFinishedTxt)
     levelFinishedTxt.text=levelsFinished.ToString();
     // save progress
      Debug.Log("try to save");
      SaveLoadManager.Instance.SaveValues(levelsFinished,InRowRewards);
     }
    }

    private void LevelClear() // clear the level and his properties
    {
      // clear the level
      foreach (var item in spawnedCards)
      {
        Destroy(item.gameObject);
      }
      spawnedCards=new Card[0];
      bLevelOnScreen=false;
      if(startNextTxt)
      startNextTxt.enabled=true;
      if(gameManager)
      gameManager.LevelFinish();
    }

    private Vector2 CalculateCardSize(int rows, int columns)
    {
        RectTransform rectTransform = layoutGroup.GetComponent<RectTransform>();
        float containerWidth = rectTransform.rect.width;
        float containerHeight = rectTransform.rect.height;

        float cellWidth = (containerWidth - layoutGroup.padding.left - layoutGroup.padding.right - (columns - 1) * layoutGroup.spacing.x) / columns;
        float cellHeight = (containerHeight - layoutGroup.padding.top - layoutGroup.padding.bottom - (rows - 1) * layoutGroup.spacing.y) / rows;

        Vector2 returnsize = new Vector2(cellWidth, cellHeight);

        return returnsize;
    }
}
