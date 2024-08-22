using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInject<T>
{
    void Inject(T t);
}
