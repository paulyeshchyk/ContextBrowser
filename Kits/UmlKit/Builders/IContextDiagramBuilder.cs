﻿using System.Collections.Generic;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using UmlKit.Builders.Model;

namespace UmlKit.Builders;

// context: build, transition
public interface IContextDiagramBuilder
{
    GrouppedSortedTransitionList? Build(string metaItem, FetchType fetchType, List<ContextInfo> allContexts);
}

public enum FetchType
{
    FetchAction,
    FetchDomain
}