// contexts/contexts.extractor.js

const fs = require('fs');
const path = require('path');
const { parse } = require('../utils/frontmatter.utils');

/**
 * Извлекает заголовок из содержимого файла
 * @param {string} content
 * @param {string} fullPath
 * @returns {string}
 */
function extractTitle(content, fullPath) {
    const { data } = parse(content);
    if (data.title && typeof data.title === 'string') {
        return data.title.trim();
    }

    const titleMatch = content.match(/^#\s+(.*)/m);
    if (titleMatch) {
        return titleMatch[1].trim();
    }

    return path.basename(fullPath, '.md');
}

/**
 * Извлекает значение context из frontmatter и дополняет карту
 * @param {string} fullPath
 * @param {string} langDir
 * @param {any} contextMap
 */
function extractContextTagValue(fullPath, langDir, contextMap) {
    const content = fs.readFileSync(fullPath, 'utf8');
    const { data, content: body } = parse(content); // отделяем frontmatter от тела
    const contextValue = data.context;

    if (!contextValue || typeof contextValue !== 'string') {
        return;
    }

    // Разбиваем на термины
    const terms = contextValue
        .split(/[\s,]+/)
        .map(t => t.trim().toLowerCase())
        .filter(t => t.length > 0);

    if (terms.length === 0) return;

    const relativeToLang = path.relative(langDir, fullPath).replace(/\\/g, '/');
    const title = extractTitle(content, fullPath);

    // Размер текстового содержимого (в байтах UTF-8)
    const size = Buffer.byteLength(body, 'utf8');

    for (const term of terms) {
        if (!contextMap[term]) {
            contextMap[term] = { rank: 0, pages: [] };
        }

        const alreadyExists = contextMap[term].pages.some(p => p.href === relativeToLang);
        if (!alreadyExists) {
            contextMap[term].rank += 1;
            contextMap[term].pages.push({
                title: title,
                href: relativeToLang,
                size: size,  // добавляем размер
            });
        }
    }
}

module.exports = { extractContextTagValue };