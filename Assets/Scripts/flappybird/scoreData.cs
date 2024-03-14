using Firebase.Firestore;
using UnityEngine;

[FirestoreData]

public struct scoreData
{
    [FirestoreProperty]

    public float Score { get; set;}
}