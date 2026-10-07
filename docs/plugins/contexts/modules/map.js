// modules/map.js

const { slugify_filename } = require('../../utils/encoding.slugify');

/**
 * Генерирует HTML/SVG для вкладки "Карта"
 * @param {Object} params
 * @param {string} params.lang - 'ru' или 'en'
 * @param {string[]} params.sortedTerms - отсортированные термины
 * @param {ContextMap} params.contextMap - объект с данными
 * @param {number} [params.width=900] - ширина SVG
 * @param {number} [params.height=450] - высота SVG
 * @returns {string} SVG-разметка с обёрткой
 */
function generateMapContent({ lang, sortedTerms, contextMap, width = 900, height = 450 }) {
  const elements = sortedTerms.map((term) => ({
    name: term,
    weight: contextMap[term]?.rank || 0,
  }));
  elements.sort((a, b) => b.weight - a.weight);

  const rects = computeTreemap(elements, width, height);
  const svgHtml = generateSvgHeatmap(rects, width, height);
  // Экранируем фигурные скобки для шаблонизатора
  return svgHtml.replace(/{/g, '&#123;').replace(/}/g, '&#125;');
}

// --- Вспомогательные функции (перенесены из исходного файла) ---

function computeTreemap(elements, width, height) {
  const scaledElements = elements
    .map((el) => ({ name: el.name, weight: el.weight, value: Math.log(el.weight + 1) }))
    .filter((el) => el.value > 0);

  if (scaledElements.length === 0) return [];

  scaledElements.sort((a, b) => b.value - a.value);
  const totalValue = scaledElements.reduce((sum, el) => sum + el.value, 0);
  const totalArea = width * height;
  scaledElements.forEach((el) => (el.area = (el.value / totalValue) * totalArea));

  const rects = [];
  let x = 0, y = 0;
  let w = width, h = height;

  let i = 0;
  while (i < scaledElements.length) {
    let row = [];
    let currentShortestSide = Math.min(w, h);

    while (i < scaledElements.length) {
      const nextElement = scaledElements[i];
      const testRow = [...row, nextElement];
      if (isWorstAspectRatio(testRow, currentShortestSide)) break;
      row.push(nextElement);
      i++;
    }

    const rowArea = row.reduce((sum, el) => sum + el.area, 0);
    const isHorizontal = w > h;

    if (isHorizontal) {
      const rowWidth = rowArea / h;
      let currentY = y;
      row.forEach((el) => {
        const elHeight = el.area / rowWidth;
        rects.push({ name: el.name, weight: el.weight, x, y: currentY, w: rowWidth, h: elHeight });
        currentY += elHeight;
      });
      x += rowWidth;
      w -= rowWidth;
    } else {
      const rowHeight = rowArea / w;
      let currentX = x;
      row.forEach((el) => {
        const elWidth = el.area / rowHeight;
        rects.push({ name: el.name, weight: el.weight, x: currentX, y, w: elWidth, h: rowHeight });
        currentX += elWidth;
      });
      y += rowHeight;
      h -= rowHeight;
    }
  }

  // Корректировка краёв
  rects.forEach((r) => {
    if (Math.abs(r.x + r.w - width) < 5) r.w = width - r.x;
    if (Math.abs(r.y + r.h - height) < 5) r.h = height - r.y;
    r.x = Math.round(r.x);
    r.y = Math.round(r.y);
    r.w = Math.max(Math.round(r.w), 1);
    r.h = Math.max(Math.round(r.h), 1);
  });

  return rects;
}

function isWorstAspectRatio(row, side) {
  if (row.length === 0) return true;
  const sumArea = row.reduce((sum, el) => sum + el.area, 0);
  if (sumArea === 0) return true;

  const maxArea = Math.max(...row.map((el) => el.area));
  const minArea = Math.min(...row.map((el) => el.area));
  const sideSq = side * side;

  const worstWithCurrent = Math.max(
    (sideSq * maxArea) / (sumArea * sumArea),
    (sumArea * sumArea) / (sideSq * minArea)
  );

  if (row.length === 1) return false;

  const prevRow = row.slice(0, -1);
  const prevSumArea = prevRow.reduce((sum, el) => sum + el.area, 0);
  const prevMaxArea = Math.max(...prevRow.map((el) => el.area));
  const prevMinArea = Math.min(...prevRow.map((el) => el.area));

  const worstBefore = Math.max(
    (sideSq * prevMaxArea) / (prevSumArea * prevSumArea),
    (prevSumArea * prevSumArea) / (sideSq * prevMinArea)
  );

  return worstWithCurrent > worstBefore;
}

function generateSvgHeatmap(rects, totalWidth, totalHeight) {
  let svg = `<svg viewBox="0 0 ${totalWidth} ${totalHeight}" width="100%" height="auto" style="border-radius: 8px; font-family: system-ui, -apple-system, sans-serif; user-select: none; overflow: visible;">\n`;
  svg += `  <style>
    .rect-box { transition: fill 0.2s ease, stroke 0.2s ease; }
    .rect-link:hover .rect-box { fill: #1a80b6 !important; stroke: #000000 !important; }
    .heatmap-text { filter: drop-shadow(0 1px 2px rgba(0,0,0,0.5)); text-rendering: geometricPrecision; }
  </style>\n\n`;

  const filtered = rects.filter(r => r.weight > 0);
  const maxWeight = Math.max(...filtered.map(r => r.weight), 1);

  filtered.forEach((r) => {
    const slug = slugify_filename(r.name);
    const opacity = 0.4 + (r.weight / maxWeight) * 0.6;
    const fillColor = `hsla(14, 88%, 40%, ${opacity.toFixed(2)})`;

    const sizeDimension = Math.min(r.w / 6, r.h / 3.2);
    const fontSize = Math.max(Math.min(Math.round(sizeDimension), 10), 5);
    const countFontSize = Math.max(Math.round(fontSize * 0.8), 8);
    const showText = r.w > 40 && r.h > 30;

    let displayName = r.name;
    const padding = 10;
    const estimatedCharWidth = fontSize * 0.7;
    const maxChars = Math.floor((r.w - padding) / estimatedCharWidth);
    if (displayName.length > maxChars && maxChars > 2) {
      displayName = displayName.substring(0, maxChars - 2) + '…';
    } else if (displayName.length > maxChars) {
      displayName = displayName.substring(0, Math.max(maxChars, 1));
    }
    const showCount = r.w >= 45;

    svg += `  <a href="ru/contexts/${slug}.html" class="rect-link" title="${r.name} (Найдено статей: ${r.weight})">\n`;
    svg += `    <rect class="rect-box" x="${r.x}" y="${r.y}" width="${r.w - 3}" height="${r.h - 3}" rx="6" fill="${fillColor}" stroke="#ffffff" stroke-width="1.5"/>\n`;

    if (showText) {
      const centerX = r.x + r.w / 2;
      const centerY = r.y + r.h / 2;
      svg += `    <text class="heatmap-text" x="${centerX}" y="${centerY}" fill="#ffffff" font-size="${fontSize}" font-weight="700" text-anchor="middle" dominant-baseline="central">\n`;
      svg += `      <tspan x="${centerX}" dy="-0.6em">${displayName}</tspan>\n`;
      if (showCount) {
        svg += `      <tspan x="${centerX}" dy="1.4em" font-weight="400" font-size="${countFontSize}" opacity="0.95">${r.weight} ст.</tspan>\n`;
      }
      svg += `    </text>\n`;
    }
    svg += `  </a>\n`;
  });

  svg += `</svg>\n`;
  return svg;
}

module.exports = { generateMapContent };