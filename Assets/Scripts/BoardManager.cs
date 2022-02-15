using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public static BoardManager instance;
	public List<Sprite> gems = new List<Sprite>();
	public GameObject tile;
	public int rowSize, columnSize;

	private GameObject[,] tiles;

	public bool IsShifting { get; set; }

	void Start()
	{
		instance = GetComponent<BoardManager>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
		CreateBoard(offset.x, offset.y);
	}

	private void CreateBoard(float xOffset, float yOffset)
	{
		tiles = new GameObject[rowSize, columnSize];

		float startX = transform.position.x;
		float startY = transform.position.y;

		Sprite[] previousGemLeft = new Sprite[columnSize];
		Sprite previousGemBelow = null;

		int i = 0;

		for (int x = 0; x < rowSize; x++)
		{
			for (int y = 0; y < columnSize; y++)
			{
				GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
				tiles[x, y] = newTile;
				tiles[x, y].name = "" + i;

				newTile.transform.parent = transform;

				List<Sprite> possibleGems = new List<Sprite>();
				possibleGems.AddRange(gems);
				possibleGems.Remove(previousGemLeft[y]);
				possibleGems.Remove(previousGemBelow);

				Sprite newSprite = possibleGems[Random.Range(0, possibleGems.Count)];
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

				previousGemLeft[y] = newSprite;
				previousGemBelow = newSprite;
				i++;
			}
		}
	}

	public IEnumerator FindNullTiles()
	{
		for (int x = 0; x < rowSize; x++)
		{
			for (int y = 0; y < columnSize; y++)
			{
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
				{
                    if (y == (columnSize - 1))
                    {
                        tiles[x, y].GetComponent<SpriteRenderer>().sprite = GetNewSprite(x, y);
						GUIManager.instance.Score += 10;
					}
                    yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}

		for (int x = 0; x < rowSize; x++)
		{
			for (int y = 0; y < columnSize; y++)
			{
				tiles[x, y].GetComponent<Tile>().ClearAllMatches();
			}
		}
	}


	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
	{		
		IsShifting = true;
		List<SpriteRenderer> renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < columnSize; y++)
		{  
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null)
			{ 
				nullCount++;
			}
			renders.Add(render);
		}
		
		for (int i = 0; i < nullCount; i++)
		{
			GUIManager.instance.Score += 10;
			yield return new WaitForSeconds(shiftDelay);
			for (int k = 0; k < renders.Count - 1; k++)
			{
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, columnSize - 1);
			}
		}
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y)
	{
		List<Sprite> possibleGems = new List<Sprite>();
		possibleGems.AddRange(gems);

		if (x > 0)
		{
			possibleGems.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < rowSize - 1)
		{
			possibleGems.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0)
		{
			possibleGems.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleGems[Random.Range(0, possibleGems.Count)];
	}
}
