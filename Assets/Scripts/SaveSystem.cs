using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    // Declaring the data of the function
    public static Data data;

    static string dataFile = "6e6f726d616c2064617461.dat";

    // Start is called before the first frame update
    /*void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        } else if(instance != this)
        {
            Destroy(this.gameObject);
        }

    }*/

    public static void Save()
    {
        string filePath = Application.persistentDataPath + "/" + dataFile;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        bf.Serialize(file, data);
        file.Close();
    }
    
    public static void Load()
    {
        string filePath = Application.persistentDataPath + "/" + dataFile;
        BinaryFormatter bf = new BinaryFormatter();
        if (File.Exists(filePath))
        {
            FileStream file = File.Open(filePath, FileMode.Open);
            Data loaded = (Data)bf.Deserialize(file);
            data = loaded;
            file.Close();
        }
        else
        {
            Debug.LogError("Save file not found in" + filePath);
        }
    }
}

[System.Serializable]
public class Data
{
    Health nyawa;
    Currency duit;

    // Player position called to here
    public Point pos;

    public int health = 0;
    public int gold = 0;

    public Data()
    {
        health = nyawa.health;
        gold = duit.gold;
    }
}

// Data saving for player's position
[System.Serializable]
public class Point
{
    public float x;
    public float y;
    public float z;

    public Point()
    {
        GameObject thePlayer = GameObject.Find("Player");
        Player player = thePlayer.GetComponent<Player>();
        x = player.transform.position.x;
        y = player.transform.position.y;
        z = player.transform.position.z;
    }

    public Vector3 toVector()
    {
        return new Vector3(x, y, z);
    }
}
