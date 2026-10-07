// contexts/generateContexts.writer.js

const fs = require("fs");
const path = require("path");

const { generateMapContent } = require("./modules/map");
const { generateListContent } = require("./modules/list");
const { generateSearchContent } = require("./modules/search");
const { generateTabsMarkup } = require("./modules/tabs");

const { createSearchScriptContent } = require("./context-search.client");

const {
  INDEX_MD_DEFAULT_CONTENT,
  TOC_YAML_LINKS_TEMPLATE,
  TOC_YAML_LINK_TEMPLATE,
  INDEX_TAML_LINKS_TEMPLATE,
  INDEX_YAML_LINK_TEMPLATE,
} = require("./contexts.template");

const { slugify_filename } = require("../utils/encoding.slugify");

/** @import {ContextMap} from '../model/contextmap.model' */

/**
 * @param {string} outputDir
 * @param {string[]} sortedTerms
 * @param {ContextMap} contextMap
 */
function writeTermFiles(outputDir, sortedTerms, contextMap) {
  for (const term of sortedTerms) {
    const slug = slugify_filename(term);
    const pages = contextMap[term]?.pages || [];
    if (pages.length === 0) continue;

    const tree = buildTree(pages);
    let listContent = renderTree(tree, 0);

    const content = `# ${term.toUpperCase()}\n\n${listContent}`;
    fs.writeFileSync(path.join(outputDir, `${slug}.md`), content, "utf8");
  }
}

/**
 * Строит дерево путей из массива страниц.
 * @param {Array<{title: string, href: string}>} pages
 * @returns {Object} корневой узел { children: [] }
 */
function buildTree(pages) {
  const root = { name: 'root', children: [], isPage: false, isRoot: true };

  function findOrCreateNode(parent, segments, create = true) {
    if (segments.length === 0) return parent;
    const [first, ...rest] = segments;
    let child = parent.children.find(c => c.name === first);
    if (!child && create) {
      child = { name: first, children: [], isPage: false };
      parent.children.push(child);
    }
    if (!child) return null;
    if (rest.length === 0) return child;
    return findOrCreateNode(child, rest, create);
  }

  const sortedPages = [...pages].sort((a, b) => a.href.length - b.href.length);

  for (const page of sortedPages) {
    const segments = page.href.split('/').filter(s => s.length > 0);
    const fileName = segments[segments.length - 1];
    const isIndex = fileName === 'index.md';

    if (isIndex) {
      // index.md → страница папки
      const folderSegments = segments.slice(0, -1);
      const folderNode = findOrCreateNode(root, folderSegments, true);
      folderNode.isPage = true;
      folderNode.href = page.href;
      folderNode.title = page.title;
      folderNode.size = page.size;
    } else {
      // обычный .md файл
      const parentSegments = segments.slice(0, -1);
      const parentNode = findOrCreateNode(root, parentSegments, true);
      const leafName = fileName.replace(/\.md$/, '');
      let leaf = parentNode.children.find(c => c.name === leafName);
      if (!leaf) {
        leaf = { name: leafName, children: [], isPage: true, href: page.href, title: page.title, size: page.size };
        parentNode.children.push(leaf);
      } else {
        // обновляем, если уже существует
        leaf.isPage = true;
        leaf.href = page.href;
        leaf.title = page.title;
        leaf.size = page.size;
      }
    }
  }

  return root;
}
/**
 * Форматирует размер в читаемый вид (Б, КБ, МБ)
 */
function formatSize(bytes) {
  if (typeof bytes !== 'number' || bytes < 0) return '';
  if (bytes === 0) return ' (0 Б)';
  const units = ['Б', 'КБ', 'МБ'];
  let value = bytes;
  let unitIndex = 0;
  while (value >= 1024 && unitIndex < units.length - 1) {
    value /= 1024;
    unitIndex++;
  }
  let display;
  if (unitIndex === 0) {
    // Байты — целое число
    display = Math.round(value);
  } else {
    // КБ и МБ — один знак после запятой, если меньше 10, иначе целое
    display = value >= 10 ? Math.round(value) : value.toFixed(1);
  }
  return ` (${display} ${units[unitIndex]})`;
}

/**
 * Рекурсивно рендерит дерево в Markdown-список с отступами.
 * @param {Object} node - узел дерева
 * @param {number} depth - уровень вложенности (для отступов)
 * @returns {string} Markdown-список
 */
