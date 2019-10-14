# Metamorphosis

## Overview

Metamorphosis is a configuration based application framework. The building blocks of the configuration are _components_. Components define _signals_ (senders) and _triggers_ (receivers). Compatible (by signature) _signals_ and _triggers_ can be connected via configuration to build a complex tree of interacting _components_ - a complete application.

## The concept of triggers and signals

The concept of _triggers_ and _signals_ is based on the typical usecase of methods: Say component _A_ needs to push some data to component _B_. In a "normal" program component _A_ would retrieve an instance of component _B_ and execute a method on this instance of _B_.

The downside of this approach is that it cannot be configured at runtime but has to be hard-coded. To get around this limitation, interfaces can abstract away the actual implementation of component _B_. However, component _A_ would need to use a specific (hard-coded) interface which all possible component _B_s would have to implement. Again we run into limitations with possible configurations.

To get around this limitation, in _Metamorphosis_ component _A_ defines a _signal_, component _B_ defines a _trigger_. If _signal_ and _trigger_ are _compatible_, a connection can be created via configuration. Whenever component _A_ needs to push data to component _B_, it executes its _signal_ which executes the _trigger_ on component _B_.

Of course, a trigger may return data to the _singaling_ component, just as a method call would.

_Signals_ can be mandatory or optional. If a _signal_ is declared as an abstract method, it is considered mandatory. The initialization will throw an exception if the configuration does not define a valid connection to a mandatory _signal_. If a _signal_ is declared as an implemented method, it is considered optional. If the configuration does not contain a connection, the existing implementation is executed. If a connection is configured, it overwrites the default implementation.

More akin to events, a _signal_ may also be used to _trigger_ multiple components at the same time. It is then, however, not possible to return any data to the _signal_.

A component implementing a _signal_ is called a _sender_. A component implementing a _trigger_ is called a _receiver_.

## A technical view on triggers and signals

A _signal_ is a method on a _component_. A _trigger_ is a method with a compatible method signature on another _component_.

During initialization of a _sender_, a proxy is created which implements all abstract _signal_ methods and optionally overrides all virtual _signal_ methods. If a _connection_ is configured, a reference to the _receiver_ is stored in a hidden field in the _sender_. The generated implementation of the _signal_ method calls the _trigger_ method on the _receiver_ reference. Overhead is therefore minimized to an additional method call.

If multiple _triggers_ are connected to  a single _signal_, each _trigger_ on each _receiver_ is executed one after the other. Note that the order of execution is _undefined_!

Since _senders_ require references to _receivers_ (as is the case with a similarly setup method call), connections define a dependency graph. This _does_ mean that connections _must not_ create circular references. The dependency graph _must always_ be a tree. The order of components in the configuration file does _not_ matter. The initialization sequence initializes all components with respect to their dependencies.

## Implementing components

A _sender_ with a mandatory _signal_ looks like follows:
```C#
[Component]
public abstract class MathSender
{
    [Signal]
    protected abstract int Add(int a, int b);
}
```

- A _component_ must be decorated with the _ComponentAttribute_.
- A _component_ must be a _public abstract class_.
- A _signal_ must be decorated with the _SignalAttribute_.
- A _mandatory signal_ must be a _protected abstract method_.

The same _component_ using an optional _signal_ looks like follows:
```C#
[Component]
public abstract class MathSender
{
    [Signal]
    protected virtual int Add(int a, int b)
    {
        return a + b;
    }
}
```

- An _optional signal_ must be a _protected virtual method_.

A matching _receiver_ looks like follows:
```C#
[Component]
public abstract class MathReceiver
{
    [Trigger]
    public int Add(int a, int b)
    {
        return a + b;
    }
}
```

- A _trigger_ must be decorated with the _TriggerAttribute_.
- A _trigger_ must be a _public method_.
- A _trigger_ must not be _abstract_.

It is possible to use generic methods (including constraints) for _signals_ and _triggers_:
```C#
[Component]
public abstract class CloneSender
{
    [Signal]
    protected abstract T Clone<T>(T original) where T : IClonable;
}

[Component]
public abstract class CloneReceiver
{
    [Trigger]
    public T Clone<T>(T original) where T : IClonable
    {
        return original.Clone();
    }
}
```

- The signature _must_ be identical, this includes generic parameters and constraints.
- Generic type parameters on _components_ are _not_ supported.

Components may implement _IDisposable_ to perform cleanup before application shutdown. During shutdown, all components which implement _IDisposable_ get disposed with respect to their dependency tree. This means that dependent components will be disposed before their dependencies.

## Configuration

The configuration is a simple JSON file. It defines which component instances need to be created and how they are connected.

A typical configuration might look as follows:
```JSON
{
  "Components": [
    {
      "Name": "ConsoleLogger",
      "Type": "Metamorphosis.Logging.ConsoleLogger"
    },
    {
      "Name": "TestComponent",
      "Type": "Metamorphosis.Playground.TestComponent"
    }
  ],
  "Connections": [
    {
      "Signal": "TestComponent.Log",
      "Trigger": "ConsoleLogger.Log"
    }
  ],
  "Assemblies": [
    "Metamorphosis.Logging"
  ]
}
```

- Each component instance must have a unique _name_.
- Each component instance has must have a _type_, which is defined by the _FullName_ of the type object (including namespace, excluding assembly name).
- Each connection requires a _signal_ and a _trigger_.
- _Signals_ are defined by joining the _name_ of the _sender_ and the _signal_ using a "." (dot): "sender.signal".
- Likewise, _triggers_ are defined by joining the _name_ of the _receiver_ and the _trigger_: "receiver.trigger".
- All assemblies containing the configured components need to be provided using their file name without extension.

## Running the app

All "magic" is encapsulated in the class _App_.

```C#
class Program
{
    static void Main(string[] args)
    {
        var app = new App();
        app.Start("Model.json");
    }
}
```

## Special components

### Metamorphosis.Lifecycle

The lifecycle component provides signals regarding application startup and shutdown.

#### void Startup()
The startup signal is raised after the initialization is complete and before the main thread goes into sleep mode to wait for application shutdown. Components may now do their own initializations and/or start actions which need to run for the whole length of the application lifetime (timers, listeners, ...).

A components startup method _must_ be non-blocking. Any long running actions require creation of a separate thread.

#### void Shutdown()
The shutdown signal is raised just before the components get disposed. A component may now stop long running actions and perform cleanup. Do note that the order in which components receive the shutdown trigger is _undefined_.