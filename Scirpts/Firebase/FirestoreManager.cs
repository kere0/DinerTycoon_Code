using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;
using UnityEngine;

public class FirestoreManager
{
    private FirebaseApp customApp;
    public FirebaseFirestore firestore;

    private bool isInit = false;

    public async void Init()
    {
        string appName = "CustomApp_" + Guid.NewGuid();
        await InitializeAsync(appName);
    }
    private async Task InitializeAsync(string appName)
    {
        if (isInit == true) return;

        var options = new AppOptions()
        {
            ProjectId = "",
            AppId = "",
            ApiKey = ""
        };
        customApp = FirebaseApp.Create(options, appName);
        firestore = FirebaseFirestore.GetInstance(customApp);
        
        isInit = true;
    }
}
