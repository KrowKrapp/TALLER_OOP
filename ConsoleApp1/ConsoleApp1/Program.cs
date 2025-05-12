using System;
using System.Threading;

using System;
using System.IO;
public abstract class Node
{
    public abstract bool Execute();
}

public class Root : Node
{
    private Node child;

    public Root(Node child)
    {
        if (child is Root) throw new InvalidOperationException("Root no puede tener otro Root como hijo.");
        if (child == null) throw new ArgumentNullException(nameof(child), "Child node cannot be null.");
        this.child = child;
    }

    public override bool Execute() => child?.Execute() ?? false;
}

public abstract class Composite : Node
{
    protected Node[] children;

    protected Composite(params Node[] children)
    {
        foreach (var child in children)
        {
            if (child is Root) throw new InvalidOperationException("Composite no puede tener Root como hijo.");
        }
        this.children = children;
    }
}

public class Sequence : Composite
{
    public Sequence(params Node[] children) : base(children) { }

    public override bool Execute()
    {
        foreach (var child in children)
            if (!child.Execute()) return false;
        return true;
    }
}

public class Selector : Composite
{
    public Selector(params Node[] children) : base(children) { }

    public override bool Execute()
    {
        foreach (var child in children)
        {
            if (child.Execute()) return true;
        }
        return false;
    }
}
public class IsEvenTask : Task
{
    private int number;

    public IsEvenTask(int number) : base(() => number % 2 == 0)
    {
        this.number = number;
    }

    public override bool Execute()
    {
        Console.WriteLine($"Verificando si {number} es par: {action()}");
        return action();
    }
}
public abstract class Task : Node
{
    protected Func<bool> action;

    protected Task(Func<bool> action)
    {
        this.action = action;
    }
}

public class MoveTask : Task
{
    public MoveTask(Func<bool> action) : base(action) { }
    public override bool Execute() => action();
}

public class WaitTask : Task
{
    public WaitTask(Func<bool> action) : base(action) { }
    public override bool Execute() => action();
}


class Program
{
    static void Main()
    {

        // Nodo raíz con una secuencia que contiene un selector y otra secuencia
        var tree1 = new Root(
            new Sequence(
                new Selector(
                    new IsEvenTask(3),
                    new IsEvenTask(4)
                ),
                new Sequence(
                    new IsEvenTask(6),
                    new IsEvenTask(8)
                )
            )
        );

        // Nodo raíz con un selector que contiene una secuencia y otro selector
        var tree2 = new Root(
            new Selector(
                new Sequence(
                    new IsEvenTask(1),
                    new IsEvenTask(2)
                ),
                new Selector(
                    new IsEvenTask(5),
                    new IsEvenTask(10)
                )
            )
        );

        // Nodo raíz con una secuencia que contiene un selector y una tarea
        var tree3 = new Root(
            new Sequence(
                new Selector(
                    new IsEvenTask(7),
                    new IsEvenTask(12)
                ),
                new IsEvenTask(14)
            )
        );

        // Ejecutar los árboles
        Console.WriteLine("Ejecutando tree1:");
        while (!tree1.Execute()) { }

        Console.WriteLine("\nEjecutando tree2:");
        while (!tree2.Execute()) { }

        Console.WriteLine("\nEjecutando tree3:");
        while (!tree3.Execute()) { }
    }


}