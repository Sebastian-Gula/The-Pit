using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public class Room
    {
        public int roomNumber;
        public int size;

        public Room(int roomNumber, int size)
        {
            this.roomNumber = roomNumber;
            this.size = size;
        }
    }

    public enum BoardFields
    {
        Wall = 0,
        TMP = 1,
        Floor = 2,
        LeftWall = 3,
        TopWall = 4,
        RightWall = 5,
        BottomWall = 6,
        TopLeftWall = 7,
        RightLeftWall = 8,
        BottomLeftWall = 9,
        TopRightWall = 10,
        TopBottomWall = 11,    
        RightBottomWall = 12,
        TopRightLeftWall = 13,
        TopBottomLeftWall = 14,
        LeftBottomRight = 15,
        TopRightBottomWall = 16,     
        FullWall = 17
    };

    private List<Vector2> outerFloorTilesPostitions = new List<Vector2>();
    private List<Vector2> innerFloorTilesPostitions = new List<Vector2>();
    private Vector2 startPosition = new Vector2();
    private Vector2 exitPosition = new Vector2();
    private int size = 0;

    public int boardWidth;
    public int boardHeight;

    public GameObject start;
    public GameObject exit;
    public GameObject chest;
    public GameObject torch;

    public GameObject[] floorTiles;

    public GameObject[] wall0Tiles;
    public GameObject[] wall3Tiles;
    public GameObject[] wall4Tiles;
    public GameObject[] wall5Tiles;
    public GameObject[] wall6Tiles;
    public GameObject[] wall7Tiles;
    public GameObject[] wall8Tiles;
    public GameObject[] wall9Tiles;
    public GameObject[] wall10Tiles;
    public GameObject[] wall11Tiles;
    public GameObject[] wall12Tiles;
    public GameObject[] wall13Tiles;
    public GameObject[] wall14Tiles;
    public GameObject[] wall15Tiles;
    public GameObject[] wall16Tiles;
    public GameObject[] wall17Tiles;

    public GameObject[] enemies;
    public GameObject[] objects;

    public static BoardFields[,] board;
    public static char[][] obstacles;


    private void Awake()
    {
        board = new BoardFields[boardWidth, boardHeight];
        obstacles = new char[boardWidth][];
    }


    private void RoomSize(int x, int y, int roomNumber)
    {
        if (board[x, y] == BoardFields.TMP)
        {
            size++;
            board[x, y] = (BoardFields)roomNumber;

            RoomSize(x - 1, y, roomNumber);
            RoomSize(x, y + 1, roomNumber);
            RoomSize(x + 1, y, roomNumber);
            RoomSize(x, y - 1, roomNumber);
        }
        else
            return;
    }


    private void ClearBoard()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                board[x, y] = BoardFields.Wall;
            }
        }
    }

    private void GenerateBoard()
    {
        int x = 0;
        int y = 0;

        int translationX = 10;
        List<int> translationY = new List<int>();

        int column = 0;
        int row = 0;

        while (true)
        {
            int beginningOfX = translationX + Random.Range(-2, 2) + 1;
            int beginningOfY;

            int endOfX = translationX + Random.Range(3, 5);
            int endOfY;

            if (row == 0)
            {
                beginningOfY = Random.Range(-2, 2) + 11;
                endOfY = Random.Range(3, 6) + 10;
            }
            else
            {
                beginningOfY = translationY[column] + Random.Range(-2, 2) + 1;
                endOfY = translationY[column] + Random.Range(3, 6);
            }

            if (endOfY > boardHeight - 10)
            {
                break;
            }

            if (endOfX > boardWidth - 10)
            {
                for (int a = 0; a < 10; a++)
                {
                    translationY.Add(y);
                }

                translationX = 10;
                column = 0;
                row++;
                continue;
            }

            for (x = beginningOfX; x < endOfX; x++)
            {
                for (y = beginningOfY; y < endOfY; y++)
                {
                    board[x, y] = BoardFields.TMP;
                }
            }

            translationX = x;

            if (row == 0)
                translationY.Add(y);
            else
                translationY[column] = y;

            column++;
        }
    }

    private void ImproveBoard()
    {
        int roomNumber = 1;
        Room mainRoom = new Room(0, 0);
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                RoomSize(x, y, roomNumber + 1);

                if (size != 0)
                {
                    if (size > mainRoom.size) mainRoom = new Room(roomNumber, size);
                    size = 0;
                    roomNumber++;
                }
            }
        }

        if (mainRoom.size < ((boardHeight * boardWidth) / 4))
        {
            ClearBoard();
            GenerateBoard();
            ImproveBoard();
            return;
        }

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[x, y] == BoardFields.Wall)
                    continue;
                else if (board[x, y] == (BoardFields)(mainRoom.roomNumber + 1))
                    board[x, y] = BoardFields.Floor;
                else
                    board[x, y] = BoardFields.Wall;
            }
        }
    }

    private void FindOuterFloorTiles()
    {
        for (int x = 1; x < boardWidth - 1; x++)
        {
            for (int y = 1; y < boardHeight - 1; y++)
            {
                if (board[x, y] != BoardFields.Floor)
                    continue;
                else
                {
                    if (board[x - 1, y] == BoardFields.Wall ||
                        board[x, y + 1] == BoardFields.Wall ||
                        board[x + 1, y] == BoardFields.Wall ||
                        board[x, y - 1] == BoardFields.Wall)
                    {
                        outerFloorTilesPostitions.Add(new Vector2(x, y));
                    }
                }
            }
        }
    }

    private void FindInnerFloorTiles()
    {
        List<Vector2> allFloorTilesPostitions = new List<Vector2>();

        for (int x = 1; x < boardWidth - 1; x++)
        {
            for (int y = 1; y < boardHeight - 1; y++)
            {
                if (board[x, y] != BoardFields.Floor)
                    continue;
                else
                    allFloorTilesPostitions.Add(new Vector2(x, y));
            }
        }

        innerFloorTilesPostitions = allFloorTilesPostitions.Except(outerFloorTilesPostitions).ToList();
    }

    private void FindWalls()
    {
        for (int x = 1; x < boardWidth - 1; x++)
        {
            for (int y = 1; y < boardHeight - 1; y++)
            {
                if (board[x, y] != BoardFields.Wall)
                    continue;
                else
                {
                    //0
                    if (board[x - 1, y] != BoardFields.Floor &&
                        board[x, y + 1] != BoardFields.Floor &&
                        board[x + 1, y] != BoardFields.Floor &&
                        board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = 0;
                    }

                    //1
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.LeftWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.RightWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.BottomWall;
                    }

                    //2
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopLeftWall;
                    }
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.RightLeftWall;
                    }
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.BottomLeftWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopRightWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopBottomWall;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.RightBottomWall;
                    }

                    //3
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] != BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopRightLeftWall;
                    }
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] != BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopBottomLeftWall;
                    }
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] != BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.LeftBottomRight;
                    }
                    else if (board[x - 1, y] != BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.TopRightBottomWall;
                    }

                    //4
                    else if (board[x - 1, y] == BoardFields.Floor &&
                             board[x, y + 1] == BoardFields.Floor &&
                             board[x + 1, y] == BoardFields.Floor &&
                             board[x, y - 1] == BoardFields.Floor)
                    {
                        board[x, y] = BoardFields.FullWall;
                    }
                }
            }
        }
    }

    private void FindObstacles()
    {
        for (int x = 0; x < boardWidth; x++)
            obstacles[x] = new char[boardWidth];

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                if (board[x, y] == BoardFields.Floor)
                    obstacles[x][y] = '-';
                else
                    obstacles[x][y] = 'X';
            }
        }
    }

    private void DrawBoard()
    {
        Transform boardHolder = new GameObject("Board").transform;
        GameObject instance = null;

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector2 position = new Vector2(x + 0.5f, y + 0.5f);

                if (board[x, y] == BoardFields.Floor)
                {
                    GameObject tileChoice = floorTiles[Random.Range(0, floorTiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == 0)
                {
                    GameObject tileChoice = wall0Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.LeftWall)
                {
                    GameObject tileChoice = wall3Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopWall)
                {
                    GameObject tileChoice = wall4Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.RightWall)
                {
                    GameObject tileChoice = wall5Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.BottomWall)
                {
                    GameObject tileChoice = wall6Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopLeftWall)
                {
                    GameObject tileChoice = wall7Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.RightLeftWall)
                {
                    GameObject tileChoice = wall8Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.BottomLeftWall)
                {
                    GameObject tileChoice = wall9Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopRightWall)
                {
                    GameObject tileChoice = wall10Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopBottomWall)
                {
                    GameObject tileChoice = wall11Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.RightBottomWall)
                {
                    GameObject tileChoice = wall12Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopRightLeftWall)
                {
                    GameObject tileChoice = wall13Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopBottomLeftWall)
                {
                    GameObject tileChoice = wall14Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.LeftBottomRight)
                {
                    GameObject tileChoice = wall15Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.TopRightBottomWall)
                {
                    GameObject tileChoice = wall16Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }
                else if (board[x, y] == BoardFields.FullWall)
                {
                    GameObject tileChoice = wall17Tiles[Random.Range(0, wall0Tiles.Length)];
                    instance = Instantiate(tileChoice, position, Quaternion.identity) as GameObject;
                }

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    private void LayoutObjects()
    {
        Transform objectsHolder = new GameObject("Objects").transform;
        GameObject instance = null;
        List<Vector2> innerFloorTilesPostitionsCopy = new List<Vector2>(innerFloorTilesPostitions.ToArray());

        int randomIndex = Random.Range(0, innerFloorTilesPostitions.Count / 4);
        startPosition = innerFloorTilesPostitions[randomIndex];
        innerFloorTilesPostitions.RemoveAt(randomIndex);

        instance = Instantiate(start, new Vector2(startPosition.x + 0.5f, startPosition.y + 0.5f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(objectsHolder);

        randomIndex = Random.Range(innerFloorTilesPostitions.Count - innerFloorTilesPostitions.Count / 4, innerFloorTilesPostitions.Count);
        exitPosition = innerFloorTilesPostitions[randomIndex];
        innerFloorTilesPostitions.RemoveAt(randomIndex);

        instance = Instantiate(exit, new Vector2(exitPosition.x + 0.5f, exitPosition.y + 0.5f), Quaternion.identity) as GameObject;
        instance.transform.SetParent(objectsHolder);

        int count = Random.Range(5, 8);
        while (count-- > 0)
        {
            randomIndex = Random.Range(0, outerFloorTilesPostitions.Count);
            Vector2 chestPosition = outerFloorTilesPostitions[randomIndex];
            outerFloorTilesPostitions.RemoveAt(randomIndex);

            instance = Instantiate(chest, new Vector2(chestPosition.x + 0.5f, chestPosition.y + 0.5f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(objectsHolder);
        }

        count = Random.Range(16, 21);
        while (count-- > 0 || outerFloorTilesPostitions.Count <= 0)
        {
            randomIndex = Random.Range(0, outerFloorTilesPostitions.Count);
            Vector2 torchPosition = outerFloorTilesPostitions[randomIndex];
            outerFloorTilesPostitions.RemoveAt(randomIndex);

            for (int i = (int)torchPosition.x - 7; i <= (int)torchPosition.x + 7; i++)
            {
                for (int j = (int)torchPosition.y - 7; j <= (int)torchPosition.y + 7; j++)
                {
                    Vector2 tmp = new Vector2(i, j);

                    if (outerFloorTilesPostitions.Contains(tmp))
                        outerFloorTilesPostitions.Remove(tmp);
                }
            }

            instance = Instantiate(torch, new Vector2(torchPosition.x + 0.5f, torchPosition.y + 0.5f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(objectsHolder);
        }

        count = Random.Range(innerFloorTilesPostitions.Count / 9, innerFloorTilesPostitions.Count / 5);
        while (count-- > 0 || innerFloorTilesPostitions.Count <= 0)
        {
            randomIndex = Random.Range(0, innerFloorTilesPostitions.Count);
            Vector2 itemPosition = innerFloorTilesPostitions[randomIndex];
            innerFloorTilesPostitions.RemoveAt(randomIndex);

            for (int i = (int)itemPosition.x - 1; i <= (int)itemPosition.x + 1; i++)
            {
                for (int j = (int)itemPosition.y - 1; j <= (int)itemPosition.y + 1; j++)
                {
                    Vector2 tmp = new Vector2(i, j);

                    if (innerFloorTilesPostitions.Contains(tmp))
                        innerFloorTilesPostitions.Remove(tmp);
                }
            }

            instance = Instantiate(objects[Random.Range(0, objects.Length)], new Vector2(itemPosition.x + 0.5f, itemPosition.y + 0.5f), Quaternion.identity) as GameObject;
            instance.transform.SetParent(objectsHolder);
        }


        innerFloorTilesPostitions = innerFloorTilesPostitionsCopy;
    }

    private void CheckArea(int x, int y, ref int enemiesCount, Transform enemiesHolder)
    {
        int count = 0;

        for (int i = x - 10; i <= x + 10; i++)
        {
            for (int j = y - 9; j <= y + 9; j++)
            {
                Vector2 potentialEnemyPosition = new Vector2(i, j);

                if (i >= x - 4 && i < x + 4 && j >= y - 4 && j < y + 4)
                {
                    if (innerFloorTilesPostitions.Contains(potentialEnemyPosition))
                    {
                        int chance = Random.Range(0, 100);
                        if (chance >= 0 && chance < 30)
                        {
                            if (count < 6)
                            {
                                enemiesCount++;
                                count++;

                                GameObject randomedEnemy = enemies[Random.Range(0, enemies.Length)];

                                GameObject instance = Instantiate(randomedEnemy, new Vector2(i + 0.5f, j + 0.5f), Quaternion.identity) as GameObject;
                                instance.transform.SetParent(enemiesHolder);
                            }
                        }
                    }
                }

                if (innerFloorTilesPostitions.Contains(potentialEnemyPosition))
                    innerFloorTilesPostitions.Remove(potentialEnemyPosition);
            }
        }
    }

    private void LayoutPlayerAndEnemies()
    {
        int enemiesCount = 0;
        int randomedPosition;

        Transform enemiesHolder = new GameObject("Enemies").transform;
        Player.Instance.transform.position = new Vector2(startPosition.x + 0.5f, startPosition.y + 0.5f);

        for (int i = (int)startPosition.x - 7; i < (int)startPosition.x + 7; i++)
        {
            for (int j = (int)startPosition.y - 7; j < (int)startPosition.y + 7; j++)
            {
                Vector2 toRemove = new Vector2(i, j);

                if (innerFloorTilesPostitions.Contains(toRemove))
                    innerFloorTilesPostitions.Remove(toRemove);
            }
        }

        while (enemiesCount < 100 && innerFloorTilesPostitions.Count > 0)
        {
            randomedPosition = Random.Range(0, innerFloorTilesPostitions.Count);

            int x = (int)innerFloorTilesPostitions[randomedPosition].x;
            int y = (int)innerFloorTilesPostitions[randomedPosition].y;

            CheckArea(x, y, ref enemiesCount, enemiesHolder);
        }
    }


    public void SetupScene()
    {
        ClearBoard();

        GenerateBoard();

        ImproveBoard();

        FindOuterFloorTiles();

        FindInnerFloorTiles();

        FindWalls();

        FindObstacles();

        DrawBoard();

        LayoutObjects();

        LayoutPlayerAndEnemies();
    }
}