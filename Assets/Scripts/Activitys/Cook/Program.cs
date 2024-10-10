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
        // 年齢とチケット数を宣言
        AdmissionData admissionData = new AdmissionData(65, 1);

        if (!admissionData.ExistTickets)
        {
           // Console.WriteLine("入場できません");
            return;
        }

        if (admissionData.Age < 12)
        {
            Debug.Log("子供料金で入場できます");
        }
        if (admissionData.Age >= 12 && admissionData.Age < 18)
        {
            Debug.Log("ジュニア料金で入場できます");
        }
        if (admissionData.Age >= 18 && admissionData.Age < 60)
        {
            Debug.Log("通常料金で入場できます");
        }
        if (admissionData.Age >= 60)
        {
            Debug.Log("シニア割引を適用して入場できます");
        }

    }
}
