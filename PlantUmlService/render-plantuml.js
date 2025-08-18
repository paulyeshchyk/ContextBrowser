class RenderPlantUMLElement extends HTMLElement {
  constructor() {
    super();
    const publicDemoServerAddress = "https://www.plantuml.com/plantuml";
    const validRenderModes = ["img", "png", "svg", "txt"];
    const externalDiagramSource = this.getAttribute("src");
    const plantUmlServerAddress = this.getAttribute("server") || publicDemoServerAddress;
    const renderMode = (this.getAttribute("renderMode") || "svg").toLowerCase();
    let diagramDescription = "";
    if (externalDiagramSource) {
      const absoluteUri = new URL(externalDiagramSource, document.location);
      diagramDescription = `!include ${absoluteUri}`;
    } else {
      diagramDescription = Array.from(this.childNodes).map((node) => node.textContent).join("").trim() || "license";
    }
    const payload = diagramDescription.split("").map((char) => `0${char.codePointAt(0).toString(16)}`.slice(-2)).join("");
    this.renderedContainer = this.attachShadow({
      mode: "open"
    });
    const styleSheet = document.createElement("style");
    styleSheet.textContent = ".error { color: red }";
    this.renderedContainer.appendChild(styleSheet);
    if (validRenderModes.indexOf(renderMode) < 0) {
      const errorMessage = document.createElement("div");
      errorMessage.classList.add("error");
      errorMessage.innerText = `Invalid render mode '${renderMode}'. Must be one of '${validRenderModes.join("', '")}'.`;
      this.renderedContainer.appendChild(errorMessage);
    } else {
      fetch(`${plantUmlServerAddress}/${renderMode}/~h${payload}`).then((response) => {
        switch (renderMode) {
          case "svg":
            this.renderAsSvg(response);
            break;
          case "txt":
            this.renderAsTxt(response);
            break;
          default:
            this.renderAsPng(response);
        }
      });
    }
  }
  renderAsPng(response) {
    response.blob().then((pngContent) => {
      const image = document.createElement("img");
      image.src = window.URL.createObjectURL(pngContent);
      this.renderedContainer.appendChild(image);
    });
  }
  renderAsSvg(response) {
    response.text().then((svgContent) => this.renderedContainer.appendChild(document.createRange().createContextualFragment(svgContent)));
  }
  renderAsTxt(response) {
    response.text().then((textContent) => {
      const preTag = document.createElement("pre");
      preTag.innerText = textContent;
      this.renderedContainer.appendChild(preTag);
    });
  }
}
const enableElement = () => {
  window.customElements.define("render-plantuml", RenderPlantUMLElement);
};
export default enableElement;
export {enableElement};
