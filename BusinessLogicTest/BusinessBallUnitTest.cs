//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        [TestMethod]
        public void HandleBorderCollisionTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture
            {
                Position = new VectorFixture(590, 150), // Near right border
                Velocity = new VectorFixture(5, 0),     // Moving right
                Radius = 5
            };
            Ball ball = new Ball(dataBallFixture);
            bool velocityChanged = false;

            ball.NewVelocityNotification += (sender, position) =>
            {
                velocityChanged = true;
                Assert.AreEqual(-5, position.x); // Velocity should be reversed
                Assert.AreEqual(0, position.y);  // Y velocity should remain unchanged
            };

            ball.HandleBorderCollision(600, 300, 4);

            Assert.IsTrue(velocityChanged, "Velocity should change when ball hits border");
        }

        [TestMethod]
        public void HandleBallCollisionTestMethod()
        {
            DataBallFixture ball1Data = new DataBallFixture
            {
                Position = new VectorFixture(100, 100),
                Velocity = new VectorFixture(5, 0),
                Radius = 10
            };

            DataBallFixture ball2Data = new DataBallFixture
            {
                Position = new VectorFixture(119, 100), // Just within collision distance
                Velocity = new VectorFixture(-5, 0),
                Radius = 10
            };

            Ball ball1 = new Ball(ball1Data);
            Ball ball2 = new Ball(ball2Data);
            bool velocityChanged = false;

            ball1.NewVelocityNotification += (sender, position) =>
            {
                velocityChanged = true;
            };

            ball1.HandleBallCollision(ball2);

            Assert.IsTrue(velocityChanged, "Velocity should change when balls collide");
        }

        [TestMethod]
        public void NoCollisionWhenBallsAreDistantTestMethod()
        {
            DataBallFixture ball1Data = new DataBallFixture
            {
                Position = new VectorFixture(100, 100),
                Velocity = new VectorFixture(5, 0),
                Radius = 10
            };

            DataBallFixture ball2Data = new DataBallFixture
            {
                Position = new VectorFixture(200, 200), // Far away
                Velocity = new VectorFixture(-5, 0),
                Radius = 10
            };

            Ball ball1 = new Ball(ball1Data);
            Ball ball2 = new Ball(ball2Data);
            bool velocityChanged = false;

            ball1.NewVelocityNotification += (sender, position) =>
            {
                velocityChanged = true;
            };

            ball1.HandleBallCollision(ball2);

            Assert.IsFalse(velocityChanged, "Velocity should not change when balls are distant");
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get; set; } = new VectorFixture(0, 0);
            public Data.IVector Position { get; set; } = new VectorFixture(0, 0);
            public double Radius { get; set; } = 10;
            public event EventHandler<Data.IVector>? NewPositionNotification;

            internal void Move()
            {
                NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
            }
        }

        private class VectorFixture : Data.IVector
        {
            public VectorFixture(double X, double Y)
            {
                x = X;
                y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}