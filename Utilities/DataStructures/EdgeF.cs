using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace Pladi.Utilities.DataStructures
{
    public struct EdgeF : IEquatable<EdgeF>
    {
        // [public properties and fields]

        [DataMember]
        public Vector2 A;

        [DataMember]
        public Vector2 B;

        // [constructors]

        public EdgeF(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }

        // [operators]

        public static bool operator ==(EdgeF a, EdgeF b)
            => a.A == b.A && a.B == b.B;

        public static bool operator !=(EdgeF a, EdgeF b)
            => !(a == b);

        // [public methods]

        public override bool Equals(object obj)
        {
            if (obj is EdgeF edge) return this == edge;
            return false;
        }

        public override int GetHashCode()
            => A.GetHashCode() ^ B.GetHashCode();

        public bool Equals(EdgeF other)
            => this == other;
    }
}