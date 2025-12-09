using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager
{
    public SaveSnapshot.SaveData saveData = new SaveSnapshot.SaveData();
    public async Task SaveToFirebase()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").
            Document(userId).Collection("saveData").Document("data");
        await doRef.SetAsync(saveData);
    }
    public async Task LoadAll(Action OnComplete = null)
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DocumentReference doRef = Managers.FirestoreManager.firestore.Collection("users").
            Document(userId).Collection("saveData").Document("data");
        DocumentSnapshot snapshot = await doRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            saveData = snapshot.ConvertTo<SaveSnapshot.SaveData>();
        }
        else
        {
            saveData.money = Managers.DataManager.Get<UserData>("P0").Money;
            saveData.employeeCount = Managers.DataManager.Get<UserData>("P0").EmployeeCount;
            saveData.increaseProfit = Managers.DataManager.Get<UserData>("P0").IncreaseProfit;

            saveData.playerSpeed = Managers.DataManager.Get<CharacterData>("P").Speed;
            saveData.playerCapacity = Managers.DataManager.Get<CharacterData>("P").Capacity;

            saveData.customerSpeed = Managers.DataManager.Get<CharacterData>("C").Speed;
            saveData.customerCapacity = Managers.DataManager.Get<CharacterData>("C").Capacity;

            saveData.employeeSpeed = Managers.DataManager.Get<CharacterData>("E").Speed;
            saveData.employeeCapacity = Managers.DataManager.Get<CharacterData>("E").Capacity;
            saveData.activeTableCount = 0;
        }
        OnComplete?.Invoke();
    }
}
