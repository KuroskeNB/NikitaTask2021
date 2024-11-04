using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    public CardInfo cardInfo;
    public Image _image;
    private bool bIsHidden=true;
    public Button _buttton;
    
    [SerializeField]private GameManager gameManager;
    private void Start() {
        _image=GetComponent<Image>();
        _buttton=GetComponent<Button>();
        if(_buttton)
        _buttton.onClick.AddListener(OnCardClicked);

        if(!gameManager)
        gameManager=GameObject.FindAnyObjectByType<GameManager>();
    }
    public void ShowCardImage()
    {
       _image=GetComponent<Image>();
       _buttton=GetComponent<Button>();
      if(_image)
      {
        _buttton.enabled=false;
        _image.sprite=cardInfo.cardImage;
       Debug.Log("show card");
        bIsHidden=false;
      }
    }
    public IEnumerator HideCardImage(float delay) // hide card
    {
      yield return new WaitForSeconds(delay);
      if(_image)
      {
        _buttton.enabled=true;
        _image.sprite=cardInfo.cardBackImage;
        bIsHidden=true;
      }
    }
    private void OnCardClicked()
    {
     if(bIsHidden)
     {
       ShowCardImage();
     }
    if(gameManager)
    {
        gameManager.PlayerPickedCard(this);
    }
    }

    public void VanishCard()
    {
        if(_image)
      {
        //_image.sprite=cardInfo.cardSpaceImage;
        _buttton.enabled=false;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0); // set alpha to 0
      }
    }
}
