<Query Kind="Statements" />

var inputList = System.IO.File.ReadAllLines(Path.Combine(Util.CurrentQueryPath, "..", "day7-input.txt"));
#region testinput
//var inputList = @"$ cd /
//$ ls
//dir a
//14848514 b.txt
//8504156 c.dat
//dir d
//$ cd a
//$ ls
//dir e
//29116 f
//2557 g
//62596 h.lst
//$ cd e
//$ ls
//584 i
//$ cd..
//$ cd..
//$ cd d
//$ ls
//4060174 j
//8033020 d.log
//5626152 d.ext
//7214296 k".Split(Environment.NewLine);
#endregion

const int totalSize = 70000000;
const int expectedFreeSpace = 30000000;

// --- Build Tree

var input = new Queue<string>(inputList);
input.Dequeue(); //remove "cd /"
var root = new Directory("/", null);
BuildTree(root, input);

// --- Find total size of small dirs

const int maxSize = 100000;
var smallDirs = new List<Directory>();
FindSmallerThan(root, maxSize, ref smallDirs);

var answer1 = smallDirs.Sum(x => x.Size).Dump("Answer 1"); //1844187

// --- Find size of dir to delete

var freeSpace = totalSize - root.Size;
var toBeFreed = (expectedFreeSpace - freeSpace);
var largeDirs = new List<Directory>();
FindLargerThan(root, toBeFreed, ref largeDirs);

var answer2 = largeDirs.OrderBy(x => x.Size).First().Size.Dump("Answer 2"); //4978279

// ---

void BuildTree(Directory parent, Queue<string> input)
{
	while (input.Count > 0)
	{
		string line = input.Dequeue();

		if (line.EndsWith(".."))
		{
			BuildTree(parent.Parent, input);
		}
		else if (line.StartsWith("$ cd "))
		{
			var dir = new Directory(line.Substring(5), parent);
			parent.Children.Add(dir);
			BuildTree(dir, input);
		}
		else if (line[0] >= 48 && line[0] <= 57) //file
		{
			var file = new File(line);
			parent.Children.Add(file);
		}
	}
}

void FindSmallerThan(Directory root, int size, ref List<Directory> smallDirs)
{
	foreach(var dir in root.Children.Where(x => x is Directory).Cast<Directory>()) {
		if (dir.Size <= size)
		{
			smallDirs.Add(dir);
		}
		FindSmallerThan(dir, size, ref smallDirs);
	}
}

void FindLargerThan(Directory root, int size, ref List<Directory> largeDirs)
{
	foreach (var dir in root.Children.Where(x => x is Directory).Cast<Directory>())
	{
		if (dir.Size >= size)
		{
			largeDirs.Add(dir);
		}
		FindLargerThan(dir, size, ref largeDirs);
	}
}

interface Item 
{
	public string Name { get; }
	public int Size { get; }
}

class File : Item
{
	public string Name { get; set; }
	public int Size { get; set; }
	
	public File(string name, int size)
	{
		Name = name;
		Size = size;
	}

	public File(string sizeAndName)
	{
		Size = int.Parse(sizeAndName.Split(" ")[0]);
		Name = sizeAndName.Split(" ")[1];
	}
}

class Directory : Item
{
	public string Name { get; set; }
	public List<Item> Children {get; set; }  = new List<Item>();
	public int Size => Children.Select(x => x.Size).Sum();
	
	public Directory Parent { get; set; }
	
	public Directory(string name, Directory parent)
	{
		Name = name;
		Parent = parent;
	}
}