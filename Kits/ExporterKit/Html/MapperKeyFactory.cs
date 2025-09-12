using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContextKit.Model;

namespace ExporterKit.Html;

public class ExportKitMapperKeys : MapperKeyBase
{
    protected ExportKitMapperKeys(string value) : base(value) { }

    public static readonly ExportKitMapperKeys DomainPerAction = new ExportKitMapperKeys("DomainPerAction");
}