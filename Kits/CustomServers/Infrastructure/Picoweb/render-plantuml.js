class RenderPlantUMLElement extends HTMLElement {
    constructor() {
        super();

        const publicDemoServerAddress = "http://localhost:8080";
        const validRenderModes = ["img", "png", "svg", "txt"];

        const externalDiagramSource = this.getAttribute("src");
        const plantUmlServerAddress = this.getAttribute("server") || publicDemoServerAddress;
        const renderMode = (this.getAttribute("renderMode") || "svg").toLowerCase();

        this.renderedContainer = this.attachShadow({ mode: "open" });
        this.loaderId = 'plantuml-loader'; 

        const styleSheet = document.createElement("style");
        styleSheet.textContent = ".error { color: red }";
        this.renderedContainer.appendChild(styleSheet);

        if (validRenderModes.indexOf(renderMode) < 0) {
            const errorMessage = document.createElement("div");
            errorMessage.classList.add("error");
            errorMessage.innerText =
                `Invalid render mode '${renderMode}'. ` +
                `Must be one of '${validRenderModes.join("', '")}'.`;
            this.renderedContainer.appendChild(errorMessage);
            return;
        }

        let diagramPromise;
        this.showLoader("Инициализация..."); 
        
        if (externalDiagramSource) {

            const absoluteUri = new URL(externalDiagramSource, document.location);
            diagramPromise = fetch(absoluteUri)
                .then(response => {
                    if (!response.ok) {
                        throw new Error(`Failed to fetch diagram from ${absoluteUri}: ${response.statusText}`);
                    }
                    return response.text();
                });
        } else {
            const content = Array.from(this.childNodes)
                .map((node) => node.textContent)
                .join("")
                .trim() || "license";
            
            const contentSize = new TextEncoder().encode(content).length;
            const formattedSize = this.formatBytes(contentSize);

            this.showLoader(`Контент загружен. Размер: ${formattedSize}.`);
            diagramPromise = Promise.resolve(content);
        }

        diagramPromise
            .then(diagramDescription => {
                const cleanedDiagramDescription = diagramDescription.trim();

                if (cleanedDiagramDescription.length === 0) {
                    throw new Error("Diagram content is empty after loading.");
                }

                const url = `${plantUmlServerAddress}/${renderMode}/`;
                // ===================================================
                // МОДИФИКАЦИЯ: Указание размера диаграммы перед рендерингом
                // ===================================================
                const finalContentSize = new TextEncoder().encode(cleanedDiagramDescription).length;
                const formattedFinalSize = this.formatBytes(finalContentSize);
                
                // ----------------------------------------------------
                // 2. ОБНОВИТЬ ИНДИКАТОР: НАЧАЛО РЕНДЕРИНГА НА СЕРВЕРЕ
                // ----------------------------------------------------
                this.showLoader(`Рендеринг диаграммы (${formattedFinalSize})... Пожалуйста, подождите.`);

                const fetchOptions = {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'text/plain; charset=utf-8'
                    },
                    body: cleanedDiagramDescription
                };

                return fetch(url, fetchOptions);
            })
            .then(response => {
                if (!response.ok) {
                    console.warn(`PlantUML Server returned HTTP status ${response.status} (${response.statusText}). Rendering error.`);
                }
                return response;
            })
            .then(response => {
                this.hideLoader();
                switch (renderMode) {
                    case "svg":
                        this.renderAsSvg(response);
                        break;
                    case "txt":
                        this.renderAsTxt(response);
                        break;
                    default:
                        this.renderAsPng(response);
                        break;
                }
            })
            .catch(error => {
                this.hideLoader();
                console.error("Error rendering PlantUML diagram:", error);
                const errorMessage = document.createElement("div");
                errorMessage.classList.add("error");
                errorMessage.innerText = `Error: ${error.message}`;
                this.renderedContainer.appendChild(errorMessage);
            });
    }

    // ----------------------------------------------------
    // НОВЫЙ МЕТОД: ФОРМАТИРОВАНИЕ РАЗМЕРА
    // ----------------------------------------------------
    formatBytes(bytes) {
        if (bytes === 0) return '0 B';

        const k = 1024;
        const units = ['B', 'k', 'M', 'G', 'T']; // Используем префиксы k, M, G
        
        // Выбираем правильный индекс (0: B, 1: k, 2: M, ...)
        const i = Math.floor(Math.log(bytes) / Math.log(k));
        
        // Форматируем с одним знаком после запятой, если это не байты
        return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + units[i];
    }
    
    // ----------------------------------------------------
    // МЕТОДЫ ДЛЯ УПРАВЛЕНИЯ ЗАГРУЗКОЙ (Без изменений)
    // ----------------------------------------------------

    showLoader(message) {
        this.hideLoader(); 
        const loader = document.createElement("div");
        loader.id = this.loaderId;
        loader.innerText = message;
        this.renderedContainer.appendChild(loader);
    }

    hideLoader() {
        const loader = this.renderedContainer.querySelector(`#${this.loaderId}`);
        if (loader) {
            loader.remove();
        }
    }

    // ... (Ваши методы renderAsPng, renderAsSvg, renderAsTxt)
    
    renderAsPng(response) {
       response.blob().then((pngContent) => {
           const image = document.createElement("img");
           image.src = window.URL.createObjectURL(pngContent);
           this.renderedContainer.appendChild(image);
       });
    }

    renderAsSvg(response) {
       response.text().then((svgContent) =>
           this.renderedContainer.appendChild(
               document.createRange().createContextualFragment(svgContent),
           ),
       );
    }

    renderAsTxt(response) {
       response.text().then((textContent) => {
           const preTag = document.createElement("pre");
           preTag.innerText = textContent;
           this.renderedContainer.appendChild(preTag);
       });
    }
}

export const enableElement = () => {
    window.customElements.define("render-plantuml", RenderPlantUMLElement);
};
export default enableElement;