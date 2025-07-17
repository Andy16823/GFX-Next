using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace LibGFX.Physics
{
    /// <summary>
    /// Represents a physics handler for 2D physics
    /// </summary>
    public class PhysicsHandler2D : PhysicsHandler
    {
        /// <summary>
        /// The physics world
        /// </summary>
        public DiscreteDynamicsWorld PhysicsWorld { get; set; }

        /// <summary>
        /// Creates a new physics handler
        /// </summary>
        /// <param name="gravity"></param>
        public PhysicsHandler2D(OpenTK.Mathematics.Vector2 gravity)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = (System.Numerics.Vector3)new Vector3(gravity.X, gravity.Y, 0);
        }

        /// <summary>
        /// Adds a physics behavior to the physics world
        /// </summary>
        /// <param name="physicsBehavior"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public override void ManageElement(PhysicsBehavior physicsBehavior, int collisionGroup = -1, int collisionMask = -1)
        {
            PhysicsWorld.AddCollisionObject((CollisionObject)physicsBehavior.GetPhysicsObject(), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Processes the physics world
        /// </summary>
        /// <param name="scene"></param>
        public override void Process(BaseScene scene)
        {
            this.PhysicsWorld.StepSimulation(1.0f / 60.0f, 10); // (float)(game.DeltaTime / 1000)
            int numManifolds = PhysicsWorld.Dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = PhysicsWorld.Dispatcher.GetManifoldByIndexInternal(i);

                CollisionObject obA = contactManifold.Body0 as CollisionObject;
                var elementA = (GameElement)obA.UserObject;

                CollisionObject obB = contactManifold.Body1 as CollisionObject;
                var elementB = (GameElement)obB.UserObject;

                // Get the contact points
                var contactPoints = new List<CollisionPoint>();
                for (int p = 0; p < contactManifold.NumContacts; p++)
                {
                    var point = contactManifold.GetContactPoint(p);
                    contactPoints.Add(new CollisionPoint()
                    {
                        LocalPointA = (Vector3)point.LocalPointA,
                        LocalPointB = (Vector3)point.LocalPointB,
                        WorldPointA = (Vector3)point.PositionWorldOnA,
                        WorldPointB = (Vector3)point.PositionWorldOnB,
                        WorldNormal = (Vector3)point.NormalWorldOnB,
                        Impulse = point.AppliedImpulse
                    });
                }

                Collision collisionA = new Collision()
                {
                    GameElement = elementB,
                    Contacts = contactManifold.NumContacts,
                    ContactPoints = contactPoints,
                    ElementIndex = ElementIndex.A
                };
                elementA.Collide(collisionA);

                Collision collisionB = new Collision()
                {
                    GameElement = elementA,
                    Contacts = contactManifold.NumContacts,
                    ContactPoints = contactPoints,
                    ElementIndex = ElementIndex.B
                };
                elementB.Collide(collisionB);
            }
        }

        /// <summary>
        /// Removes a physics behavior from the physics world
        /// </summary>
        /// <param name="physicsBehavior"></param>
        public override void RemoveElement(PhysicsBehavior physicsBehavior)
        {
            PhysicsWorld.RemoveCollisionObject((CollisionObject)physicsBehavior.GetPhysicsObject());
        }

        /// <summary>
        /// Determines if the physics handler has a debug drawer
        /// </summary>
        /// <returns></returns>
        public override bool HasDebugDrawer()
        {
            if (this.PhysicsWorld.DebugDrawer != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Debug draws the physics handler
        /// </summary>
        /// <param name="renderer"></param>
        public override void DebugDraw(IRenderDevice renderer)
        {
            this.PhysicsWorld.DebugDrawWorld();
        }

        /// <summary>
        /// Gets the debug drawer
        /// </summary>
        /// <returns></returns>
        public override DebugDrawer GetDebugDrawer()
        {
            return (DebugDrawer)this.PhysicsWorld.DebugDrawer;
        }

        /// <summary>
        /// Sets the debug drawer
        /// </summary>
        /// <param name="debugDrawer"></param>
        public override void SetDebugDrawer(DebugDrawer debugDrawer)
        {
            this.PhysicsWorld.DebugDrawer = debugDrawer;
        }
    }
}
