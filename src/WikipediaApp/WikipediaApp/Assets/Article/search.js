var results = [];

function search(query) {
    _clearSearch();

    if (query.length > 0) {
        _performSearch(query);
    }

    window.external.notify(results.length.toString());
}

function _clearSearch() {
    for (let i = 0; i < results.length; ++i) {
        const resultNode = results[i];
        const parentNode = resultNode.parentNode;

        const textNode = document.createTextNode(resultNode.textContent);

        parentNode.replaceChild(textNode, resultNode);
        parentNode.normalize();
    }

    results = [];
}

function _performSearch(query) {
    const queue = [document.body];
    const re = new RegExp(query, 'gi');

    let currentNode;

    while ((currentNode = queue.pop())) {
        if (currentNode.tagName === "SCRIPT" || !currentNode.textContent.match(re))
            continue;

        let match;

        for (let i = 0; i < currentNode.childNodes.length; ++i) {
            const childNode = currentNode.childNodes[i];

            switch (childNode.nodeType) {
            case Node.TEXT_NODE: // 3
                while ((match = re.exec(childNode.textContent))) {
                    const index = match.index;

                    const splitNode = childNode.splitText(index);
                    splitNode.splitText(query.length);

                    const spanNode = document.createElement("span");
                    spanNode.className = "highlight";

                    currentNode.replaceChild(spanNode, splitNode);
                    spanNode.appendChild(splitNode);

                    results.push(spanNode);
                    ++i;
                }
                break;
            case Node.ELEMENT_NODE: // 1
                queue.push(childNode);
                break;
            }
        }
    }
}