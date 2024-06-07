using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameZone : IInteraction, ISelectedNotification
{
    public ISelectedNotification SelectedNotification => this;

    public void Close()
    {
        throw new System.NotImplementedException();
    }

    public void Open()
    {
        throw new System.NotImplementedException();
    }

    public void Select(SelectArgs selectArgs)
    {
        throw new System.NotImplementedException();
    }

    public void Unselect(SelectArgs selectArgs)
    {
        throw new System.NotImplementedException();
    }
}
