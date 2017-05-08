var debug = process.env.NODE_ENV !== "production";
var webpack = require('webpack');
var path = require('path');
// List all files in a directory in Node.js recursively in a synchronous fashion
var walkSync = function (dir, filelist) {

    if (dir[dir.length - 1] != '/')
        dir = dir.concat('/')

    var fs = fs || require('fs'),
        files = fs.readdirSync(dir);
    filelist = filelist || [];
    files.forEach(function (file) {
        if (fs.statSync(dir + file).isDirectory()) {
            filelist = walkSync(dir + file + '/', filelist);
        }
        else {
            // build regex search
            var re = new RegExp(/\kjsmodule.(json)/g);
            if (re.test(file)) {
                console.log(file);
                filelist.push(dir + file);
            }
        }
    });
    return filelist;


};
var getAppEntries = function (dir) {
    console.log(dir);
    var files = walkSync(dir);
    //var parsedJSON = require(x);
    var mergeJson = require("merge-json");
    var result = {};
    files.forEach(function (file) {
        var json = require(file);
        //console.log(json)
        result = mergeJson.merge(result, json);

    });
    console.log(result);
    return result;
}

module.exports = {
    context: __dirname,
    devtool: debug ? "sourcemap" : false,//or sourcemap
    entry: getAppEntries(__dirname),
    output: {
        path: path.join(__dirname, 'wwwroot/resources'),
        filename: '[name]/dist/[name].bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                loader: 'ts-loader',
                include: path.resolve(__dirname, 'ts'),
                exclude: /node_modules/,
            },
        ]
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js"]
    }
    ,
    plugins: debug ? [] : [
        //remove duplicate codes
        // new webpack.optimize.DedupePlugin(),
        new webpack.optimize.OccurrenceOrderPlugin(),
        //uglify
        new webpack.optimize.UglifyJsPlugin({ minimize: true, compressor: { warnings: false } })
        // new webpack.optimize.UglifyJsPlugin({ mangle: false, sourcemap: false }),
    ],
};