﻿//using System;
//class P
//{
//    static void Main()
//    {
//        var i=Array.ConvertAll(Console.ReadLine().Split(' '),int.Parse);
//        int a = i[3], b = i[2], c = i[1], d = i[0];
//        for (; ; )
//        {
//            var m = "";
//            if (c < a)
//            { a--; m = "N"; }
//            else if (c > a)
//            { a++; m = "S"; }
//            if (d > b)
//            { b++; m += "E"; }
//            else if (d < b)
//            { b--; m += "W"; }
//            Console.WriteLine(m);
//        }
//    }
//}