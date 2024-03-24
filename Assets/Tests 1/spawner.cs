using System.Collections.Generic;
using NUnit.Framework;

public class SpawnerTests
{
    [Test]
    public void Spawn_CreatesObjectWithinSpawnChance()
    {
        // Arrange
        var spawner = new MockSpawner();
        spawner.objects = new[]
        {
            new SpawnableObject { spawnChance = 1f },
            new SpawnableObject { spawnChance = 0f }
        };
        
        // Act
        spawner.Spawn();
        
        // Assert
        Assert.AreEqual(1, spawner.InstantiatedObjects.Count);
    }

    [Test]
    public void Spawn_DoesNotCreateObjectOutsideSpawnChance()
    {
        // Arrange
        var spawner = new MockSpawner();
        spawner.objects = new[]
        {
            new SpawnableObject { spawnChance = 0f }
        };
        
        // Act
        spawner.Spawn();
        
        // Assert
        Assert.AreEqual(0, spawner.InstantiatedObjects.Count);
    }

    [Test]
    public void OnDisable_CancelInvoke()
    {
        // Arrange
        var spawner = new MockSpawner();
        
        // Act
        spawner.OnDisable();
        
        // Assert
        Assert.IsTrue(spawner.InvokeCancelled);
    }
}

public class MockSpawner
{
    public SpawnableObject[] objects;
    public List<SpawnedObjectInfo> InstantiatedObjects { get; } = new List<SpawnedObjectInfo>();
    public bool InvokeCancelled { get; private set; }

    public void Spawn()
    {
        if (objects == null || objects.Length == 0) return;

        float spawnChance = UnityEngine.Random.value;

        foreach (var obj in objects)
        {
            if (spawnChance < obj.spawnChance)
            {
                InstantiatedObjects.Add(new SpawnedObjectInfo()); // Simulate object creation
                break;
            }

            spawnChance -= obj.spawnChance;
        }
    }

    public void OnDisable()
    {
        InvokeCancelled = true;
    }
}

public struct SpawnableObject
{
    // Here you can define properties like prefabPath or other info related to spawning
    public float spawnChance;
}

// Define a struct to store information about spawned objects if needed
public struct SpawnedObjectInfo
{
    // You can add properties to store information about the spawned object
}
