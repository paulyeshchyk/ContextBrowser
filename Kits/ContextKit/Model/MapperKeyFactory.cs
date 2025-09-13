using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextKit.Model;

namespace ExporterKit.Html;

public static class GlobalMapperKeys
{
    public static readonly MapperKeyBase DomainPerAction = MapperKeyBase.Create("DomainPerAction");
    public static readonly MapperKeyBase NameClassName = MapperKeyBase.Create("NameClassName");

    // public static readonly MapperKeyBase AnotherMapper = MapperKeyBase.Create("AnotherMapper");
}
