namespace GuruRefactoringUnitTest;
using System;

public interface ITransporter
{
    void Deliver();
}


public class Ship : ITransporter
{

    public void Deliver()
    {
        Console.WriteLine("ship");
    }

}


public class Truck : ITransporter
{

    public void Deliver()
    {
        Console.WriteLine("truck");
    }

}


[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
    }
}