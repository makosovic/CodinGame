using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TronBattle.Tests
{
    [TestClass]
    public class MoveValidatorShould
    {
        #region private fields
        readonly Position _currentPlayerPosition = new Position("18", "3");
        readonly int?[,] _board =
            {
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 1, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 1, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 1, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 1, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null },
                { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null }
            };
        #endregion

        [TestMethod]
        public void ReturnFalseIfMoveRightButRightTaken()
        {
            MoveValidator validator = new MoveValidator();
            bool actual = validator.IsValid(Move.RIGHT, new GameState { Board = _board, MyPosition = _currentPlayerPosition });
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void ReturnDownForGivenBoard()
        {
            IMoveFactory factory = new ValidMoveFactory();
            Move actual = factory.GetMove(new GameState { Board = _board, MyPosition = _currentPlayerPosition });
            Assert.AreEqual(Move.DOWN, actual);
        }
    }
}
