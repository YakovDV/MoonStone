using UnityEngine;

public interface IAttractable
{
    ResourceConfig Config { get; }

    public void SetTarget(Transform target);
}

public interface IResource
{
    ResourceConfig Config { get; }
}