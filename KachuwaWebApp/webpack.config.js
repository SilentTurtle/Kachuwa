/* eslint-disable */
const path = require('path');
const fs = require('fs');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const merge = require('webpack-merge');
const autoprefixer = require('autoprefixer');
const isDebug = false;//global.DEBUG === false ? false : !process.argv.includes('--release');
const APP_DIR = path.resolve(__dirname, "./reactapp/src");
// List all files in a directory in Node.js recursively in a synchronous fashion
var walkSync = function(dir, filelist) {

    if (dir[dir.length - 1] != '/')
        dir = dir.concat('/')


    files = fs.readdirSync(dir);
    filelist = filelist || [];
    files.forEach(function(file) {
        if (fs.statSync(dir + file).isDirectory()) {
            filelist = walkSync(dir + file + '/', filelist);
        } else {
            //console.log(file);
            // build regex search
            var re = new RegExp(/\pageconfig.(json)/g);
            if (re.test(file)) {
               // console.log("added {file}");
                filelist.push(dir + file);
            }
        }
    });

    // console.log(filelist);
    return filelist;


};
var getAppEntries = function(dir) {

    var files = walkSync(dir);
    //var parsedJSON = require(x);
    var mergeJSON = require("merge-json");
    var result = {};
    files.forEach(function(file) {
        var json = require(file);
        result = mergeJSON.merge(result, json);

    });
    console.log("final");
    console.log(result);
    return result;

}

module.exports = {
    entry: getAppEntries(APP_DIR), //{ "home":"./reactapp/src/pages/home/homepage.tsx" },
    output: {
        // `path` is the folder where Webpack will place your bundles
        path: path.resolve(__dirname, "./wwwroot/dist"),
        // `publicPath` is where Webpack will load your bundles from (optional)
        publicPath: "/dist/",
        filename: '[name].bundle.js',
        // `chunkFilename` provides a template for naming code-split bundles (optional)
        // chunkFilename: '[name].bundle_dynamic.js'
    },
    resolve: {
        extensions: ['.js', '.jsx', '.ts', '.tsx']
    },
    optimization: {
        splitChunks: {
            //  chunks: 'all'
        }
    },
    devServer: {
        historyApiFallback: true,
        contentBase: './wwwroot/dist',
        hot: true
    },
    module: {
        rules: [{
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader'
                }
            },
            {
                test: /\.(ts|tsx)?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },

            {
                test: /\.(png|jpg|jpeg|gif|svg)$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: 'images/[name].[hash:4].[ext]'
                            //outputPath: 'images/'
                    }
                }]
            },
            {
                test: /\.css$/,
                use: [
                    isDebug ? 'style-loader' : MiniCssExtractPlugin.loader,
                    //'style-loader' , MiniCssExtractPlugin.loader,
                    'css-loader',
                    {
                        loader: require.resolve('postcss-loader'),
                        options: {
                            // Necessary for external CSS imports to work
                            // https://github.com/facebookincubator/create-react-app/issues/2677
                            ident: 'postcss',
                            plugins: () => [
                                //require('postcss-flexbugs-fixes'),
                                autoprefixer({
                                    browsers: [
                                        '>1%',
                                        'last 4 versions',
                                        'Firefox ESR',
                                        'not ie < 9' // React doesn't support IE8 anyway
                                    ],
                                    flexbox: 'no-2009'
                                })
                            ]
                        }
                    }
                ]
            }
        ]
    },
    plugins: [
            //	new CleanWebpackPlugin(clientBundleOutputDir, { allowExternal: true }), //Moved to Prod
            new MiniCssExtractPlugin({
                // Options similar to the same options in webpackOptions.output
                // both options are optional
                filename: isDebug ? '[name].css' : 'styles/[name].[contenthash:4].css',
                chunkFilename: isDebug ? '[id].css' : 'styles/[id].[contenthash:4].css'
            })
        ] //.concat(htmlPlugins)
};