class RenderPlantUMLElement extends HTMLElement {
  constructor() {
    super();

    const publicDemoServerAddress = "http://localhost:8080";
    const validRenderModes = ["img", "png", "svg", "txt"];

    const externalDiagramSource = this.getAttribute("src");
    const plantUmlServerAddress =
      this.getAttribute("server") || publicDemoServerAddress;
    const renderMode = (this.getAttribute("renderMode") || "svg").toLowerCase();

    this.renderedContainer = this.attachShadow({ mode: "open" });

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
    if (externalDiagramSource) {
      // Загрузка контента из внешнего источника
      const absoluteUri = new URL(externalDiagramSource, document.location);
      diagramPromise = fetch(absoluteUri)
        .then(response => {
          if (!response.ok) {
            throw new Error(`Failed to fetch diagram from ${absoluteUri}: ${response.statusText}`);
          }
          return response.text();
        });
    } else {
      // Получение контента из тела тега
      const content = Array.from(this.childNodes)
        .map((node) => node.textContent)
        .join("")
        .trim() || "license";
      diagramPromise = Promise.resolve(content);
    }

    diagramPromise
      .then(diagramDescription => {
        // Кодирование содержимого диаграммы в hex
        const payload = diagramDescription
          .split("")
          .map(
            (char) => `0${char.codePointAt(0).toString(16)}`.slice(-2),
          )
          .join("");

        // Отправка запроса на PlantUML-сервер с закодированным контентом
        return fetch(`${plantUmlServerAddress}/${renderMode}/~h${payload}`);
      })
      .then(response => {
        if (!response.ok) {
          throw new Error(`PlantUML server responded with an error: ${response.statusText}`);
        }
        return response;
      })
      .then(response => {
        // Рендеринг результата в зависимости от режима
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
        console.error("Error rendering PlantUML diagram:", error);
        const errorMessage = document.createElement("div");
        errorMessage.classList.add("error");
        errorMessage.innerText = `Error: ${error.message}`;
        this.renderedContainer.appendChild(errorMessage);
      });
  }

  renderAsPng(response) {
    response.blob().then((pngContent) => {
      const image = document.createElement("img");
      image.src = window.URL.createObjectURL(pngContent);
      this.renderedContainer.appendChild(image);
    });
  }

  renderAsSvg(response) {
    response
      .text()
      .then((svgContent) =>
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