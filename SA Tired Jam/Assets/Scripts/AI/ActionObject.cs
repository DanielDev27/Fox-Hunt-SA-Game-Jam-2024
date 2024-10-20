using System.Collections.Generic;
using UnityEngine;

public abstract class ActionObject : ScriptableObject
{
    [HideInInspector] public DogActions dogActions;

    [Header("Debug")]
    public List<DogActions> otherDogs = new List<DogActions>();

    public abstract void Awake();

    public abstract void OnEnable();

    public abstract void OnDisable();

    public abstract void Start();

    public abstract void Update();

    public abstract void FixedUpdate();

    public abstract void OnDrawGizmos();
}
