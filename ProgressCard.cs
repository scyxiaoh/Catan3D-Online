using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ProgressCard{

	public pgCardType myType;
	public Improvement category;		

	public Queue<ProgressCard> generateDeck(int cate){
		Queue<ProgressCard> deck = new Queue<ProgressCard> ();
		List<ProgressCard> deckbuffer = new List<ProgressCard> ();
		// 0-trade, 1-politics, 2-science
		if (cate == 0) {
			deckbuffer.Add(setPg (pgCardType.Commercial_Harbor, 0));
			deckbuffer.Add(setPg (pgCardType.Commercial_Harbor, 0));
			deckbuffer.Add(setPg (pgCardType.Master_Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Master_Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant_Fleet, 0));
			deckbuffer.Add(setPg (pgCardType.Merchant_Fleet, 0));
			deckbuffer.Add(setPg (pgCardType.Resource_Monopoly, 0));
			deckbuffer.Add(setPg (pgCardType.Resource_Monopoly, 0));
			deckbuffer.Add(setPg (pgCardType.Resource_Monopoly, 0));
			deckbuffer.Add(setPg (pgCardType.Resource_Monopoly, 0));
			deckbuffer.Add(setPg (pgCardType.Trade_Monopoly, 0));
			deckbuffer.Add(setPg (pgCardType.Trade_Monopoly, 0));
		} else if (cate == 1) {
			deckbuffer.Add(setPg (pgCardType.Bishop, 1));
			deckbuffer.Add(setPg (pgCardType.Bishop, 1));
			deckbuffer.Add(setPg (pgCardType.Constitution, 1));
			deckbuffer.Add(setPg (pgCardType.Deserter, 1));
			deckbuffer.Add(setPg (pgCardType.Deserter, 1));
			deckbuffer.Add(setPg (pgCardType.Diplomat, 1));
			deckbuffer.Add(setPg (pgCardType.Diplomat, 1));
			deckbuffer.Add(setPg (pgCardType.Intrigue, 1));
			deckbuffer.Add(setPg (pgCardType.Intrigue, 1));
			deckbuffer.Add(setPg (pgCardType.Saboteur, 1));
			deckbuffer.Add(setPg (pgCardType.Saboteur, 1));
			deckbuffer.Add(setPg (pgCardType.Spy, 1));
			deckbuffer.Add(setPg (pgCardType.Spy, 1));
			deckbuffer.Add(setPg (pgCardType.Spy, 1));
			deckbuffer.Add(setPg (pgCardType.Warlord, 1));
			deckbuffer.Add(setPg (pgCardType.Warlord, 1));
			deckbuffer.Add(setPg (pgCardType.Wedding, 1));
			deckbuffer.Add(setPg (pgCardType.Wedding, 1));
		} else if (cate == 2) {
			deckbuffer.Add(setPg(pgCardType.Alchemist,2));
			deckbuffer.Add(setPg(pgCardType.Alchemist,2));
			deckbuffer.Add(setPg(pgCardType.Crane,2));
			deckbuffer.Add(setPg(pgCardType.Crane,2));
			deckbuffer.Add(setPg(pgCardType.Engineer,2));
			deckbuffer.Add(setPg(pgCardType.Inventor,2));
			deckbuffer.Add(setPg(pgCardType.Inventor,2));
			deckbuffer.Add(setPg(pgCardType.Irrigation,2));
			deckbuffer.Add(setPg(pgCardType.Irrigation,2));
			deckbuffer.Add(setPg(pgCardType.Medicine,2));
			deckbuffer.Add(setPg(pgCardType.Medicine,2));
			deckbuffer.Add(setPg(pgCardType.Mining,2));
			deckbuffer.Add(setPg(pgCardType.Mining,2));
			deckbuffer.Add(setPg(pgCardType.Printer,2));
			deckbuffer.Add(setPg(pgCardType.Road_Building,2));
			deckbuffer.Add(setPg(pgCardType.Road_Building,2));
			deckbuffer.Add(setPg(pgCardType.Smith,2));
			deckbuffer.Add(setPg(pgCardType.Smith,2));
		}
		shuffle (deckbuffer);
		foreach (ProgressCard c in deckbuffer) {
			deck.Enqueue (c);
		}
			
		return deck;
	
	}
		
	public ProgressCard setPg(pgCardType type,int cate){
		ProgressCard newCard = new ProgressCard();
		newCard.myType = type;
		newCard.category = (Improvement)Enum.Parse(typeof(Improvement),cate.ToString());
		return newCard;
	}

	public void shuffle (List<ProgressCard> alpha){
		for (int i = 0; i < alpha.Count; i++)
		{
			ProgressCard temp = alpha[i];
			int randomIndex = UnityEngine.Random.Range(i, alpha.Count);
			alpha[i] = alpha[randomIndex];
			alpha[randomIndex] = temp;
		}
	}

	bool CheckDeck(Queue<ProgressCard> Deck)
	{
		if (Deck.Count > 0)
		{
			return true;
		}
		return false;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
