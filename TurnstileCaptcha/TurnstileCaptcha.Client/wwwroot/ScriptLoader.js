window.loadTurnstileScript = function (path) {
    if (loaded[path]) {

        return new this.Promise(function (resolve) {
            resolve();
        });
    }

    return new this.Promise(function (resolve, reject) {
        var script = document.createElement('script');
        script.src = path;
        script.onload = function () {
            loaded[path] = true;
            resolve(path);
        };
        script.onerror = function () {
            reject(path);
        };
        document.body.appendChild(script);
    });

}

loaded = [];