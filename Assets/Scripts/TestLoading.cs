using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLoading : MonoBehaviour
{

    // Start is called before the first frame update
    private void Save()
    {
        SaveSystem.Save();
    }

    public void Load()
    {
        SaveSystem.Load();

        Vector3 position;
        position = SaveSystem.data.pos.toVector();
        transform.position = position;
    }
}
