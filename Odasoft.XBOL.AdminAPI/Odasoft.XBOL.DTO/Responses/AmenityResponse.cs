using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Text;

namespace Odasoft.XBOL.DTO.Responses
{
    public class AmenityResponse
    {
        public required long Id { get; set; }
        public required string Name { get; set; } = "";
        public required string IconIdentifier { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is AmenityResponse other)
            {
                return this.Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
