using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;


[Serializable]
public class MySaveGame : SaveGame
{
//	public string playerName =  new List<string>(4);
//	public List<Color> playerColor = new List<Color>(4);
//	public List<int> numOfCityWall = new List<int>(4);
//	public List<int> vP = new List<int>(4);
//	public List<int> resourceSum = new List<int>(4);
//	public List<int> progressCardSum = new List<int>(4);
//	public List<int> defendCount = new List<int>(4);
//	public List<int> longestRoad = new List<int>(4);
//	public List<int> fcTradeLvl = new List<int>(4);
//	public List<int> fcPoliticsLvl = new List<int>(4);
//	public List<int> fcScienceLvl = new List<int>(4);
//	public List<List<ProgressCard>> progressCards = new List<List<ProgressCard>>(4);
//	public List<List<int>> resources = new List<List<int>>(4);
//	public List<List<int>> tradeRatios = new List<List<int>>(4);
//	public List<List<Village>> villages = new List<List<Village>>(4);
//	public List<List<Knight>> knights = new List<List<Knight>> (4);
//	public List<List<EdgeUnit>> roads = new List<List<EdgeUnit>>(4);
	public string playerName;
	public string playerColor;
	public int numOfCityWall;
	public int vP;
	public int resourceSum;
	public int progressCardSum;
	public int defendCount;
	public int longestRoad;
	public int fcTradeLvl;
	public int fcPoliticsLvl;
	public int fcScienceLvl;
    public List<int> tradeCardsQueue = new List<int>();
    public List<int> politicsCardsQueue = new List<int>();
    public List<int> scienceCardsQueue = new List<int>();
    public List<int> resources = new List<int>();
	public List<int> tradeRatios = new List<int>();
	public List<Text> resourceDisplay = new List<Text>(); 
	public List<Village> villages = new List<Village>();
	public List<Knight> knights = new List<Knight> ();
	public List<EdgeUnit> roads = new List<EdgeUnit>();
    public List<int> pgCard = new List<int>();
	public int numberOfPlayer;
	public int VPsToWin;
	public int barbarianPosition;
	public int knightStrength;
	public int barbarianStrength;
	public bool barbarianAttacked;
	public GamePhase currentPhase;

	public List<int> numbers = new List<int>();
	public List<int> terrains	= new List<int>();
	public List<int> harbortypes	= new List<int>();
//	public List<GameObject> landHexes = new List<GameObject>(); 
//	public List<GameObject> junctions = new List<GameObject>();
//	public List<GameObject> edges = new List<GameObject>();
//	public List<GameObject> harbours = new List<GameObject> ();
	
}