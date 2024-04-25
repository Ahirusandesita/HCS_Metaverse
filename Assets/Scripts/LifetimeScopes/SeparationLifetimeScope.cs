using VContainer;
using VContainer.Unity;
using UnityEngine;

public class SeparationLifetimeScope : LifetimeScope
{
    private RemoteView remoteView;
    public LifetimeScope SeparationSetup(GameObject localGameObject, RemoteView remoteView)
    {
        autoInjectGameObjects.Add(localGameObject);
        this.remoteView = remoteView;

        return this;
    }


    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent<RemoteView>(remoteView);
    }
}
