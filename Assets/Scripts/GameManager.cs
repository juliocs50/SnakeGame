using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager classe;
    public GameObject FruitPrefab;
    public GameObject BodyPrefab;
    public GameObject InformWinTextObject;
    private List<Vector2Int> snakeMovePositionList;
    public static List<Vector2Int> AvailablePositions;
    private Vector2Int LocationForFruit;
    private Vector2Int SnakeMovementDirection;
    private Vector2Int SnakeLocation;
    private float SnakeMovementTime;
    public float SnakeMaximumMovementTime;
    private int GridWidth = 4;
    private int GridHeight = 4;
    private int MaximumSnakeSize;
    public static int SnakeBodySize;
    public static bool GameWin;
    

    private void Awake()
    {
        InformWinTextObject.SetActive(false);
        GameWin = false;
        MaximumSnakeSize = ((GridWidth * 2) + 1) * ((GridHeight * 2) + 1);

        classe = this;
        AvailablePositions = new List<Vector2Int>();
        //Cria uma lista de posições para criar a fruta.
        for (int i = -GridWidth; i < GridWidth + 1; i++)
        {
            for (int j = -GridHeight; j < GridHeight + 1; j++)
            {
                AvailablePositions.Insert(0, new Vector2Int(i, j));
            }
        }
    }
    void Start()
    {
        //Remove a posição inicial.
        AvailablePositions.Remove(new Vector2Int(0, 0));
        //A partida sempre inicia com uma comida em alguma posição aleatória do level.       
        GenerateFruit();   

        SnakeLocation = new Vector2Int(0, 0);
        SnakeMaximumMovementTime = 1f;
        SnakeMovementTime = SnakeMaximumMovementTime;
        SnakeMovementDirection = new Vector2Int(1, 0);
        snakeMovePositionList = new List<Vector2Int>();
        SnakeBodySize = 0;
       
    }
    void Update()
    {
        if (SnakeBodySize >= MaximumSnakeSize)
        {
            GameWin = true;
            InformWinTextObject.SetActive(true);
        }
        if (Input.anyKey && GameWin)
        {
            SceneManager.LoadScene(0);
        }
    }
    void LateUpdate()
    {
        if (!GameWin)
        {
            InputKeys();
            PlayerMotion();
        }

    }

    public void CollisionEnterDetected(CollisionDetecter CollisionDetecter)
    {
        //Fim de Jogo quando a cobra colidir com uma parede ou com ela mesma
        if ((CollisionDetecter.gameObject.CompareTag("wall")) || (CollisionDetecter.gameObject.CompareTag("SnakeBody")))
        {
            SceneManager.LoadScene(0);
        }
        
        //Ao colidir com a cobra 
        if (CollisionDetecter.gameObject.CompareTag("food"))
        {
            //Aumentando o tamanho da cobra.
            SnakeBodySize++;
            //A velocidade da cobra deve aumentar progressivamente de acordo com o consumo de comidas.
            SnakeMaximumMovementTime = 1.0f - (SnakeBodySize / 100.0f);
            //Nova comida é criada em uma nova posição aleatória.           
            classe.GenerateFruit();
            //Comida  é removida do level.
            Destroy(CollisionDetecter.gameObject);
        }

    }
   
    private void PlayerMotion()
    {
        //Controla o tempo de movimentação 
        SnakeMovementTime += Time.deltaTime;

        if (SnakeMovementTime >= SnakeMaximumMovementTime)
        {
            SnakeMovementTime -= SnakeMaximumMovementTime;
            snakeMovePositionList.Insert(0, SnakeLocation);
            SnakeLocation += SnakeMovementDirection;
            if (snakeMovePositionList.Count >= SnakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            //Cria o corpo da Snake
            for (int i = 0; i < snakeMovePositionList.Count; i++)
            {
                Vector2Int posicao = snakeMovePositionList[i];
                var VarSnakeBody = Instantiate(BodyPrefab, new Vector3(posicao.x, posicao.y, 0), Quaternion.identity);
                Destroy(VarSnakeBody, SnakeMaximumMovementTime);
            }

            //Libera a última posição percorrida pela Snake,
            //depois disso a liberação é controlado pelo ultimo corpo inserido no script "SnakeBody" 
            if (SnakeBodySize == 0)
            {
                AvailablePositions.Insert(0, new Vector2Int((int)transform.position.x, (int)transform.position.y));
            }
            //Move a Snake um passo
            transform.GetChild(0).transform.position = new Vector3(SnakeLocation.x, SnakeLocation.y, 0);
            //Remove a posição que o corpo esta da lista de opções para fruta
            AvailablePositions.Remove(new Vector2Int(SnakeLocation.x, SnakeLocation.y));
            //Rotaciona a cabeça da Snake conforme a direção
            transform.GetChild(0).transform.eulerAngles = new Vector3(0, 0, GetAngle(SnakeMovementDirection));
        }


    }
    private float GetAngle(Vector2Int direction)
    {
        float angle = 0;
        if (direction.x == 1)
        {
            angle = 0;
        }
        if (direction.x == -1)
        {
            angle = 180;
        }
        if (direction.y == -1)
        {
            angle = 270;
        }
        if (direction.y == 1)
        {
            angle = 90;
        }
        return angle;
    }
    private void InputKeys()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //verifica se não esta indo para baixo
            if (SnakeMovementDirection.y != -1)
            {
                SnakeMovementDirection.x = 0;
                SnakeMovementDirection.y = 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //verifica se não esta indo para cima
            if (SnakeMovementDirection.y != 1)
            {
                SnakeMovementDirection.x = 0;
                SnakeMovementDirection.y = -1;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //verifica se não esta indo para direita
            if (SnakeMovementDirection.x != 1)
            {
                SnakeMovementDirection.x = -1;
                SnakeMovementDirection.y = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //verifica se não esta indo para direita
            if (SnakeMovementDirection.x != -1)
            {
                SnakeMovementDirection.x = 1;
                SnakeMovementDirection.y = 0;
            }
        }

    }
    
   
    public void GenerateFruit()
    {
        //Escolher um indice da lista de posições disponiveis.
        //As comidas não podem aparecer dentro da cobra.
        LocationForFruit = AvailablePositions[Random.Range(0, AvailablePositions.Count)];
        Instantiate(FruitPrefab, new Vector3(LocationForFruit.x, LocationForFruit.y, 0), Quaternion.identity);
    }
}
