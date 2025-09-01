using System.Collections.Generic;
using System.Text.Json;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Service;
using LoggerKit;

namespace ContextBrowser.FileManager;

// context: relations, build
public static class ContextInfoRelationBuilder
{
    // context: relations, build
    public static void BuildRelation(ContextInfoSerializableModel contextInfoSerializableModel, ContextInfo context, IAppLogger<AppLevel> _logger, Dictionary<string, ContextInfo> lookupDictionary)
    {
        // Восстанавливаем ClassOwner и MethodOwner.

        ContextInfoRelationClassOwnerInjector.Inject(context, lookupDictionary: lookupDictionary, contextInfoSerializableModel: contextInfoSerializableModel);
        ContextInfoRelationMethodOwnerInjector.Inject(context, lookupDictionary: lookupDictionary, contextInfoSerializableModel: contextInfoSerializableModel);

        // Восстанавливаем References, InvokedBy и Properties.

        ContextInfoRelationReferenceInjector.Inject(context: context, lookupDictionary: lookupDictionary, referencesFullNames: contextInfoSerializableModel.ReferencesFullNames, _logger: _logger);
        ContextInfoRelationInvokedByInjector.Inject(context: context, lookupDictionary: lookupDictionary, invokedByFullNames: contextInfoSerializableModel.InvokedByFullNames, _logger: _logger);
        ContextInfoRelationPropertyInjector.Inject(context: context, lookupDictionary: lookupDictionary, propertiesFullNames: contextInfoSerializableModel.PropertiesFullNames, _logger: _logger);
    }
}

// context: relations, build
public static class ContextInfoRelationClassOwnerInjector
{
    // context: relations, build
    public static void Inject(ContextInfo context, ContextInfoSerializableModel contextInfoSerializableModel, Dictionary<string, ContextInfo> lookupDictionary)
    {
        if (contextInfoSerializableModel.ClassOwnerFullName != null && lookupDictionary.ContainsKey(contextInfoSerializableModel.ClassOwnerFullName))
        {
            context.ClassOwner = lookupDictionary[contextInfoSerializableModel.ClassOwnerFullName];
        }
    }
}

// context: relations, build
public static class ContextInfoRelationMethodOwnerInjector
{
    // context: relations, build
    public static void Inject(ContextInfo context, ContextInfoSerializableModel contextInfoSerializableModel, Dictionary<string, ContextInfo> lookupDictionary)
    {
        if (contextInfoSerializableModel.MethodOwnerFullName != null && lookupDictionary.ContainsKey(contextInfoSerializableModel.MethodOwnerFullName))
        {
            context.MethodOwner = lookupDictionary[contextInfoSerializableModel.MethodOwnerFullName];
        }
    }
}

// context: relations, build
public static class ContextInfoRelationReferenceInjector
{
    // context: relations, build
    public static void Inject(IAppLogger<AppLevel> _logger, Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> referencesFullNames)
    {
        foreach (var fullName in referencesFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var reference))
            {
                InjectReference(context, reference, _logger);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Reference not found {fullName}");
            }
        }
    }

    private static void InjectReference(ContextInfo context, ContextInfo reference, IAppLogger<AppLevel> _logger)
    {
        var addedReference = ContextInfoService.AddToReferences(context, reference);
        var message = (addedReference)
            ? $"[DONE] Adding reference for [{context.FullName}] with [{reference.FullName}]"
            : $"[SKIP] Adding reference for [{context.FullName}] with [{reference.FullName}]";
        var level = addedReference
            ? LogLevel.Trace
            : LogLevel.Err;

        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }

}

// context: relations, build
public static class ContextInfoRelationInvokedByInjector
{
    // context: relations, build
    public static void Inject(IAppLogger<AppLevel> _logger, Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> invokedByFullNames)
    {
        foreach (var fullName in invokedByFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var invokedBy))
            {
                InjectInvokedBy(context, invokedBy, _logger);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"InvokedBy not found {fullName}");
            }
        }
    }

    private static void InjectInvokedBy(ContextInfo context, ContextInfo invokedBy, IAppLogger<AppLevel> _logger)
    {
        var addedInvokedBy = ContextInfoService.AddToInvokedBy(context, invokedBy);
        var message = addedInvokedBy
            ? $"[DONE] Adding invokedBy for [{context.FullName}] with [{invokedBy.FullName}]"
            : $"[SKIP] Adding invokedBy for [{context.FullName}] with [{invokedBy.FullName}]";
        var level = addedInvokedBy
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}

// context: relations, build
public static class ContextInfoRelationPropertyInjector
{
    // context: relations, build
    public static void Inject(IAppLogger<AppLevel> _logger, Dictionary<string, ContextInfo> lookupDictionary, ContextInfo context, HashSet<string> propertiesFullNames)
    {
        foreach (var fullName in propertiesFullNames)
        {
            if (lookupDictionary.TryGetValue(fullName, out var property))
            {
                InjectProperty(context, property, _logger);
            }
            else
            {
                _logger.WriteLog(AppLevel.R_Cntx, LogLevel.Err, $"Property not found {fullName}");
            }
        }
    }

    private static void InjectProperty(ContextInfo context, ContextInfo property, IAppLogger<AppLevel> _logger)
    {
        var addedToProperties = ContextInfoService.AddToProperties(context, property);
        var message = addedToProperties
            ? $"[DONE] Adding property for [{context.FullName}] with [{property.FullName}]"
            : $"[SKIP] Adding property for [{context.FullName}] with [{property.FullName}]";
        var level = addedToProperties
            ? LogLevel.Trace
            : LogLevel.Err;
        _logger.WriteLog(AppLevel.R_Cntx, level, message);
    }
}