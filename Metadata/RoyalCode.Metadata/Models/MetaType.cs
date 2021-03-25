using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RoyalCode.Metadata.Models
{
    public class MetaType : MetaBase
    {
        public MetaType(TypeInfo typeInfo)
        {
            TypeInfo = typeInfo;
        }

        public MetaType(MetaModel metaModel, MetaBase parent, TypeInfo typeInfo) : base(metaModel, parent)
        {
            TypeInfo = typeInfo;
        }

        public TypeInfo TypeInfo;

        public MetaMember GetMember(string memberName)
        {
            throw new NotImplementedException();
        }
    }
}
