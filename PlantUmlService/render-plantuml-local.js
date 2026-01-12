import plantumlEncoder from 'https://cdn.jsdelivr.net/npm/plantuml-encoder@1.4.0/dist/plantuml-encoder.esm.min.js';

export default function enableElement() {
  document.querySelectorAll('render-plantuml').forEach(el => {
    const umlText = el.textContent.trim();
    const encoded = plantumlEncoder.encode(umlText);
    const renderMode = el.getAttribute("renderMode") || "svg";
    const url = `http://localhost:8081/${renderMode}/${encoded}`;

    const img = document.createElement("img");
    img.src = url;
    el.replaceWith(img);
  });
}