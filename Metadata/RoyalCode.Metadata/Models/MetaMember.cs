using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RoyalCode.Metadata.Models
{
    /// <summary>
    /// Metadados de um membro de um tipo, como propriedades, campos, métodos, eventos, construtor.
    /// </summary>
    public class MetaMember : MetaBase
    {
        public MetaMember(MemberInfo member)
        {
            Member = member;
        }

        public MetaMember(MetaModel metaModel, MetaBase parent, MemberInfo member) 
            : base(metaModel, parent)
        {
            Member = member;
        }

        public MemberInfo Member { get; protected set; }
    }
}
