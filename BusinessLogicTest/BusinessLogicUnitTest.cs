﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (BusinessLogicImplementation newInstance = new(new DataLayerConstructorFixcure()))
            {
                bool newInstanceDisposed = true;
                newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
                Assert.IsFalse(newInstanceDisposed);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataLayerDisposeFixcure dataLayerFixcure = new DataLayerDisposeFixcure();
            BusinessLogicImplementation newInstance = new(dataLayerFixcure);
            Assert.IsFalse(dataLayerFixcure.Disposed);
            bool newInstanceDisposed = true;
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsFalse(newInstanceDisposed);
            newInstance.Dispose();
            newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
            Assert.IsTrue(newInstanceDisposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, radius, ball) => { }));
            Assert.IsTrue(dataLayerFixcure.Disposed);
        }

        [TestMethod]
        public void StartTestMethod()
        {
            DataLayerStartFixcure dataLayerFixcure = new();
            using (BusinessLogicImplementation newInstance = new(dataLayerFixcure))
            {
                int called = 0;
                int numberOfBalls2Create = 10;
                newInstance.Start(
                  numberOfBalls2Create,
                  (startingPosition, radius, ball) => { called++; Assert.IsNotNull(startingPosition); Assert.IsNotNull(radius); Assert.IsNotNull(ball); });
                Assert.AreEqual<int>(1, called);
                Assert.IsTrue(dataLayerFixcure.StartCalled);
                Assert.AreEqual<int>(numberOfBalls2Create, dataLayerFixcure.NumberOfBallseCreated);
            }
        }

        #region testing instrumentation

        private class DataLayerConstructorFixcure : Data.DataAbstractAPI
        {
            public override void Dispose()
            { }

            public override void SetVelocity(double VelocityX, double VelocityY, Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void UpdateBallPosition(int i) {
                throw new NotImplementedException();
            }

            public override void LogCollision(ICollision loggerEntry)
            {
                throw new NotImplementedException();
            }
            public override IVector? CreateVector(double? x, double? y)
            {
                throw new NotImplementedException();
            }

        }

        private class DataLayerDisposeFixcure : Data.DataAbstractAPI
        {
            internal bool Disposed = false;

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void SetVelocity(double VelocityX, double VelocityY, Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                throw new NotImplementedException();
            }

            public override void UpdateBallPosition(int i)
            {
                throw new NotImplementedException();
            }

            public override void LogCollision(ICollision loggerEntry)
            {
                throw new NotImplementedException();
            }
            public override IVector? CreateVector(double? x, double? y)
            {
                throw new NotImplementedException();
            }

        }

        private class DataLayerStartFixcure : Data.DataAbstractAPI
        {
            internal bool StartCalled = false;
            internal int NumberOfBallseCreated = -1;

            public override void Dispose()
            { }

            public override void SetVelocity(double VelocityX, double VelocityY, Data.IBall ball)
            {
                throw new NotImplementedException();
            }

            public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
            {
                StartCalled = true;
                NumberOfBallseCreated = numberOfBalls;
                upperLayerHandler(new DataVectorFixture(), new DataBallFixture());
            }

            public override void UpdateBallPosition(int i)
            {
                throw new NotImplementedException();
            }

            private record DataVectorFixture : Data.IVector
            {
                public double x { get; init; }
                public double y { get; init; }
            }

            private class DataBallFixture : Data.IBall
            {
                public IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public IVector Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
                public double Radius { get; set; }

                public event EventHandler<IVector>? NewPositionNotification = null;
            }

            public override void LogCollision(ICollision loggerEntry)
            {
                throw new NotImplementedException();
            }
            public override IVector? CreateVector(double? x, double? y)
            {
                return new DataVectorFixture { x = x ?? 0, y = y ?? 0 };
            }
        }

        #endregion testing instrumentation
    }
}