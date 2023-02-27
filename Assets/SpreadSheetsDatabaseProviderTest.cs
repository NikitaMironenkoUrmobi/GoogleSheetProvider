using System.Collections;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Examples;
using Redpenguin.GoogleSheets.Provider;
using UnityEngine;

public class SpreadSheetsDatabaseProviderTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var provider = new SpreadSheetsDatabaseProvider();
        provider.Load();
        Debug.Log(provider.Database.GetSpreadSheetData<ExampleData>()[0].myString);;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
