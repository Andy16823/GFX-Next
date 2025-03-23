using BulletSharp;
using LibGFX.Core;
using LibGFX.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Pyhsics
{
    /// <summary>
    /// Represents a 3D physics handler
    /// </summary>
    public class PhysicsHandler3D : PhysicsHandler
    {
        /// <summary>
        /// The physics world
        /// </summary>
        public DiscreteDynamicsWorld PhysicsWorld { get; set; }

        /// <summary>
        /// Creates a new 3D physics handler
        /// </summary>
        /// <param name="gravity"></param>
        public PhysicsHandler3D(Vector3 gravity)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = (System.Numerics.Vector3)gravity;
        }

        /// <summary>
        /// Manages a physics element
        /// </summary>
        /// <param name="physicsBehavior"></param>
        /// <param name="collisionGroup"></param>
        /// <param name="collisionMask"></param>
        public override void ManageElement(PhysicsBehavior physicsBehavior, int collisionGroup = -1, int collisionMask = -1)
        {
            PhysicsWorld.AddCollisionObject((CollisionObject)physicsBehavior.GetPhysicsObject(), collisionGroup, collisionMask);
        }

        /// <summary>
        /// Processes the physics handler
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

                Collision collisionA = new Collision()
                {
                    GameElement = elementB,
                    Contacts = contactManifold.NumContacts
                };
                elementA.Collide(collisionA);

                Collision collisionB = new Collision()
                {
                    GameElement = elementA,
                    Contacts = contactManifold.NumContacts
                };
                elementB.Collide(collisionB);
            }
        }

        /// <summary>
        /// Removes a physics element
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
            return (DebugDrawer) this.PhysicsWorld.DebugDrawer;
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
