// contentNavigator/generateNavigationContent.js

// @ts-nocheck

const { generateBytesCounterPage } = require('./Pagebuilder.Config.Bytescounter.js');
const { generateHelpTagPage } = require('./Pagebuilder.Config.HelptagMap.js');
const { generateContextsPage } = require('./Pagebuilder.Config.ContextMap.js');
const { generateStat } = require('./Pagebuilder.Stats.js');

function runGeneration({ mapDir, outputDir, command, docsDir }) {

  if (command === "periodicTable") {
    generateBytesCounterPage({ mapDir: outputDir, outputDir: outputDir, lang: ["ru", "en"] });
  } else if (command === "periodicStat") {
    generateStat({ outputDir: outputDir, docsDir: docsDir });
  } else if (command === "periodicTableHelpTag") {
    generateHelpTagPage({ mapDir: outputDir, outputDir: outputDir, lang: ["ru", "en"] });
  } else if (command === "periodicTableContext") {
    generateContextsPage({ mapDir: outputDir, outputDir: outputDir, lang: ["ru", "en"] });
  }
}

module.exports = { runGeneration }
