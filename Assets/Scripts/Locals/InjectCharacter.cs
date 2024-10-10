using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Layer_lab._3D_Casual_Character;
public class InjectCharacter : MonoBehaviour
{
    [SerializeField]
    private CharacterControl characterControl;
    [SerializeField]
    private CharacterBase characterBase;
    void Start()
    {
        FindObjectOfType<LocalCharacterControl>().InjectCanvasCharacterControl(characterControl);
        FindObjectOfType<LocalCharcterBase>().CanvasCharacterBaseInject(characterBase);
    }
}
