const isXpath = cssOrXpath => /^([\(\.]{0,3}\/|\.\.)/.test(cssOrXpath)
const fixXpath = xpath => xpath.replace(/^\//, "./")

const any$ = (cssOrXpath, parent = document) => !isXpath(cssOrXpath)
        ? parent.querySelector(cssOrXpath)
        : document.evaluate(fixXpath(cssOrXpath), parent, null, 9, null).singleNodeValue

const any$$ = (cssOrXpath, parent = document) => {
    if(!isXpath(cssOrXpath)) return parent.querySelectorAll(cssOrXpath)
    const query = document.evaluate(fixXpath(cssOrXpath), parent, null, 5, null);

    const results = []
    while (node = query.iterateNext()) results.push(node)
    return results
}

/**
 * @param cssOrXpathArgs {IArguments|string[]}
 * @returns {(number|*)[]|Document}
 */
function find(cssOrXpathArgs) {
    let curr = document;
    for (let i = 0; i < cssOrXpathArgs.length; i++) {
        let parent = curr;
        if (curr.shadowRoot) parent = curr.shadowRoot;
        if (curr.contentWindow) parent = curr.contentWindow.document

        curr = any$(cssOrXpathArgs[i], parent)
        if(curr == null) return i;
    }
    return curr;
}

/**
 * @param cssOrXpathArgs {IArguments|string[]}
 * @returns {(number|*)[]|Document}
 */
function findAll(cssOrXpathArgs) {
    const arr = [...cssOrXpathArgs]
    const lastSelector = arr.pop()
    const parent = arr ? find(arr) : document
    return any$$(lastSelector, parent)
}