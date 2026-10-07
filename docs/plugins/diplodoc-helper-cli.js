#!/usr/bin/env node

// diplodoc-helper-cli.js
const path = require("path");
const { runGeneration: generateHelpMap } = require("./helpMap/helpmap.js");
const { runGeneration: generateContexts } = require("./contexts/сontexts.js");
const { runGeneration: generateHelptags } = require("./helptags/helptags.js");
const { runGeneration: generateBreadcrumb } = require("./breadcrumb/breadcrumb.js");
const { runGeneration: generateContexttags } = require("./context-tags/context-tags.js");
const { injectCleanMode: generateTocCleanMode } = require("./tocCleanMode/post-build.js");
const { reindexFigures } = require("./reindexer/reindexer.figures.js");
const { DiplodocConfigFromCli } = require("../plugins/manifest/config/diplodoc.config.js");
const { fixFootnoteAnchors } = require('./footnoteAnchors/fix-footnote-anchors.js');
const { runGeneration: generateNavigationContent } = require("./contentNavigator/generateNavigationContent.js");
const { runGeneration: generateRoadMap, DiagramEngine } = require("./roadMap/roadmap");
const { runGeneration: generateGanttFixScroll } = require("./roadMap/post-build");
const { CONFIG_KEY } = require("../plugins/manifest/constants.js");

// Если запускают как cli
if (require.main === module) {
  const args = process.argv.slice(2);
  const command = args[0];

  const docsDirIndex = args.indexOf("--docsDir");
  const docsDir = docsDirIndex !== -1 ? args[docsDirIndex + 1] : "./docs";

  const outputDirIndex = args.indexOf("--outputDir");
  const outputDir = outputDirIndex !== -1 ? args[outputDirIndex + 1] : "./build";

  const segregation = args.includes("--segregation");

  if (command === "helpMap") {
    generateHelpMap({ docsDir, outputDir, segregation });
  } else if (command === "periodicStat") {
    generateNavigationContent({ mapDir: outputDir, outputDir: outputDir, command: command, docsDir: docsDir });
  } else if (command === "periodicTable") {
    generateNavigationContent({ mapDir: outputDir, outputDir: outputDir, command: command, docsDir: docsDir });
  } else if (command === "periodicTableHelpTag") {
    generateNavigationContent({ mapDir: outputDir, outputDir: outputDir, command: command, docsDir: docsDir });
  } else if (command === "periodicTableContext") {
    generateNavigationContent({ mapDir: outputDir, outputDir: outputDir, command: command, docsDir: docsDir });
  } else if (command === "reindexFigures") {
    const { targetLocale, configObj } = DiplodocConfigFromCli(CONFIG_KEY);
    reindexFigures(docsDir, targetLocale, configObj);
  } else if (command === "contexts") {
    generateContexts(docsDir);
  } else if (command === 'footnoteAnchors') {
    fixFootnoteAnchors(outputDir);
  } else if (command === "helptags") {
    generateHelptags(docsDir);
  } else if (command === "contexttags") {
    generateContexttags(outputDir);
  } else if (command === "breadcrumb") {
    generateBreadcrumb(outputDir);
  } else if (command === "tocCleanMode") {
    generateTocCleanMode(outputDir);
  } else if (command === "ganttFixScroll") {
    generateGanttFixScroll({ docsRoot: outputDir, engine:  DiagramEngine.MERMAID});
  } else if (command === "roadMap") {
    const docsDirIndex = args.indexOf("--docsDir");
    const docsDir = docsDirIndex !== -1 ? args[docsDirIndex + 1] : "./docs";

    const dataPathIndex = args.indexOf("--dataPath");
    const dataPath = dataPathIndex !== -1 ? args[dataPathIndex + 1] : null;

    const dataRawIndex = args.indexOf("--dataRaw");
    const dataRaw = dataRawIndex !== -1 ? args[dataRawIndex + 1] : null;

    // Если передали raw JSON — берем его, иначе путь, иначе fallback
    const data = dataRaw || dataPath;

    generateRoadMap({ docsRoot: docsDir, data, engine:  DiagramEngine.MERMAID});
  }
}

module.exports = {
  generateHelpMap,
  generateContexts,
  generateBreadcrumb,
  generateTocCleanMode,
  fixFootnoteAnchors,
  reindexFigures,
};
