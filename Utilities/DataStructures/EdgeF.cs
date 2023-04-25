using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Pladi.Utilities.DataStructures
{
    public struct EdgeF : IEquatable<EdgeF>
    {
        [DataMember]
        public Vector2 A;

        [DataMember]
        public Vector2 B;

        // ...

        public EdgeF(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }

        // ...

        public static bool operator ==(EdgeF a, EdgeF b)
            => a.A == b.A && a.B == b.B;

        public static bool operator !=(EdgeF a, EdgeF b)
            => !(a == b);

        public override bool Equals(object obj)
        {
            if (obj is EdgeF)
            {
                return this == (EdgeF)obj;
            }

            return false;
        }

        public bool Equals(EdgeF other)
            => this == other;

        public override int GetHashCode()
            => A.GetHashCode() ^ B.GetHashCode();
    }
}
