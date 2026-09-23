using UnityEngine;

public interface IResourceReceiver
{
    Transform ReceivePoint { get; }
    bool Receive(IStorage storage);
}