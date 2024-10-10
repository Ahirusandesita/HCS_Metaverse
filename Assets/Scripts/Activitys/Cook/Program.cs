using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    public struct AdmissionData
    {
        public readonly int Age;
        private int tickets;
        public int Tickets => tickets;
        public AdmissionData(int age, int tickets)
        {
            this.Age = age;
            this.tickets = tickets;
        }
        public bool ExistTickets => tickets >= 1;
    }
    private const int FEE_CHILD = 12;
    private const int FEE_JUNIOR = 18;
    private const int FEE_SENIOR = 60; 
    static void Main(string[] args)
    {
        // �N��ƃ`�P�b�g����錾
        AdmissionData admissionData = new AdmissionData(65, 1);

        if (!admissionData.ExistTickets)
        {
           // Console.WriteLine("����ł��܂���");
            return;
        }

        if (admissionData.Age < 12)
        {
            Debug.Log("�q�������œ���ł��܂�");
        }
        if (admissionData.Age >= 12 && admissionData.Age < 18)
        {
            Debug.Log("�W���j�A�����œ���ł��܂�");
        }
        if (admissionData.Age >= 18 && admissionData.Age < 60)
        {
            Debug.Log("�ʏ헿���œ���ł��܂�");
        }
        if (admissionData.Age >= 60)
        {
            Debug.Log("�V�j�A������K�p���ē���ł��܂�");
        }

    }
}
