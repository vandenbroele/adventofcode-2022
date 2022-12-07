using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Day7
{
    internal static class Program
    {
        public static void Main()
        {
            File.ReadAllText("./input.txt");
            Dir root = Parse(File.ReadAllLines("./input.txt"));

            int result1 = root.Traverse()
                .Select(d => d.CalculateSize())
                .Where(size => size <= 100000)
                .Sum();

            Console.WriteLine($"part 1: {result1}");

            const int maxSpace = 70000000;
            const int requiredSpace = 30000000;

            int usedSpace = root.CalculateSize();
            int unusedSpace = maxSpace - usedSpace;
            int toDelete = requiredSpace - unusedSpace;

            int result2 = root.Traverse()
                .Select(d => d.CalculateSize())
                .Where(size => size > toDelete)
                .OrderBy(size => size)
                .First();

            Console.WriteLine($"part 2: {result2}");
        }

        public static Dir Parse(IEnumerable<string> input)
        {
            Dir root = new Dir(null);

            Dir current = root;


            foreach (string cmd in input)
            {
                string[] parts = cmd.Split(' ');

                bool isCommand = cmd.StartsWith("$");
                if (isCommand)
                {
                    parts = cmd.Substring(2).Split(' ');
                }

                if (isCommand)
                {
                    switch (parts[0])
                    {
                        case "cd":
                        {
                            string parameter = parts[1];
                            switch (parameter)
                            {
                                case "/":
                                    current = root;
                                    break;
                                case "..":
                                    current = current.Up();
                                    break;
                                default:
                                    current = current.Enter(parameter);
                                    break;
                            }

                            break;
                        }

                        case "ls":

                            break;
                    }
                }
                else
                {
                    // We are in ls

                    if (parts[0] == "dir")
                    {
                        current.AddDir(parts[1]);
                    }
                    else
                    {
                        current.AddFile(parts[1], int.Parse(parts[0]));
                    }
                }
            }

            return root;
        }
    }


    internal class Dir
    {
        private readonly Dir parent;
        private readonly Dictionary<string, Dir> children = new Dictionary<string, Dir>();
        private readonly Dictionary<string, F> files = new Dictionary<string, F>();

        public Dir(Dir parent)
        {
            this.parent = parent;
        }

        public void AddDir(string name)
        {
            children.Add(name, new Dir(this));
        }

        public void AddFile(string name, int size)
        {
            files.Add(name, new F(size));
        }

        public Dir Enter(string name)
        {
            if (children.ContainsKey(name))
            {
                return children[name];
            }

            Dir newDir = new Dir(this);
            children.Add(name, newDir);
            return newDir;
        }

        public Dir Up() => parent;

        public string WriteTree()
        {
            DirWriter writer = new DirWriter();
            writer.WriteDir("/");
            WriteSelf(writer);

            return writer.ToString();
        }

        private void WriteSelf(DirWriter writer)
        {
            writer.Down();
            foreach (KeyValuePair<string, Dir> keyValuePair in children)
            {
                writer.WriteDir(keyValuePair.Key);
                keyValuePair.Value.WriteSelf(writer);
            }

            foreach (KeyValuePair<string, F> keyValuePair in files)
            {
                writer.WriteFile(keyValuePair.Key, keyValuePair.Value.Size);
            }

            writer.Up();
        }

        public int CalculateSize()
        {
            return Traverse()
                .Select(d => d.GetOwnSize())
                .Sum();
        }

        private int GetOwnSize()
        {
            return files.Values.Select(f => f.Size).Sum();
        }

        public IEnumerable<Dir> Traverse()
        {
            yield return this;

            foreach (Dir child in children.Values)
            {
                foreach (Dir dir in child.Traverse())
                {
                    yield return dir;
                }
            }
        }

        private class DirWriter
        {
            private StringBuilder sb = new StringBuilder();
            private int indent = 0;


            public override string ToString()
            {
                return sb.ToString();
            }

            public void WriteDir(string s)
            {
                sb.Append(new string(' ', indent));
                sb.AppendLine($"{s} (dir)");
            }

            public void WriteFile(string name, int size)
            {
                sb.Append(new string(' ', indent));
                sb.AppendLine($"{name} (file {size})");
            }

            public void Down()
            {
                indent++;
            }

            public void Up()
            {
                indent--;
            }
        }
    }

    internal class F
    {
        public int Size { get; }

        public F(int size)
        {
            Size = size;
        }
    }


    public class Tests
    {
        [Test]
        public void TestInput()
        {
            string[] lines = testInput.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None);


            Dir d = Program.Parse(lines);

            Assert.AreEqual(4, d.Traverse().Count());

            Assert.AreEqual(48381165, d.CalculateSize());

            Console.WriteLine(d.WriteTree());
        }

        private const string testInput = @"$ cd /
$ ls
dir a
14848514 b.txt
8504156 c.dat
dir d
$ cd a
$ ls
dir e
29116 f
2557 g
62596 h.lst
$ cd e
$ ls
584 i
$ cd ..
$ cd ..
$ cd d
$ ls
4060174 j
8033020 d.log
5626152 d.ext
7214296 k";
    }
}