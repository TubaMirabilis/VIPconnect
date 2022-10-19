var request = new XMLHttpRequest();
request.open("GET", "js/lang.json", false);
request.send(null)
var localList = JSON.parse(request.responseText).words;

class Filter {
    constructor(options = {}) {
        Object.assign(this, {
            list: localList || [],
            exclude: options.exclude || [],
            splitRegex: options.splitRegex || /\b/,
            placeHolder: options.placeHolder || '*',
            regex: options.regex || /[^a-zA-Z0-9|\$|\@]|\^/g,
            replaceRegex: options.replaceRegex || /\w/g
        })
    }

    isProfane(string) {
        return this.list
            .filter((word) => {
                const wordExp = new RegExp(`\\b${word.replace(/(\W)/g, '\\$1')}\\b`, 'gi');
                return !this.exclude.includes(word.toLowerCase()) && wordExp.test(string);
            })
            .length > 0 || false;
    }
}