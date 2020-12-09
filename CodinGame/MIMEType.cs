using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Solution
{
    static void Main(string[] args)
    {
        var N = int.Parse(Console.ReadLine()); // Number of elements which make up the association table.
        var Q = int.Parse(Console.ReadLine()); // Number Q of file names to be analyzed.

        var extsToMime = new Hashtable();


        for (var i = 0; i < N; i++)
        {
            var inputs = Console.ReadLine().Split(' ');
            var EXT = inputs[0]; // file extension
            var MT = inputs[1]; // MIME type.

            extsToMime.Add(EXT.ToLower(), MT);
        }

        for (var i = 0; i < Q; i++)
        {
            var FNAME = Console.ReadLine();
            if (FNAME.IndexOf('.') == -1) {
                Console.WriteLine("UNKNOWN");
                continue;
            }
            var substring = FNAME.Split('.').Last().ToLower();
            if (extsToMime.ContainsKey(substring))
                Console.WriteLine(extsToMime[substring]);
            else
                Console.WriteLine("UNKNOWN");
        }
    }
}