using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDependencyInjector<T> where T : DependencyInformation
{
    void Inject(T information);
}
public interface IDependencyProvider<T> where T : DependencyInformation
{
    T Information { get; }
}

public class DependencyInformation
{

}
