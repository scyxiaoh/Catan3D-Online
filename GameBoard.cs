using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameBoard : NetworkBehaviour {


	public GameObject vertexObj;
	public GameObject hexObj;
	public GameObject edgeObj;
	public GameObject num;
	public GameObject tile0;
	public GameObject[] landTiles = new GameObject[7];
	public GameObject villageObj;
	public GameObject roadObj;
	public GameObject harbourObj;
	public GameObject harbourIconObj;
	public GameObject knightObj;
	public GameObject robberObj;
	public GameObject merchantObj;
	public GameObject pirateObj;
	public Material[] harbourIconMaterials = new Material[6];

	public GameObject panelInfoBar;
	public GameObject panelActions;
	public GameObject panelTrade;
	public GameObject panelDice;
	public GameObject panelDiscard;
	public GameObject panelInfo;
	public GameObject panelProgressCard;
	public GameObject panelSelection;
	public GameObject panelChat;

	/*public class SyncListGameObject : SyncList<GameObject> {

		protected override void SerializeItem(NetworkWriter writer, GameObject item)
		{
			writer.Write(item);
		}

		protected override GameObject DeserializeItem(NetworkReader reader)
		{
			return reader.ReadGameObject();
		}
			
	}*/
	public SyncListInt numbers = new SyncListInt();
	public SyncListInt terrains	= new SyncListInt();
	public SyncListInt harbortypes	= new SyncListInt();

	public int vpToWin;

	public List<GameObject> tiles = new List<GameObject>();
	public List<GameObject> landHexes = new List<GameObject>(); 
	public List<GameObject> islandHexes = new List<GameObject>();
	public List<GameObject> junctions = new List<GameObject>();
	public List<GameObject> edges = new List<GameObject>();
	public List<GameObject> harbours = new List<GameObject> ();
	public GameObject robber;
	public GameObject merchant;
	public GameObject pirate;

	public int firstNumTokenSelected;
	public int secondNumTokenSelected;

	public delegate void GameBoardDelegate();
	public GameBoardDelegate waitingDelegate;
	public void RunWaitingDelegate(){
		if (waitingDelegate != null) {
			waitingDelegate ();
			waitingDelegate = null;
		}
	}

	// Use this for initialization
	public override void OnStartServer(){
		List<int> deck = new List<int>{0,1,1,1,1,2,2,2,2,3,3,3,4,4,4,5,5,5,5};
		int random;
		//terrains
		for (int i = 19; i > 0; i--) {
			random = Random.Range (0, i);
			terrains.Add (deck[random]);
			deck.RemoveAt (random);
		}
		//harbortypes
		deck = new List<int> {0,0,0,0,1,2,3,4,5};
		for (int i = 9; i > 0; i--) {
			random = Random.Range (0, i);
			harbortypes.Add (deck[random]);
			deck.RemoveAt (random);
		}
		//numbers
		deck = new List<int>{6, 6, 8, 8, 2, 3, 3, 4, 4, 5, 5, 9, 9, 10, 10, 11, 11, 12};
		VertexCreator();
		EdgeCreator ();
		HexCreator ();
		HarbourCreator ();
		int counterA = 0;

		foreach (GameObject h in landHexes)
		{
			h.GetComponent<Hex> ().resourceType = terrains [counterA];
			counterA++;
		}

		List<Hex> tempHexList = new List<Hex> ();
		foreach (GameObject o in landHexes) {
			tempHexList.Add (o.GetComponent<Hex> ());
			numbers.Add (0);
		}
		while (tempHexList.Count > 1) 
		{
			bool sixOrEightNeibored = false;
			int randomIndex = Random.Range (0, tempHexList.Count);	//we will choose a random Hex
			Hex randomHex = tempHexList [randomIndex];
			if (randomHex.hexNumber == -1 && randomHex.resourceType != 0) //if no number was assigned and its not desert
			{
				if (deck [0] == 6 || deck [0] == 8) // if we have a 6 or an 8
				{	//we check if the neighbours have a 6 or 8
					foreach (Edge e in randomHex.adjacentEdges){
						if (e != null) {
							foreach (Hex h in e.adjacentHexes) {
								if (h != null) {
									if ((h.hexNumber == 8)||(h.hexNumber == 6)) {
										sixOrEightNeibored = true;
									} 
								}
							}
						}
					}
					if (!sixOrEightNeibored) //if it does not have a neighbour with 6 or 8, we add the number
					{
						randomHex.hexNumber = deck [0];
						numbers [landHexes.IndexOf (randomHex.gameObject)] = deck [0];
						tempHexList.RemoveAt (randomIndex);
						deck.RemoveAt (0);	
					}
				}
				else
				{
					randomHex.hexNumber = deck[0];
					numbers [landHexes.IndexOf (randomHex.gameObject)] = deck [0];
					tempHexList.RemoveAt (randomIndex);
					deck.RemoveAt (0);	
				}
			}
		}

		int counterB = 0;
		foreach (GameObject h in tiles)
		{

			float[] pos = h.GetComponent<Hex> ().getPosition();
			if (h.GetComponent<Hex> ().isLand) {
				foreach (Vertex v in h.GetComponent<Hex>().adjacentVertices) {
					v.isLand = true;
				}
				GameObject thisTerrain = Instantiate (landTiles [h.GetComponent<Hex> ().resourceType], new Vector3 (pos [0], 0, pos [1]), Quaternion.Euler (new Vector3 (0, -30 + 60 * Random.Range (0, 6), 0)));
				thisTerrain.transform.SetParent (h.transform, true);
				h.GetComponent<Hex> ().hexNumber = numbers [counterB];
				counterB++;
				if (h.GetComponent<Hex> ().resourceType != 0) {
					GameObject thisNum = Instantiate (num, h.transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
					thisNum.transform.SetParent (h.transform, true);
					thisNum.GetComponent<TextMesh> ().text = h.GetComponent<Hex> ().hexNumber.ToString();
					if (h.GetComponent<Hex> ().hexNumber == 6 || h.GetComponent<Hex> ().hexNumber == 8) {
						thisNum.GetComponent<TextMesh> ().color = Color.red;
					}
				}else {
					robber = Instantiate (robberObj, h.transform.position, Quaternion.identity);
					robber.transform.SetParent (h.transform, true);
				}
			} else {
				GameObject thisTerrain = Instantiate(tile0, new Vector3(pos [0], 0, pos [1]), Quaternion.Euler(new Vector3(90, 90, 0)));
				thisTerrain.transform.SetParent (h.transform, true);
			}
		}

		islandHexes.Add (tiles [15]);
		islandHexes.Add (tiles [24]);
		islandHexes.Add (tiles [41]);
		islandHexes.Add (tiles [49]);
		islandHexes.Add (tiles [66]);
		islandHexes.Add (tiles [65]);
		islandHexes.Add (tiles [64]);
		islandHexes.Add (tiles [62]);
		islandHexes.Add (tiles [61]);

		islandHexes [0].GetComponent<Hex> ().resourceType = 1;
		islandHexes [1].GetComponent<Hex> ().resourceType = 3;
		islandHexes [2].GetComponent<Hex> ().resourceType = 6;
		islandHexes [3].GetComponent<Hex> ().resourceType = 3;
		islandHexes [4].GetComponent<Hex> ().resourceType = 5;
		islandHexes [5].GetComponent<Hex> ().resourceType = 2;
		islandHexes [6].GetComponent<Hex> ().resourceType = 4;
		islandHexes [7].GetComponent<Hex> ().resourceType = 2;
		islandHexes [8].GetComponent<Hex> ().resourceType = 6;

		islandHexes [0].GetComponent<Hex> ().hexNumber = 11;
		islandHexes [1].GetComponent<Hex> ().hexNumber = 4;
		islandHexes [2].GetComponent<Hex> ().hexNumber = 3;
		islandHexes [3].GetComponent<Hex> ().hexNumber = 10;
		islandHexes [4].GetComponent<Hex> ().hexNumber = 5;
		islandHexes [5].GetComponent<Hex> ().hexNumber = 8;
		islandHexes [6].GetComponent<Hex> ().hexNumber = 11;
		islandHexes [7].GetComponent<Hex> ().hexNumber = 9;
		islandHexes [8].GetComponent<Hex> ().hexNumber = 6;

		foreach (GameObject o in islandHexes) {
			o.GetComponent<Hex> ().isLand = true;
			foreach (Vertex v in o.GetComponent<Hex>().adjacentVertices) {
				v.isLand = true;
			}
			foreach (Edge e in o.GetComponent<Hex> ().adjacentEdges) {
				if (e != null) {
					e.isLand = true;
				}
			}
			GameObject newTerrain = Instantiate (landTiles [o.GetComponent<Hex> ().resourceType], o.transform.position, Quaternion.Euler (new Vector3 (0, -30 + 60 * Random.Range (0, 6), 0)));
			newTerrain.transform.SetParent (o.transform, true);
			GameObject newNum = Instantiate (num, o.transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
			newNum.transform.SetParent (o.transform, true);
			newNum.GetComponent<TextMesh> ().text = o.GetComponent<Hex>().hexNumber.ToString();
			if (o.GetComponent<Hex> ().hexNumber == 6 || o.GetComponent<Hex> ().hexNumber == 8) {
				newNum.GetComponent<TextMesh> ().color = Color.red;
			}
		}

		// set edge isWater
		foreach (GameObject e in edges) {
			bool hex1 = false;
			bool hex2 = false;
			if (e.GetComponent<Edge>().adjacentHexes [0] != null) {
				hex1 = e.GetComponent<Edge>().adjacentHexes [0].isLand;
			}
			if (e.GetComponent<Edge>().adjacentHexes [1] != null) {
				hex2 = e.GetComponent<Edge>().adjacentHexes [1].isLand;
			}
			if (!hex1 || !hex2) {
				e.GetComponent<Edge>().isCoast = true;
			}
		}



		for (int i = 0; i < 3; i++) {
			GameObject.Find ("panelFlipChart").transform.GetChild (i).gameObject.GetComponent<Button> ().interactable = false;
		}
		merchant = Instantiate (merchantObj, new Vector3 (-100f, 0, -100f), Quaternion.identity);
		pirate = Instantiate (pirateObj, new Vector3 (100f, 0, 100f), Quaternion.identity);
	}

	public override void OnStartClient()
	{	
		if (isServer) {
			return;
		}
		VertexCreator();
		EdgeCreator();
		HexCreator();
		HarbourCreator ();

		int counterA = 0;
		int counterB = 0;
		foreach (GameObject h in tiles)
		{

			float[] pos = h.GetComponent<Hex> ().getPosition();
			if (h.GetComponent<Hex> ().isLand) {
				foreach (Vertex v in h.GetComponent<Hex>().adjacentVertices) {
					v.isLand = true;
				}
				h.GetComponent<Hex> ().resourceType = terrains [counterA];
				counterA++;
				GameObject thisTerrain = Instantiate (landTiles [h.GetComponent<Hex> ().resourceType], new Vector3 (pos [0], 0, pos [1]), Quaternion.Euler (new Vector3 (0, -30 + 60 * Random.Range (0, 6), 0)));
				thisTerrain.transform.SetParent (h.transform, true);
				h.GetComponent<Hex> ().hexNumber = numbers [counterB];
				counterB++;
				if (h.GetComponent<Hex> ().resourceType != 0) {
					GameObject thisNum = Instantiate (num, h.transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
					thisNum.transform.SetParent (h.transform, true);
					thisNum.GetComponent<TextMesh> ().text = h.GetComponent<Hex> ().hexNumber.ToString();
					if (h.GetComponent<Hex> ().hexNumber == 6 || h.GetComponent<Hex> ().hexNumber == 8) {
						thisNum.GetComponent<TextMesh> ().color = Color.red;
					}
				} else {
					robber = Instantiate (robberObj, h.transform.position, Quaternion.identity);
					robber.transform.SetParent (h.transform, true);
				}
			} else {
				GameObject thisTerrain = Instantiate(tile0, new Vector3(pos [0], 0, pos [1]), Quaternion.Euler(new Vector3(90, 90, 0)));
				thisTerrain.transform.SetParent (h.transform, true);
			}
		}

		islandHexes.Add (tiles [15]);
		islandHexes.Add (tiles [24]);
		islandHexes.Add (tiles [41]);
		islandHexes.Add (tiles [49]);
		islandHexes.Add (tiles [66]);
		islandHexes.Add (tiles [65]);
		islandHexes.Add (tiles [64]);
		islandHexes.Add (tiles [62]);
		islandHexes.Add (tiles [61]);

		islandHexes [0].GetComponent<Hex> ().resourceType = 1;
		islandHexes [1].GetComponent<Hex> ().resourceType = 3;
		islandHexes [2].GetComponent<Hex> ().resourceType = 6;
		islandHexes [3].GetComponent<Hex> ().resourceType = 3;
		islandHexes [4].GetComponent<Hex> ().resourceType = 5;
		islandHexes [5].GetComponent<Hex> ().resourceType = 2;
		islandHexes [6].GetComponent<Hex> ().resourceType = 4;
		islandHexes [7].GetComponent<Hex> ().resourceType = 2;
		islandHexes [8].GetComponent<Hex> ().resourceType = 6;

		islandHexes [0].GetComponent<Hex> ().hexNumber = 11;
		islandHexes [1].GetComponent<Hex> ().hexNumber = 4;
		islandHexes [2].GetComponent<Hex> ().hexNumber = 3;
		islandHexes [3].GetComponent<Hex> ().hexNumber = 10;
		islandHexes [4].GetComponent<Hex> ().hexNumber = 5;
		islandHexes [5].GetComponent<Hex> ().hexNumber = 8;
		islandHexes [6].GetComponent<Hex> ().hexNumber = 11;
		islandHexes [7].GetComponent<Hex> ().hexNumber = 9;
		islandHexes [8].GetComponent<Hex> ().hexNumber = 6;

		foreach (GameObject o in islandHexes) {
			o.GetComponent<Hex> ().isLand = true;
			foreach (Vertex v in o.GetComponent<Hex>().adjacentVertices) {
				v.isLand = true;
			}
			foreach (Edge e in o.GetComponent<Hex> ().adjacentEdges) {
				if (e != null) {
					e.isLand = true;
				}
			}
			GameObject newTerrain = Instantiate (landTiles [o.GetComponent<Hex> ().resourceType], o.transform.position, Quaternion.Euler (new Vector3 (0, -30 + 60 * Random.Range (0, 6), 0)));
			newTerrain.transform.SetParent (o.transform, true);
			GameObject newNum = Instantiate (num, o.transform.position, Quaternion.Euler (new Vector3 (90, 0, 0)));
			newNum.transform.SetParent (o.transform, true);
			newNum.GetComponent<TextMesh> ().text = o.GetComponent<Hex>().hexNumber.ToString();
			if (o.GetComponent<Hex> ().hexNumber == 6 || o.GetComponent<Hex> ().hexNumber == 8) {
				newNum.GetComponent<TextMesh> ().color = Color.red;
			}
		}

		// set edge isWater
		foreach (GameObject e in edges) {
			bool hex1 = false;
			bool hex2 = false;
			if (e.GetComponent<Edge>().adjacentHexes [0] != null) {
				hex1 = e.GetComponent<Edge>().adjacentHexes [0].isLand;
			}
			if (e.GetComponent<Edge>().adjacentHexes [1] != null) {
				hex2 = e.GetComponent<Edge>().adjacentHexes [1].isLand;
			}
			if (!hex1 || !hex2) {
				e.GetComponent<Edge>().isCoast = true;
			}
		}
			
		for (int i = 0; i < 3; i++) {
			GameObject.Find ("panelFlipChart").transform.GetChild (i).gameObject.GetComponent<Button> ().interactable = false;
		}
		merchant = Instantiate (merchantObj, new Vector3 (-100f, 0, -100f), Quaternion.identity);
		pirate = Instantiate (pirateObj, new Vector3 (100f, 0, 100f), Quaternion.identity);
	}

	private void VertexCreator()
	{
		float x = 0;      // Current x position.
		float y = 0;      // Current y position.
		float isHalfDistance = 0;
		while (x <= 14.722f || y <= 17.16f)
		{
			isHalfDistance = 1 - isHalfDistance;
			for (int i = 0; i < 2; i++)
			{
				while (x <= 14.722f)
				{
					GameObject thisBase = Instantiate(vertexObj,new Vector3(x,0.03f,y),Quaternion.Euler(new Vector3(-90, 0, 0)));
					thisBase.GetComponent<Vertex> ().setPosition (x, y);
					thisBase.GetComponent<Vertex> ().harbourtype = HarbourType.None;
					junctions.Add(thisBase);
					thisBase.GetComponent<MeshRenderer> ().enabled = false;
					thisBase.GetComponent<CircleCollider2D> ().enabled = false;
					x = x + (2f * 0.866f);
				}
				if (y <= 17.16f)
				{
					x = (isHalfDistance) * 0.866f;
				}
				y = y + (1f / (2f - i));
			}
		}

	}

	private void EdgeCreator()
	{
		for(int i = 0; i < junctions.Count; i++)
		{
			for(int j = i + 1; j < junctions.Count; j++)
			{
				float[] ipos = junctions [i].GetComponent<Vertex> ().getPosition();
				float[] jpos = junctions[j].GetComponent<Vertex> ().getPosition();
				if((ipos[0] - jpos[0]) * (ipos[0] - jpos[0]) + (ipos[1] - jpos[1]) * (ipos[1] - jpos[1]) < 2)
				{
					GameObject thisEdge = Instantiate(edgeObj, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));

					if (jpos[0] == ipos[0])
					{
						thisEdge.GetComponent<Edge> ().setPosition((ipos[0] + jpos[0]) / 2, (ipos[1] + jpos[1]) / 2, 0);
					}
					else if(jpos[0] - ipos[0] > jpos[1] - ipos[1])
					{
						thisEdge.GetComponent<Edge> ().setPosition((ipos[0] + jpos[0]) / 2, (ipos[1] + jpos[1]) / 2, 1);
					}
					else
					{
						thisEdge.GetComponent<Edge> ().setPosition((ipos[0] + jpos[0]) / 2, (ipos[1] + jpos[1]) / 2, -1);
					}
					thisEdge.GetComponent<Edge> ().add(junctions[i].GetComponent<Vertex> ());
					thisEdge.GetComponent<Edge> ().add(junctions[j].GetComponent<Vertex> ());
					junctions[i].GetComponent<Vertex> ().add(thisEdge.GetComponent<Edge> ());
					junctions[j].GetComponent<Vertex> ().add(thisEdge.GetComponent<Edge> ());
					edges.Add(thisEdge);
					thisEdge.transform.GetChild(0).gameObject.SetActive (false);
					thisEdge.GetComponent<BoxCollider> ().enabled = false;
				}
			}
		}
	}

	private void HexCreator()
	{
		float x = 0;      // Current x position.
		float y = 1;       // Current y position.
		float isHalfDistance = 0;
		while (x <= 14.722f || y <= 17.16f)
		{
			isHalfDistance = 1 - isHalfDistance;
			while (x <= 14.722f)
			{
				GameObject thisHex = Instantiate(hexObj, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));

				thisHex.GetComponent<Hex> ().setPosition(x, y);
				if (y <= 8.5f && y >= 2.5f && y <= ((750f/433f)*(x-0.8f)+2.5f) && y <= ((-(750f/433f))*(x-9f)+8.5f) && y >= (-(750f/433f)*(x-0.8f)+8.5f) && y >= ((750f/433f)*(x-9f)+2.5f))
				{
					thisHex.GetComponent<Hex> ().isLand = true;
					landHexes.Add (thisHex);
				}

				thisHex.GetComponent<Hex> ().linkVertices(junctions);
				thisHex.GetComponent<Hex> ().linkEdges(edges);
				tiles.Add(thisHex);
				x = x + 2 * 0.866f;
			}
			if (y <= 17.16f)
			{
				x = isHalfDistance * 0.866f;
			}
			y = y + 1.5f;
		}
	}

	// set all habours and the adjacentVertices
	private void HarbourCreator()
	{
		GameObject harbour1 = Instantiate (harbourObj, edges [28].transform.position+new Vector3(-0.1f,0,-0.1f), Quaternion.Euler(new Vector3(-90, 30, 0)));
		edges [28].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[0]);
		edges [28].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[0]);
		GameObject harbourIcon1 = Instantiate (harbourIconObj, edges [28].transform.position+new Vector3(-0.5f,0,-0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon1.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[0]];

		GameObject harbour2 = Instantiate (harbourObj, edges [31].transform.position+new Vector3(0.1f,0,-0.1f), Quaternion.Euler(new Vector3(-90, -30, 0)));
		edges [31].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[1]);
		edges [31].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[1]);
		GameObject harbourIcon2 = Instantiate (harbourIconObj, edges [31].transform.position+new Vector3(0.5f,0,-0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon2.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[1]];

		GameObject harbour3 = Instantiate (harbourObj, edges [59].transform.position+new Vector3(0.1f,0,-0.1f), Quaternion.Euler(new Vector3(-90, -30, 0)));
		edges [59].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[2]);
		edges [59].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[2]);
		GameObject harbourIcon3 = Instantiate (harbourIconObj, edges [59].transform.position+new Vector3(0.5f,0,-0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon3.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[2]];

		GameObject harbour4 = Instantiate (harbourObj, edges [66].transform.position+new Vector3(-0.2f,0,0), Quaternion.Euler(new Vector3(-90, 90, 0)));
		edges [66].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[3]);
		edges [66].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[3]);
		GameObject harbourIcon4 = Instantiate (harbourIconObj, edges [66].transform.position+new Vector3(-0.8f,0,0), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon4.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[3]];

		GameObject harbour5 = Instantiate (harbourObj,edges [95].transform.position+new Vector3(0.2f,0,0), Quaternion.Euler(new Vector3(-90, -90, 0)));
		edges [95].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[4]);
		edges [95].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[4]);
		GameObject harbourIcon5 = Instantiate (harbourIconObj, edges [95].transform.position+new Vector3(0.8f,0,0), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon5.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[4]];

		GameObject harbour6 = Instantiate (harbourObj, edges [115].transform.position+new Vector3(-0.2f,0,0), Quaternion.Euler(new Vector3(-90, 90, 0)));
		edges [115].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[5]);
		edges [115].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[5]);
		GameObject harbourIcon6 = Instantiate (harbourIconObj, edges [115].transform.position+new Vector3(-0.8f,0,0), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon6.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[5]];

		GameObject harbour7 = Instantiate (harbourObj, edges [132].transform.position+new Vector3(0.1f,0,0.1f), Quaternion.Euler(new Vector3(-90, -150, 0)));
		edges [132].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[6]);
		edges [132].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[6]);
		GameObject harbourIcon7 = Instantiate (harbourIconObj, edges [132].transform.position+new Vector3(0.5f,0,0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon7.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[6]];

		GameObject harbour8 = Instantiate (harbourObj, edges [151].transform.position+new Vector3(-0.1f,0,0.1f), Quaternion.Euler(new Vector3(-90, 150, 0)));
		edges [151].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[7]);
		edges [151].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[7]);
		GameObject harbourIcon8 = Instantiate (harbourIconObj, edges [151].transform.position+new Vector3(-0.5f,0,0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon8.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[7]];

		GameObject harbour9 = Instantiate (harbourObj, edges [154].transform.position+new Vector3(0.1f,0,0.1f), Quaternion.Euler(new Vector3(-90, -150, 0)));
		edges [154].GetComponent<Edge> ().adjacentVertices [0].harbourtype = IdentifyHType(harbortypes[8]);
		edges [154].GetComponent<Edge> ().adjacentVertices [1].harbourtype = IdentifyHType(harbortypes[8]);
		GameObject harbourIcon9 = Instantiate (harbourIconObj, edges [154].transform.position+new Vector3(0.5f,0,0.5f), Quaternion.Euler(new Vector3(0, 180, 0)));
		harbourIcon9.GetComponent<Renderer> ().sharedMaterial = harbourIconMaterials [harbortypes[8]];

	}

	private HarbourType IdentifyHType(int i){
		if (i == 0) {
			return HarbourType.Generic;
		}
		if (i == 1) {
			return HarbourType.Lumber;
		}
		if (i == 2) {
			return HarbourType.Brick;
		}
		if (i == 3) {
			return HarbourType.Wool;
		}
		if (i == 4) {
			return HarbourType.Grain;
		}
		if (i == 5) {
			return HarbourType.Ore;
		}
		return HarbourType.None;
	}

	public void GetTurn(){
		panelActions.SetActive (true);
		EnableAllKnightsAndVillagesSelection (true);
		bool hasCity = false;
		bool hasMetropole = false;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();

		RenewImprovementsAvailability();

		// renew knights status
		foreach (Knight k in localPlayer.knights) {
			k.hasMovedThisTurn = false;
			k.hasPromotedThisTurn = false;
		}


		panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Your Turn!");
	}

	public void RenewImprovementsAvailability(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		for (int i = 0; i < 3; i++) {
			GameObject.Find ("panelFlipChart").transform.GetChild (i).gameObject.GetComponent<Button> ().interactable = false;
		}
		// set improvements availability
		bool hasMetro = false;
		foreach(Village v in localPlayer.villages){
			if (v.vt == VillageType.City) {
				for (int i = 0; i < 3; i++) {
					GameObject.Find ("panelFlipChart").transform.GetChild (i).gameObject.GetComponent<Button> ().interactable = true;
				}
				break;
			} else if (v.vt == VillageType.TradeMetropole) {
				GameObject.Find ("panelFlipChart").transform.GetChild (0).gameObject.GetComponent<Button> ().interactable = true;
				hasMetro = true;
			} else if (v.vt == VillageType.PoliticsMetropole) {
				GameObject.Find ("panelFlipChart").transform.GetChild (1).gameObject.GetComponent<Button> ().interactable = true;
				hasMetro = true;
			} else if (v.vt == VillageType.ScienceMetropole) {
				GameObject.Find ("panelFlipChart").transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = true;
				hasMetro = true;
			}
		}
		if (!GameObject.Find ("panelFlipChart").transform.GetChild (0).gameObject.GetComponent<Button> ().interactable) {
			if (hasMetro&&localPlayer.fcTradeLvl < 4) {
				GameObject.Find ("panelFlipChart").transform.GetChild (0).gameObject.GetComponent<Button> ().interactable = true;
			}
		} else {
			if (localPlayer.fcTradeLvl >= 6) {
				GameObject.Find ("panelFlipChart").transform.GetChild (0).gameObject.GetComponent<Button> ().interactable = false;
			}
		}
		if (!GameObject.Find ("panelFlipChart").transform.GetChild (1).gameObject.GetComponent<Button> ().interactable) {
			if (hasMetro&&localPlayer.fcPoliticsLvl < 4) {
				GameObject.Find ("panelFlipChart").transform.GetChild (1).gameObject.GetComponent<Button> ().interactable = true;
			}
		} else {
			if (localPlayer.fcPoliticsLvl >= 6) {
				GameObject.Find ("panelFlipChart").transform.GetChild (1).gameObject.GetComponent<Button> ().interactable = false;
			}
		}
		if (!GameObject.Find ("panelFlipChart").transform.GetChild (2).gameObject.GetComponent<Button> ().interactable) {
			if (hasMetro&&localPlayer.fcScienceLvl < 4) {
				GameObject.Find ("panelFlipChart").transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = true;
			}
		} else {
			if (localPlayer.fcScienceLvl >= 6) {
				GameObject.Find ("panelFlipChart").transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = false;
			}
		}
		// set improvements availability ends
	}

	public void RequestBuildSettlementRoundOne(){
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildVertexRequestEnd;
		foreach (GameObject v in junctions) {
			if (v.GetComponent<Vertex> ().isBuildable) {
				v.GetComponent<MeshRenderer> ().enabled = true;
				v.GetComponent<CircleCollider2D> ().enabled = true;
				v.GetComponent<Vertex> ().waitingBuildDelegate = v.GetComponent<Vertex> ().BuildSettlementRoundOne;
			}
		}
		panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Build a settlement!");
	}

	public void RequestBuildSettlement(){
		//max 5 per player
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		//enough resources?
		if (localPlayer.resources [0] < 1 || localPlayer.resources [1] < 1
			|| localPlayer.resources [2] < 1 || localPlayer.resources [3] < 1) {
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.",null,null);
			return;
		}
		//max number of settlements? 
		int villageCounter = 0;
		List<Village> villages = localPlayer.villages;
		foreach (var village in villages) {
			if (village.vt == VillageType.Settlement)
				villageCounter++;
		}
		if (villageCounter > 4){
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have reached your max number of settlements! (5)",null,null);
			return;
		}

		BuildEdgeRequestEnd ();
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildVertexRequestEnd;
		GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		foreach (EdgeUnit eu in localPlayer.roads) {
			Edge e = eu.gameObject.transform.parent.gameObject.GetComponent<Edge> ();
			foreach (Vertex v in e.adjacentVertices) {
				if (v != null && v.isBuildable && !v.isOccupied) {
					v.GetComponent<MeshRenderer> ().enabled = true;
					v.GetComponent<CircleCollider2D> ().enabled = true;
					v.GetComponent<Vertex> ().waitingBuildDelegate = v.GetComponent<Vertex> ().BuildSettlement;
				}
			}
		}
	}

	public void RequestBuildCity(){
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildVertexRequestEnd;
		foreach (GameObject v in junctions) {
			if (v.GetComponent<Vertex> ().isBuildable) {
				v.GetComponent<MeshRenderer> ().enabled = true;
				v.GetComponent<CircleCollider2D> ().enabled = true;
				v.GetComponent<Vertex> ().waitingBuildDelegate = v.GetComponent<Vertex> ().BuildCityRoundTwo;
			}
		}
		panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Build a city!");
		panelActions.transform.GetChild (1).GetChild (5).gameObject.GetComponent<Button> ().onClick.AddListener (GameObject.Find ("Local Player Panel").GetComponent<Player> ().EndTurn);
	}

	public void RequestBuildMetropole(int type){
		BuildEdgeRequestEnd ();
		BuildVertexRequestEnd ();
		EnableAllKnightsAndVillagesSelection (false);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		foreach (Village v in localPlayer.villages) {
			if(v.vt.Equals(VillageType.City)){
				Vertex myVertex = v.gameObject.transform.parent.gameObject.GetComponent<Vertex> ();
				myVertex.gameObject.GetComponent<MeshRenderer> ().enabled = true;
				myVertex.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
				myVertex.waitingBuildDelegate = delegate {
					myVertex.UpgradeCityToMetropole (type);
				};
			}
		}
	}

	public void RequestDowngradeCity(){
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildVertexRequestEnd;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		int cityCount = 0;
		foreach (Village v in localPlayer.villages) {
			if (v.vt.Equals (VillageType.City)) {
				Vertex myVertex = v.gameObject.transform.parent.gameObject.GetComponent<Vertex> ();
				myVertex.gameObject.GetComponent<MeshRenderer> ().enabled = true;
				myVertex.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
				myVertex.waitingBuildDelegate = myVertex.DowngradeCity;
				cityCount++;
			}
		}
		if (cityCount == 0) {
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You don't have any city to be downgraded.",null,null);
			localPlayer.CmdResponseToServer ();
		}
	}

	public void RequestBuildRoad(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if ((localPlayer.resources [0] < 1 || localPlayer.resources [1] < 1) && (localPlayer.roadBuilding == 0)) {
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
			return;
		}
		BuildVertexRequestEnd ();
		BuildEdgeRequestEnd ();
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildEdgeRequestEnd;
		bool hasRoadToBuild = false;
		foreach (GameObject e in edges) {
			if (e.GetComponent<Edge> ().isBuildable && e.GetComponent<Edge> ().isLand) {
				e.GetComponent<BoxCollider> ().enabled = true;
				e.transform.GetChild (0).gameObject.SetActive (true);
				e.GetComponent<Edge> ().waitingBuildDelegate = e.GetComponent<Edge> ().buildRoad;
				hasRoadToBuild = true;
			}
		}
		if (!hasRoadToBuild && localPlayer.roadBuilding != 0) {
			localPlayer.roadBuilding = 0;
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You are not able to build any roads, the Road building card is wasted.",null,null);
		}
	}

	public void RequestPlayRoadBuildingCard(){
		StartCoroutine (SelectTwoRoadToBuild ());
	}

	public IEnumerator SelectTwoRoadToBuild(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		panelInfo.GetComponent<InfoPanel> ().pushMessage ("Select the first road to build.", RequestBuildRoad, RequestBuildRoad);
		yield return new WaitUntil (() => localPlayer.roadBuilding < 2);
		panelInfo.GetComponent<InfoPanel> ().pushMessage ("Select the second road to build.", RequestBuildRoad, RequestBuildRoad);
		yield return new WaitUntil (() => localPlayer.roadBuilding < 1);
		EnableAllKnightsAndVillagesSelection (true);
		EnablePrimaryActionPanel (true);
	}

	public void RequestBuildShip(){ 
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (localPlayer.resources [0] < 1 || localPlayer.resources [2] < 1) {
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.",null,null);
			return;
		}
		BuildVertexRequestEnd ();
		BuildEdgeRequestEnd ();
		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildEdgeRequestEnd;

		foreach (GameObject g in edges) {
			Edge current = g.GetComponent<Edge> ();
			if (current.isShipBuildable){
				g.GetComponent<BoxCollider> ().enabled = true;
				g.transform.GetChild (0).gameObject.SetActive (true);
				g.GetComponent<Edge> ().waitingBuildDelegate = g.GetComponent<Edge> ().buildShip;
//				Vertex adj1 = current.adjacentVertices[0];
//				List<Edge> adjEdges1 = new List<Edge> ();
//				for (int i = 0; i < 3; i++) {
//					if (adj1.adjacentEdges [i] != current) {
//						adjEdges1.Add (adj1.adjacentEdges [i]);
//					}
//				}
//
//				Vertex adj2 = current.adjacentVertices[1];
//				List<Edge> adjEdges2 = new List<Edge> ();
//				for (int i = 0; i < 3; i++) {
//					if (adj2.adjacentEdges [i] != current) {
//						adjEdges2.Add (adj2.adjacentEdges [i]);
//					}
//				}
//
//				foreach (Edge neighborEdge in adjEdges1) {
//					for (int k = 0; k < side1Safe.Length; k++) {
//						if (neighborEdge.isLand || neighborEdge.isOccupied) {
//							side1Safe [k] = false;
//						} else if (!neighborEdge.isLand || neighborEdge.isCoast){
//							side1Safe [k] = true;
//						}
//					}
//					if (side1Safe [0]) {
//						if (current.isCoast && !neighborEdge.isLand) {
//							//we can build on a coastal route
//							g.GetComponent<BoxCollider> ().enabled = true;
//							g.transform.GetChild (0).gameObject.SetActive (true);
//						}
//					}
//					if (side1Safe [1]) {
//						if (!current.isLand && !neighborEdge.isLand) {
//							//we can build on an aquatic route
//							g.GetComponent<BoxCollider> ().enabled = true;
//							g.transform.GetChild (0).gameObject.SetActive (true);
//						}
//					}
//				}
//					
//				foreach (Edge neighborEdge in adjEdges2) {
//					for (int k = 0; k < side2Safe.Length; k++) {
//						if (neighborEdge.isLand || neighborEdge.isOccupied) {
//							side2Safe [k] = false;
//						} else if (!neighborEdge.isLand || neighborEdge.isCoast) {
//							side2Safe [k] = true;
//						}
//					}
//					if (side2Safe [0]) {
//						if (current.isCoast && !neighborEdge.isLand) {
//							//we can build on a coastal route
//							g.GetComponent<BoxCollider> ().enabled = true;
//							g.transform.GetChild (0).gameObject.SetActive (true);
//						}
//					}
//					if (side2Safe [1]) {
//						if (!current.isLand && !neighborEdge.isLand) {
//							//we can build on an aquatic route
//							g.GetComponent<BoxCollider> ().enabled = true;
//							g.transform.GetChild (0).gameObject.SetActive (true);
//						}
//					}
//				}
//			}
			}
		}
	}

	public void RequestPurchaseKnight(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		//ONE WOOL ONE ORE
		if (localPlayer.resources [2] < 1 || localPlayer.resources [4] < 1) {
			panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.",null,null);
			return;
		}

		EnableAllKnightsAndVillagesSelection (false);
		waitingDelegate = BuildVertexRequestEnd;
		foreach (EdgeUnit e in GameObject.Find("Local Player Panel").GetComponent<Player>().roads) {
			foreach (Vertex v in e.gameObject.transform.parent.gameObject.GetComponent<Edge>().adjacentVertices) {
				if (v != null && !v.isRealOccupied && !v.GetComponent<MeshRenderer> ().enabled) {
					v.gameObject.GetComponent<MeshRenderer> ().enabled = true;
					v.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
					v.waitingBuildDelegate = v.PurchaseKnight;
				}
			}
		}
		panelActions.transform.GetChild (1).GetChild (4).gameObject.SetActive (true);
	}

	public void RequestImproveFlipchart(int chosen){ //0trade, 1politics, 2science, 
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();

		//level 1 trade: 1 cloth / politics: 1coin / science: 1paper
		//level 2 2 of each... etc
		if (chosen == 0) { 
			if ((localPlayer.resources [6] < localPlayer.fcTradeLvl && !localPlayer.isCrane) ||
				(localPlayer.resources [6] < (localPlayer.fcTradeLvl - 1) && localPlayer.isCrane)) {
				panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
				return;
			} else {
				if (localPlayer.isCrane) {
					localPlayer.OnResourcesChanged (ResourceType.Cloth, -localPlayer.fcTradeLvl+1);
					localPlayer.isCrane = false;
				} else {
					localPlayer.OnResourcesChanged (ResourceType.Cloth, -localPlayer.fcTradeLvl);
				}
			}
		} else if (chosen == 1) { 
			if ((localPlayer.resources [7] < localPlayer.fcPoliticsLvl && !localPlayer.isCrane) ||
				(localPlayer.resources [7] < (localPlayer.fcPoliticsLvl - 1) && localPlayer.isCrane)) {
				panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
				return;
			} else {
				if (localPlayer.isCrane) {
					localPlayer.OnResourcesChanged (ResourceType.Coin, -localPlayer.fcPoliticsLvl+1);
					localPlayer.isCrane = false;
				} else {
					localPlayer.OnResourcesChanged (ResourceType.Coin, -localPlayer.fcPoliticsLvl);
				}
			}
		} else if (chosen == 2) { 
			if ((localPlayer.resources [5] < localPlayer.fcScienceLvl && !localPlayer.isCrane) ||
				(localPlayer.resources [5] < (localPlayer.fcScienceLvl - 1) && localPlayer.isCrane)) {
				panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
				return;
			} else {
				if (localPlayer.isCrane) {
					localPlayer.OnResourcesChanged (ResourceType.Paper, -(localPlayer.fcScienceLvl-1));
					localPlayer.isCrane = false;
				} else {
					localPlayer.OnResourcesChanged (ResourceType.Paper, -(localPlayer.fcScienceLvl));
				}
			}
		}

		localPlayer.CmdflipchartUpgrade (chosen);
	}

	public void RequestMoveRobber(){
		EnableAllKnightsAndVillagesSelection (false);
		GameObject currentRobberAt = robber.transform.parent.gameObject;
		foreach (GameObject o in landHexes) {
			if (o != currentRobberAt && (o.GetComponent<Hex> ().resourceType > 0)) {
				o.GetComponent<CircleCollider2D> ().enabled = true;
				o.GetComponent<Hex> ().waitingDelegate = delegate {
					o.GetComponent<Hex> ().MoveRobber(false);
				};
			}
		}
	}

	public void RequestChaseAwayRobber(){
		EnableAllKnightsAndVillagesSelection (false);
		GameObject currentRobberAt = robber.transform.parent.gameObject;
		foreach (GameObject o in landHexes) {
			if (o != currentRobberAt && (o.GetComponent<Hex> ().resourceType > 0)) {
				o.GetComponent<CircleCollider2D> ().enabled = true;
				o.GetComponent<Hex> ().waitingDelegate = delegate {
					o.GetComponent<Hex> ().MoveRobber(true);
				};
			}
		}
	}

	public void RequestMovePirate(){
		EnableAllKnightsAndVillagesSelection (false);
		GameObject currentPirateAt;
		if (pirate.transform.position == new Vector3 (100f, 0, 100f)) {
			currentPirateAt = null;
		} else {
			currentPirateAt = pirate.transform.parent.gameObject;
		}
		foreach (GameObject o in tiles) {
			if (!o.GetComponent<Hex>().isLand && o != currentPirateAt) {
				o.GetComponent<CircleCollider2D> ().enabled = true;
				o.GetComponent<Hex> ().waitingDelegate = o.GetComponent<Hex> ().MovePirate;
			}
		}
	}

	public void RequestMoveMerchant(){
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		GameObject currentMerchantAt;
		if (merchant.transform.position == new Vector3 (-100f, 0, -100f)) {
			currentMerchantAt = null;
		} else {
			currentMerchantAt = merchant.transform.parent.gameObject;
		}
		foreach (Village v in GameObject.Find("Local Player Panel").GetComponent<Player>().villages) {
			foreach (Hex h in v.transform.parent.GetComponent<Vertex>().adjacentHexes) {
				if (h != null && h.gameObject != currentMerchantAt && h.resourceType > 0) {
					h.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
					h.gameObject.GetComponent<Hex> ().waitingDelegate = h.MoveMerchant;
				}
			}
		}
	}

	public void RequestInventorChoice(){
		StartCoroutine(SelectTwoToken());
	}

	public IEnumerator SelectTwoToken(){
		firstNumTokenSelected = -1;
		secondNumTokenSelected = -1;
		panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Select the first number token.");
		RequestInventorFirstToken ();
		yield return new WaitWhile (() => firstNumTokenSelected < 0);
		panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Select the second number token.");
		RequestInventorSecondToken (firstNumTokenSelected);
		yield return new WaitWhile (() => secondNumTokenSelected < 0);
		panelInfoBar.SetActive (false);
		GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdSwapNumberTokens (firstNumTokenSelected, secondNumTokenSelected);
	}

	public void RequestInventorFirstToken(){
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		foreach (GameObject o in tiles) {
			Hex h = o.GetComponent<Hex> ();
			if (h.hexNumber > 0 && (h.hexNumber != 2 && h.hexNumber != 12 && h.hexNumber != 6 && h.hexNumber != 8)) {
				h.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
				h.gameObject.GetComponent<Hex> ().waitingDelegate = delegate {
					firstNumTokenSelected = tiles.IndexOf(o);
					HexActionRequestEnd();
					EnablePrimaryActionPanel (true);
				};
			}
		}
	}

	public void RequestInventorSecondToken(int indexHasChosen){
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		foreach (GameObject o in tiles) {
			Hex h = o.GetComponent<Hex> ();
			if (h.hexNumber > 0 && (h.hexNumber != 2 && h.hexNumber != 12 && h.hexNumber != 6 && h.hexNumber != 8)) {
				if (tiles.IndexOf (o) != indexHasChosen) {
					h.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
					h.gameObject.GetComponent<Hex> ().waitingDelegate = delegate {
						secondNumTokenSelected = tiles.IndexOf(o);
						HexActionRequestEnd();
						EnablePrimaryActionPanel (true);
					};
				}
			}
		}
	}

	public void PlaySpyCard(){
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		foreach (Player p in l) {
			if (p != localPlayer && p.progressCardSum >0) {
				foreach (Village v in p.villages) {
					GameObject o = v.transform.parent.gameObject;
					o.GetComponent<MeshRenderer> ().enabled = true;
					o.GetComponent<CircleCollider2D> ().enabled = true;
					o.GetComponent<Vertex>().waitingBuildDelegate = delegate {
						localPlayer.CmdPlaySpy(l.IndexOf(p));
						BuildVertexRequestEnd();
						EnablePrimaryActionPanel (true);
					};
				}
			}
		}
	}

	public void HexActionRequestEnd(){
		foreach (GameObject o in tiles) {
			o.GetComponent<CircleCollider2D> ().enabled = false;
		}
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.Invoke ();
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		EnableAllKnightsAndVillagesSelection (true);
	}

	public void BuildVertexRequestEnd(){
		foreach (GameObject v in junctions) {
			if (v.GetComponent<MeshRenderer> ().enabled) {
				v.GetComponent<MeshRenderer> ().enabled = false;
				v.GetComponent<CircleCollider2D> ().enabled = false;
			}
		}
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.Invoke ();
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		EnableAllKnightsAndVillagesSelection (true);
	}

	public void BuildEdgeRequestEnd(){
		foreach (GameObject e in edges) {
			if (e.GetComponent<BoxCollider> ().enabled) {
				e.GetComponent<BoxCollider> ().enabled = false;
			}
			if (e.transform.GetChild(0).gameObject.activeSelf) {
				e.transform.GetChild (0).gameObject.SetActive (false);
			}
		}
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.Invoke ();
		GameObject.Find ("panelBuildConfirmation").transform.GetChild (1).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		EnableAllKnightsAndVillagesSelection (true);
	}

	public void EnableAllKnightsAndVillagesSelection(bool selection){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		foreach (Village v in localPlayer.villages) {
			v.GetComponent<BoxCollider> ().enabled = selection;
		}
		foreach (Knight n in localPlayer.knights) {
			n.GetComponent<CapsuleCollider> ().enabled = selection;
		}
	}

	public void EnablePrimaryActionPanel(bool selection){
		for (int i = 0; i < panelActions.transform.GetChild(1).childCount; i++) {
			panelActions.transform.GetChild (1).GetChild (i).gameObject.GetComponent<Button> ().interactable = selection;
		}
	}

	public void RequestBishop(){
		EnableAllKnightsAndVillagesSelection (false);
		EnablePrimaryActionPanel (false);
		GameObject currentRobberAt = robber.transform.parent.gameObject;
		foreach (GameObject o in landHexes) {
			if (o != currentRobberAt && (o.GetComponent<Hex> ().resourceType > 0)) {
				o.GetComponent<CircleCollider2D> ().enabled = true;
				o.GetComponent<Hex> ().waitingDelegate = delegate {
					o.GetComponent<Hex> ().DoBishop();
					EnablePrimaryActionPanel(true);
				} ;
			}
		}
	}

}