function renderTree(node, depth) {
  const indent = '  '.repeat(depth);
  let result = '';
  const children = node.children || [];
  if (children.length === 0) return result;

  // Сортировка по отображаемому имени с учётом чисел
  const collator = new Intl.Collator('ru', { numeric: true, sensitivity: 'base' });
  const sorted = [...children].sort((a, b) => {
    const nameA = a.title || a.name || '';
    const nameB = b.title || b.name || '';
    return collator.compare(nameA, nameB);
  });

  for (const child of sorted) {
    if (child.isPage && child.href) {
      const sizeStr = formatSize(child.size);
      const link = `../${child.href}`;
      result += `${indent}- [${child.title || child.name}](${link})${sizeStr}\n`;
      if (child.children && child.children.length > 0) {
        result += renderTree(child, depth + 1);
      }
    } else {
      // Узел не страница (папка без index.md) – выводим пункт без ссылки
      result += `${indent}- ${child.title || child.name || 'Без названия'}\n`;
      if (child.children && child.children.length > 0) {
        result += renderTree(child, depth + 1);
      }
    }
  }
  return result;
}

function writeIndexMd(outputDir, sortedTerms, contextMap, lang, title) {
  const mapContent = generateMapContent({
    lang,
    sortedTerms,
    contextMap,
    width: 900,
    height: 450,
  });

  const listContent = generateListContent({
    lang,
    sortedTerms,
    contextMap,
  });

  const searchContent = generateSearchContent({
    lang,
    placeholder: lang === "ru" ? "Введите контексты через пробел..." : "Enter contexts separated by space...",
  });

  // 2. Собираем табы
  const tabs = [
    { title: lang === "ru" ? "Карта" : "Map", content: mapContent },
    { title: lang === "ru" ? "Алфавитный указатель" : "Alphabetical index", content: listContent },
    { title: lang === "ru" ? "Поиск" : "Search", content: searchContent },
  ];

  const tabsMarkup = generateTabsMarkup({ tabs, selectedIndex: 0 });

  // 3. Формируем итоговый контент
  let content = INDEX_MD_DEFAULT_CONTENT(title);
  content += `\n${tabsMarkup}`;

  fs.writeFileSync(path.join(outputDir, "index.md"), content.trim() + "\n", "utf8");
}

/**
 * Генерирует JS-скрипт для поиска по контекстам
 * @param {string} docsRoot - корневая папка документации
 */
function writeContextSearchScript(docsRoot) {
  const resolvedDocsRoot = path.resolve(docsRoot);
  const scriptsDir = path.join(resolvedDocsRoot, '_assets', 'scripts');
  if (!fs.existsSync(scriptsDir)) fs.mkdirSync(scriptsDir, { recursive: true });

  const clientScriptPath = path.join(__dirname, 'context-search.client.js');
  const clientScriptCode = fs.readFileSync(clientScriptPath, 'utf8');
  fs.writeFileSync(path.join(scriptsDir, 'context-search.js'), clientScriptCode, 'utf8');
}

/**
 * @param {string} outputDir
 * @param {string[]} sortedTerms
 * @param {ContextMap} contextMap
 * @param {string} lang
 */
function writeTocAndIndexYaml(outputDir, sortedTerms, contextMap, lang) {
  const title = lang === "ru" ? "Контексты" : "Contexts";
  const slugifiedItems = sortedTerms.map((t) => ({
    term: t,
    slug: slugify_filename(t),
  }));

  const tocItems = slugifiedItems.map((i) => TOC_YAML_LINK_TEMPLATE(i)).join("\n");
  fs.writeFileSync(path.join(outputDir, "toc.yaml"), TOC_YAML_LINKS_TEMPLATE(title, tocItems), "utf8");

  const linksYaml = slugifiedItems.map((i) => INDEX_YAML_LINK_TEMPLATE(i, contextMap)).join("\n");
  fs.writeFileSync(path.join(outputDir, "index.yaml"), INDEX_TAML_LINKS_TEMPLATE(title, linksYaml), "utf8");
}

module.exports = {
  writeTermFiles,
  writeIndexMd,
  writeTocAndIndexYaml,
  writeContextSearchScript,
};