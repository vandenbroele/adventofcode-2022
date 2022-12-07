var root = new Directory { DirectoryName = "/" };
Node node = root;
foreach (var line in System.IO.File.ReadAllLines("input.txt").Skip(1))
{
    node = line[..2] switch
    {
        "$ " => line[2..4] switch
        {
            "cd" => line[5..] switch
            {
                ".." => node.Parent!,
                var x => ((Directory) node).Children.OfType<Directory>().First(_ => _.DirectoryName == x)
            },
            _ => node
        },
        _ => line.Split(' ') switch
        {
            ["dir", var name] => node.AddChild(new Directory {DirectoryName = name}),
            [var size, var name] => node.AddChild(new File {FileName = name, FileSize = int.Parse(size)}),
            _ => node
        }
    };
}

var allDirs = root.ChildDirectories.ToArray();
Console.WriteLine(allDirs.Where(_ => _.Size <= 100000).Sum(_ => _.Size));
var diff = 30000000 - (70000000 - root.Size);
Console.WriteLine(allDirs.OrderBy(_ => _.Size).First(_ => _.Size > diff).Size);

internal abstract class Node
{
    public Node? Parent { get; protected internal set; }
    public abstract int Size { get; }

    public abstract Node AddChild(Node child);
}

internal class Directory : Node
{
    public required string DirectoryName { get; init; }
    public IReadOnlyCollection<Node> Children => _children.AsReadOnly();
    private readonly IList<Node> _children = new List<Node>();
    public IEnumerable<Directory> ChildDirectories => _children.OfType<Directory>()
        .Union(_children.OfType<Directory>().SelectMany(_ => _.ChildDirectories));
    public override int Size => _children.Sum(_ => _.Size);

    public override Node AddChild(Node child)
    {
        child.Parent = this;
        _children.Add(child);
        return this;
    }
}

internal class File : Node
{
    public required string FileName { get; init; }
    public required int FileSize { get; init; }
    public override int Size => FileSize;

    public override Node AddChild(Node child) => this;
}
