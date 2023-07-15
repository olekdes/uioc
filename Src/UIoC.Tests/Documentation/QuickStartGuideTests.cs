﻿using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIoC.Tests.Documentation {
  [TestClass]
  public class QuickStartGuideTests {

    private interface IMyType {
      public string Text { get; }
    }

    private class MyType : IMyType {
      public string Text { get; }
      public MyType(string text = null) => Text = text;
    }

    private class MyTypeA : IMyType {
      public string Text { get; }
      public MyTypeA(string text = null) => Text = text;
    }

    private class MyTypeB : IMyType {
      public string Text { get; }
      public MyTypeB(string text = null) => Text = text;
    }

    private class MyOuterTypeA {
      public object Obj { get; }
      public MyOuterTypeA(MyType myType) => Obj = myType;
    }

    private class MyOuterTypeB {
      public object Obj { get; }
      public MyOuterTypeB([ResolveName("keyB")] IMyType myType) => Obj = myType;
    }

    // Specify resolve type
    [Type(typeof(IMyType))] 
    private class MyTypeC : IMyType {
      public string Text => throw new NotImplementedException();
    }

    // Specify resolve type and resolve name
    [Singleton(typeof(IMyType), "MySingleton")] 
    private class MyTypeD : IMyType {
      public string Text => throw new NotImplementedException();
    }

    class MyUnregstredType {
      public object Obj { get; }
      public MyUnregstredType(IMyType myType) => Obj = myType;
    }

    /// <summary>
    /// Add type with interface
    /// </summary>
    [TestMethod]
    public void Example01() {
      var container = new Container();

      // Add type
      container.AddType<IMyType, MyType>();

      // Get instance of registered type
      var myType = container.Get<IMyType>();

      Assert.IsNotNull(myType);
      Assert.AreEqual(myType.GetType().FullName, typeof(MyType).FullName);
    }

    /// <summary>
    /// Add type with resolve name 
    /// </summary>
    [TestMethod]
    public void Example02() {
      var container = new Container();

      // Add type
      container.AddType<IMyType, MyTypeA>("keyA");
      container.AddType<IMyType, MyTypeB>("keyB");

      // Get instance of registered type by name
      var myTypeA = container.Get<IMyType>("keyA");
      var myTypeB = container.Get<IMyType>("keyB");

      Assert.IsNotNull(myTypeA);
      Assert.AreEqual(myTypeA.GetType().FullName, typeof(MyTypeA).FullName);
      Assert.IsNotNull(myTypeB);
      Assert.AreEqual(myTypeB.GetType().FullName, typeof(MyTypeB).FullName);
    }

    /// <summary>
    /// Add type without interface
    /// </summary>
    [TestMethod]
    public void Example03() {
      var container = new Container();

      // Add type
      container.AddType<MyType>();

      // Get an instance of the registered type
      var myType = container.Get<MyType>();

      Assert.IsNotNull(myType);
      Assert.AreEqual(myType.GetType().FullName, typeof(MyType).FullName);
    }

    /// <summary>
    /// Add Singleton
    /// </summary>
    [TestMethod]
    public void Example04() {
      var container = new Container();

      // Add singleton
      container.AddSingleton<IMyType, MyType>();

      // Get the instance of the singleton
      var myType = container.Get<IMyType>();

      Assert.IsNotNull(myType);
      Assert.AreEqual(myType.GetType().FullName, typeof(MyType).FullName);
      Assert.AreEqual(myType, container.Get<IMyType>());
    }

    /// <summary>
    /// Add Instance
    /// </summary>
    [TestMethod]
    public void Example05() {
      var container = new Container();

      // Add instance
      container.AddInstance<IMyType>(new MyType("My Instance Type"));

      // Get the instance 
      var myType = container.Get<IMyType>();

      Assert.IsNotNull(myType);
      Assert.AreEqual(myType.GetType().FullName, typeof(MyType).FullName);
      Assert.AreEqual(myType.Text, "My Instance Type");
      Assert.ReferenceEquals(myType, container.Get<IMyType>());
    }

    /// <summary>
    /// Add Factory
    /// </summary>
    [TestMethod]
    public void Example06() {
      var container = new Container();

      // Add factory
      container.AddFactory<IMyType>(() => new MyType("My Factory Produced Type"));

      // Get an instance 
      var myType = container.Get<IMyType>();

      Assert.IsNotNull(myType);
      Assert.AreEqual(myType.GetType().FullName, typeof(MyType).FullName);
      Assert.AreEqual(myType.Text, "My Factory Produced Type");
    }

    /// <summary>
    /// Inject type
    /// </summary>
    [TestMethod]
    public void Example07() {
      var container = new Container();

      // Add types
      container.AddType<MyType>();
      container.AddType<MyOuterTypeA>();

      // Get an instance of the MyTypeOuterA with MyType
      var myOuterTypeA = container.Get<MyOuterTypeA>();

      Assert.IsNotNull(myOuterTypeA);
      Assert.AreEqual(myOuterTypeA.GetType().FullName, typeof(MyOuterTypeA).FullName);
      Assert.IsNotNull(myOuterTypeA.Obj);
      Assert.AreEqual(myOuterTypeA.Obj.GetType().FullName, typeof(MyType).FullName);
    }

    /// <summary>
    /// Inject type with resolve name 
    /// </summary>
    [TestMethod]
    public void Example08() {
      var container = new Container();

      // Add types
      container.AddType<IMyType, MyTypeA>("keyA");
      container.AddType<IMyType, MyTypeB>("keyB");
      container.AddType<MyOuterTypeB>();

      // Get an instance of the MyTypeOuter with MyTypeB
      var myOuterTypeB = container.Get<MyOuterTypeB>();

      Assert.IsNotNull(myOuterTypeB);
      Assert.AreEqual(myOuterTypeB.GetType().FullName, typeof(MyOuterTypeB).FullName);
      Assert.IsNotNull(myOuterTypeB.Obj);
      Assert.AreEqual(myOuterTypeB.Obj.GetType().FullName, typeof(MyTypeB).FullName);
    }

    /// <summary>
    /// Inject all types of assembly
    /// </summary>
    [TestMethod]
    public void Example09() {
      var container = new Container();

      // Inject all types from the current assembly
      container.AddAllTypesFromAssembly(Assembly.GetExecutingAssembly());

      Assert.IsNotNull(container.Get<MyType>());
      Assert.AreEqual(container.Get<MyType>().GetType().FullName, typeof(MyType).FullName);
    }

    /// <summary>
    /// Specify resolve type and name for injection from assembly
    /// </summary>
    [TestMethod]
    public void Example10() {
      var container = new Container();
      container.AddAllTypesFromAssembly(Assembly.GetExecutingAssembly());

      Assert.IsNotNull(container.Get<MyTypeC>());
      Assert.AreEqual(container.Get<MyTypeC>().GetType().FullName, typeof(MyTypeC).FullName);

      Assert.IsNotNull(container.Get<MyTypeD>());
      Assert.AreEqual(container.Get<MyTypeD>().GetType().FullName, typeof(MyTypeD).FullName);
      Assert.ReferenceEquals(container.Get<MyTypeD>(), container.Get<MyTypeD>());
    }

    /// <summary>
    /// Create type without registration
    /// </summary>
    [TestMethod]
    public void Example11() {
      var container = new Container();

      // Add type
      container.AddType<IMyType, MyType>();

      // Create type without registration with injection into the constructor
      var myUnregstredType = container.Create<MyUnregstredType>();

      Assert.IsNotNull(myUnregstredType);
      Assert.AreEqual(myUnregstredType.GetType().FullName, typeof(MyUnregstredType).FullName);
      Assert.IsNotNull(myUnregstredType.Obj);
      Assert.AreEqual(myUnregstredType.Obj.GetType().FullName, typeof(MyType).FullName);
    }
  }
}