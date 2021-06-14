using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class block : MonoBehaviour


{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    public static Transform[,] grid = new Transform[width, height];
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A)) //sol
        {

            transform.position += new Vector3(-1, 0, 0);
            if (!ValidMove()) { transform.position -= new Vector3(-1, 0, 0); }

        }
        else if (Input.GetKeyDown(KeyCode.D))//sag
        {

            transform.position += new Vector3(1, 0, 0);
            if (!ValidMove()) { transform.position -= new Vector3(1, 0, 0); }

        }
        else if (Input.GetKeyDown(KeyCode.W))// dondur
        {

            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
            if (!ValidMove()) { transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90); }
        }

        //asagi
        if ((Time.time - previousTime) > (Input.GetKey(KeyCode.S) ? fallTime / 10 : fallTime))
        {

            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                add();
                Check();
                this.enabled = false;
                FindObjectOfType<Spawner>().Spawn();
            }
            previousTime = Time.time;
        }
    }

    bool ValidMove() //alanin icinde mi bakiyor
    {

        foreach (Transform children in transform)
        { //tum cocuklara bak

            int roundedX = Mathf.RoundToInt(children.transform.position.x); //x kordinatini al
            int roundedY = Mathf.RoundToInt(children.transform.position.y);//y kordinatini al

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            { //bizim ekrandan büyükse false ver
                return false;
            }

            if (grid[roundedX, roundedY] != null) {
                return false;
            }
        }

        return true; //büyük değilse true ver
    }

    void add() //gride ekliyor
    { 

        foreach (Transform children in transform)
        { //tum cocuklara bak

            int roundedX = Mathf.RoundToInt(children.transform.position.x); //x kordinatini al
            int roundedY = Mathf.RoundToInt(children.transform.position.y);//y kordinatini al

            grid[roundedX, roundedY] = children;
        }
    }

    bool Line(int i) //Tam satır var mı kontrol
    {
        for (int j = 0; j < width; j++) //soldan sağa satırda boşluk var mı diye kontrol
        {
            if (grid[j, i] == null) { //boşluk varsa false
                return false;
            }
        }
        return true; //yoksa true
    }

    void Delete(int i) //tam satır varsa sil
    {
        for (int j = 0; j < width; j++) //soldan sağa satırı sil
        {
            Destroy(grid[j, i].gameObject); //objeyi yok et
            grid[j, i] = null; //yerine boşluk koy
        }
    }

    void Down(int i) //silindikten sonra üsttekileri alt satıra geçir
    {
        for (int k = i; k < height; k++) //aşağıdan yukarı tüm satırlara bak
        {
            for (int j = 0; j < width; j++) //sağdan sola bak
            {
                if (grid[j, k] != null) //eğer boşluk varsa
                {
                    grid[j, k - 1] = grid[j, k];
                    grid[j, k] = null;
                    grid[j, k - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    } 


    void Check() //Yukardan aşağı her satırda temizlenmesi gereken bir şey var mı diye bak, varsa temizle
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (Line(i)) {
                Delete(i);
                Down(i);
            }  
        }
    }
}
