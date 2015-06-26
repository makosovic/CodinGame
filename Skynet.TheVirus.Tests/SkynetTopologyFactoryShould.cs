using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Skynet.TheVirus.Tests
{
    [TestClass]
    public class SkynetTopologyFactoryShould
    {
        [TestMethod]
        public void CreateTreeProperly()
        {
            int rootValue = 0;
            List<int> gateways = new List<int> { 0, 1, 2 };
            #region links
            List<Tuple<int, int>> links = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(0, 4),
                new Tuple<int, int>(0, 5),
                new Tuple<int, int>(0, 6),
                new Tuple<int, int>(0, 7),
                new Tuple<int, int>(0, 8),
                new Tuple<int, int>(0, 9),
                new Tuple<int, int>(0, 10),
                new Tuple<int, int>(0, 11),
                new Tuple<int, int>(3, 11),
                new Tuple<int, int>(3, 4),
                new Tuple<int, int>(4, 5),
                new Tuple<int, int>(5, 6),
                new Tuple<int, int>(6, 7),
                new Tuple<int, int>(7, 8),
                new Tuple<int, int>(8, 9),
                new Tuple<int, int>(9, 10),
                new Tuple<int, int>(10, 11),
                new Tuple<int, int>(8, 19),
                new Tuple<int, int>(7, 20),
                new Tuple<int, int>(6, 13),
                new Tuple<int, int>(6, 12),
                new Tuple<int, int>(5, 12),
                new Tuple<int, int>(12, 28),
                new Tuple<int, int>(12, 29),
                new Tuple<int, int>(12, 13),
                new Tuple<int, int>(13, 29),
                new Tuple<int, int>(14, 21),
                new Tuple<int, int>(15, 22),
                new Tuple<int, int>(13, 1),
                new Tuple<int, int>(14, 1),
                new Tuple<int, int>(15, 1),
                new Tuple<int, int>(16, 1),
                new Tuple<int, int>(17, 1),
                new Tuple<int, int>(18, 1),
                new Tuple<int, int>(19, 1),
                new Tuple<int, int>(20, 1),
                new Tuple<int, int>(13, 20),
                new Tuple<int, int>(13, 14),
                new Tuple<int, int>(14, 15),
                new Tuple<int, int>(15, 16),
                new Tuple<int, int>(16, 17),
                new Tuple<int, int>(17, 18),
                new Tuple<int, int>(18, 19),
                new Tuple<int, int>(19, 20),
                new Tuple<int, int>(21, 2),
                new Tuple<int, int>(22, 2),
                new Tuple<int, int>(23, 2),
                new Tuple<int, int>(24, 2),
                new Tuple<int, int>(25, 2),
                new Tuple<int, int>(26, 2),
                new Tuple<int, int>(27, 2),
                new Tuple<int, int>(28, 2),
                new Tuple<int, int>(29, 2),
                new Tuple<int, int>(21, 29),
                new Tuple<int, int>(21, 22),
                new Tuple<int, int>(22, 23),
                new Tuple<int, int>(23, 24),
                new Tuple<int, int>(24, 25),
                new Tuple<int, int>(25, 26),
                new Tuple<int, int>(26, 27),
                new Tuple<int, int>(27, 28),
                new Tuple<int, int>(28, 29)
            };
            #endregion

            List<Tree<int>> trees = new List<Tree<int>>();

            foreach (var gateway in gateways)
            {
                SkynetTopologyFactory factory = new SkynetTopologyFactory();
                trees.Add(factory.CreateTree(rootValue, links, gateways));
            }

            Assert.AreEqual(5, trees.First(tree => tree.Root.Value == 0).Root.Children.First(node => node.Value == 6).Children.First(node => node.Value == 12).Children.Count);
        }
    }
}
